#define XDEBUG_LOG
using System.Text;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TinyName : Attribute
{
    private string name;
    public TinyName(string name)
    {
        this.name = name;
    }

    public string Name { get { return name; } }
}

public class TinyReader
{
    public static List<T> Load<T>(string content, char separateChar = '\t') where T : new()
    {
        if (string.IsNullOrEmpty(content)) return null;

        // float time = Time.realtimeSinceStartup;

        var records = new List<T>();

        // map fields
        var fieldMapper = new Dictionary<string, FieldInfo>();
        var customAttrMapper = new Dictionary<string, FieldInfo>();
        InitField<T>(fieldMapper, customAttrMapper);

        // Debug.LogError("======= mapping time: " + (Time.realtimeSinceStartup - time));
        // time = Time.realtimeSinceStartup;

        var attributes = new List<string>();
        var value = new StringBuilder();
        var index = 0;
        var lineIndex = 0;
        var record = new T();
#if XDEBUG_LOG
        var lineStr = string.Empty;
#endif

        for (int i = 0; i < content.Length; i++)
        {
            var endOfContent = (i == (content.Length - 1));
            if (content[i] != separateChar && content[i] != EndLine)
            {
                value.Append(content[i]);
                if (!endOfContent) continue;  
            }
           
            var val = value.ToString().Trim();

            if (lineIndex == 0)
            {
                // get attributes
                if (!string.IsNullOrEmpty(val)) attributes.Add(val);
            }
            else
            {
                if (attributes.Count <= index)
                {
                    Debug.LogWarning("Songthing wrong, value out of attribute range");
                }
                else
                {
                    var attr = attributes[index];
#if XDEBUG_LOG
                    lineStr += attr + ": " + val + " - ";
#endif
                    FieldInfo field = null;

                    if (fieldMapper.ContainsKey(attr))
                    {
                        field = fieldMapper[attr];
                    }
                    else if (customAttrMapper.ContainsKey(attr))
                    {
                        field = customAttrMapper[attr];
                    }

                    if (field != null)
                    {
                        ParseField<T>(record, field, val);
                    }
                    else
                    {
#if UNITY_EDITOR
                        Debug.LogWarning("Cannot find field: " + attr + "  " + attr.Length);
#endif
                    }
                    index++;
                }
            }

            value.Length = 0;

            if (content[i] == EndLine || endOfContent)
            {
                if (lineIndex != 0)
                {
                    if (record != null) records.Add(record);
                }

#if XDEBUG_LOG
                UnityEngine.Debug.Log("line[" + lineIndex + "]: " + lineStr);
                lineStr = string.Empty;
#endif

                if (endOfContent) break;

                // new line
                lineIndex++;
                index = 0;
                record = new T();
            }
        }

        // Debug.LogError("======= parse time: " + (Time.realtimeSinceStartup - time));
        // time = Time.realtimeSinceStartup;

        return records;
    }

    const char EndLine = '\n';
    private static void InitField<T>(Dictionary<string, FieldInfo> fieldMapper, Dictionary<string, FieldInfo> customAttrMapper)
    {
        var type = typeof(T);
        var allFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        for (int k = 0; k < allFields.Length; k++)
        {
            var fi = allFields[k];
            var name = fi.Name.Trim();
            if (fieldMapper.ContainsKey(name))
            {
                fieldMapper[name] = fi;
            }
            else
            {
                fieldMapper.Add(name, fi);
            }

            var attrs = fi.GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                if (attr.GetType() != typeof(TinyName)) continue;

                var customAttr = attr as TinyName;
                var attrName = customAttr.Name.Trim();

                if (customAttrMapper.ContainsKey(attrName))
                {
                    customAttrMapper[attrName] = fi;
                    // Debug.Log("Dupplicated custome name! Field: " + attr + " <---> CustomName: " + attrName);
                }
                else
                {
                    customAttrMapper.Add(attrName, fi);
                    // Debug.Log("Add new Field: " + attr + " - CustomName: " + attrName);
                }
            }
        }
    }

    private static bool ParseField<T>(T target, FieldInfo field, string value)
    {
        var fieldType = field.FieldType;

        if (fieldType == typeof(string))
        {
            field.SetValue(target, value);
            return true;
        }

        if (string.IsNullOrEmpty(value)) return false;

        if (fieldType == typeof(int))
        {
            var result = 0;
            if (Int32.TryParse(value, out result))
            {
                field.SetValue(target, result);
                return true;
            }
            else
            {
                Debug.LogWarning("Parse int fail: val " + value);
                return false;
            }
        }

        if (fieldType == typeof(float))
        {
            Debug.Log("value: " + value);
            float result = 0.0f;

            var style = System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowCurrencySymbol;
            var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");

            if (float.TryParse(value, style, culture, out result))
            {
                Debug.Log("result:" + result);
                field.SetValue(target, result);
                return true;
            }
        }

        if (fieldType == typeof(bool) && value.Length == 1)
        {
            field.SetValue(target, value == "1");
            return true;
        }
        if (fieldType == typeof(Vector3))
        {
            VectorParse(target, field, value);
            return true;
        }
        if (fieldType.IsEnum)
        {
            try
            {
                field.SetValue(target, Enum.Parse(fieldType, value));
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Array/Generic support
        if (fieldType.IsGenericType == true && fieldType.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (fieldType.GetGenericArguments()[0] == typeof(int))
            {
                var lst = new List<int>();
                if (!string.IsNullOrEmpty(value))
                {
                    var arrays = value.Split(',');
                    for (int i = 0; i < arrays.Length; i++)
                    {
                        if (int.TryParse(arrays[i], out int val))
                        {
                            lst.Add(val);
                        }
                    }
                }
                field.SetValue(target, lst);
                return true;
            }

            if (fieldType.GetGenericArguments()[0] == typeof(string))
            {
                var lst = new List<string>();
                if (!string.IsNullOrEmpty(value))
                {
                    var arrays = value.Split(',');
                    for (int i = 0; i < arrays.Length; i++)
                    {
                        var val = arrays[i];
                        if (string.IsNullOrEmpty(val)) continue;

                        lst.Add(val);
                    }
                }
                field.SetValue(target, lst);
                return true;
            }
        }

#if XDEBUG_LOG || UNITY_EDITOR
        Debug.LogWarning("Unsupported type: " + fieldType);
#endif
        return false;
    }

    public static object GetValue(Type t, string value)
    {
        var fieldType = t;

        if (fieldType == typeof(string))
        {
            return value;
        }

        if (string.IsNullOrEmpty(value)) return false;

        if (fieldType == typeof(int))
        {
            var result = 0;
            if (Int32.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                Debug.LogWarning("Parse int fail: val " + value);
                return result;
            }
        }

        if (fieldType == typeof(float))
        {
            float result = 0.0f;
            if (float.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                Debug.LogWarning("Parse float fail: val " + value);
                return result;
            }
        }

        if (fieldType == typeof(bool) && value.Length == 1)
        {
            return value == "1";
        }
        if (fieldType == typeof(Vector3))
        {
            return GetVector3(value);
        }
        if (fieldType.IsEnum)
        {
            try
            {
                return Enum.Parse(fieldType, value);
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    private static bool VectorParse<T>(T target, FieldInfo field, string val)
    {
        var s = val.Split(',');
        if (s.Length != 3) return false;
        Vector3 vector3 = Vector3.zero;
        float.TryParse(s[0], out vector3.x);
        float.TryParse(s[1], out vector3.y);
        float.TryParse(s[2], out vector3.z);
        field.SetValue(target, vector3);
        return true;
    }

    private static Vector3 GetVector3(string val)
    {
        var s = val.Split(',');
        if (s.Length != 3) return Vector3.zero;
        Vector3 vector3 = Vector3.zero;
        float.TryParse(s[0], out vector3.x);
        float.TryParse(s[1], out vector3.y);
        float.TryParse(s[2], out vector3.z);
        return vector3;
    }



    public static IEnumerable<KeyValuePair<string, string>> GetHorizontalData(string csv, char Delimiter = '\t')
    {
        var length = csv.Length;
        StringBuilder k = new StringBuilder();
        StringBuilder v = new StringBuilder();
        bool foundKey = false;
        for (int i = 0; i < length; i++)
        {
            var c = csv[i];
            if (c == EndLine || (i == length - 1))
            {
                if (i == length - 1)
                {
                    v.Append(c);
                }
                yield return new KeyValuePair<string, string>(k.ToString().Trim(), v.ToString().Trim());
                k.Length = 0;
                v.Length = 0;
                foundKey = false;
                continue;
            }
            if (c == Delimiter)
            {
                foundKey = true;
                continue;
            }
            if (foundKey)
            {
                v.Append(c);
            }
            else
            {
                k.Append(c);
            }
        }
    }
}