

#if UNITY_5_3_OR_NEWER
    #define UNITY_4_3_OR_NEWER
    #define UNITY_4_4_OR_NEWER
    #define UNITY_4_5_OR_NEWER
    #define UNITY_4_6_OR_NEWER
    #define UNITY_4_7_OR_NEWER
    #define UNITY_5_0_OR_NEWER
    #define UNITY_5_1_OR_NEWER
    #define UNITY_5_2_OR_NEWER
#else
    #if UNITY_5
    #define UNITY_4_3_OR_NEWER
    #define UNITY_4_4_OR_NEWER
    #define UNITY_4_5_OR_NEWER
    #define UNITY_4_6_OR_NEWER
    #define UNITY_4_7_OR_NEWER

    #if UNITY_5_0
    #define UNITY_5_0_OR_NEWER
    #elif UNITY_5_1
#define UNITY_5_0_OR_NEWER
#define UNITY_5_1_OR_NEWER
#elif UNITY_5_2
#define UNITY_5_0_OR_NEWER
#define UNITY_5_1_OR_NEWER
#define UNITY_5_2_OR_NEWER
#endif
#else
#if UNITY_4_3
#define UNITY_4_3_OR_NEWER
#elif UNITY_4_4
#define UNITY_4_3_OR_NEWER
#define UNITY_4_4_OR_NEWER
#elif UNITY_4_5
#define UNITY_4_3_OR_NEWER
#define UNITY_4_4_OR_NEWER
#define UNITY_4_5_OR_NEWER
#elif UNITY_4_6
#define UNITY_4_3_OR_NEWER
#define UNITY_4_4_OR_NEWER
#define UNITY_4_5_OR_NEWER
#define UNITY_4_6_OR_NEWER
#elif UNITY_4_7
#define UNITY_4_3_OR_NEWER
#define UNITY_4_4_OR_NEWER
#define UNITY_4_5_OR_NEWER
#define UNITY_4_6_OR_NEWER
#define UNITY_4_7_OR_NEWER
#endif
#endif
#endif

using System;
using System.Reflection;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;


using Object = UnityEngine.Object;

namespace vietlabs.vs
{
    public class VisualState_Unity
    {
        const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public |
                                   BindingFlags.NonPublic;

#if UNITY_EDITOR
        /// <summary>
        /// index 0 is value from
        /// </summary>
        public static PropertyModification[] GetValueModification(UndoPropertyModification undoProperty)
        {
            PropertyModification[] arr = new PropertyModification[2];
#if UNITY_5_1_OR_NEWER
        arr[0] = undoProperty.previousValue;
        arr[1] = undoProperty.currentValue;
#else
            arr[0] = undoProperty.propertyModification;
            arr[1] = GetCurModificationTo(undoProperty.propertyModification);
#endif
            return arr;
        }

#endif
        /// <summary>
        /// check and set dirty to tell unity update target
        /// </summary>
        public static void CheckUpdateDirtyState(UnityEngine.Object target)
        {
#if UNITY_4_6_OR_NEWER
        if (target is UnityEngine.UI.Graphic)
        {
            ((UnityEngine.UI.Graphic)target).SetAllDirty();
        }
        if (target is UnityEngine.UI.MaskableGraphic)
        {
            ((UnityEngine.UI.MaskableGraphic)target).SetAllDirty();
        }
#endif
        }
//#if !UNITY_5_1_OR_NEWER

        public static string ToHtmlStringRGBA(Color color)
        {
            // Round to int to prevent precision issues that, for example cause values very close to 1 to become FE instead of FF (case 770904).
            Color32 col32 = new Color32(
                (byte) Mathf.Clamp(Mathf.RoundToInt(color.r * 255), 0, 255),
                (byte) Mathf.Clamp(Mathf.RoundToInt(color.g * 255), 0, 255),
                (byte) Mathf.Clamp(Mathf.RoundToInt(color.b * 255), 0, 255),
                (byte) Mathf.Clamp(Mathf.RoundToInt(color.a * 255), 0, 255));

            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", col32.r, col32.g, col32.b, col32.a);
        }
        public static bool TryParseHtmlString(string htmlString, out Color32 color)
        {
            try
            {
                byte a = (byte) Convert.ToInt32(htmlString[0].ToString() + htmlString[1], 16);
                byte b = (byte) Convert.ToInt32(htmlString[2].ToString() + htmlString[3], 16);
                byte g = (byte) Convert.ToInt32(htmlString[4].ToString() + htmlString[5], 16);
                byte r = (byte) Convert.ToInt32(htmlString[6].ToString() + htmlString[7], 16);
                color = new Color32(r, g, b, a);
                return true;
            }
            catch (Exception)
            {
                color = Color.white;
                return false;
            }
        }
        public static string ReverseColor(string s)
        {
            return s[6].ToString() + s[7] + s[4] + s[5] + s[2] + s[3] + s[0] +s[1];
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

#if UNITY_EDITOR
        public static bool CheckAddColorGUIText4_5(PropertyModification from, PropertyModification to,
            VisualState visualState)
        {
#if !UNITY_4_5
        return false;
#endif
            string path = from.propertyPath;
            if (path == "m_Color.rgba") //gui text of unity
            {
                var vs = visualState.FindProperty(visualState.listValue, to.target, to.propertyPath);
                if (vs != null)
                {
                    if (vs.isNormal) vs.WriteChange(visualState.state, to);
                    return true;
                }

                // not existed, create new!
                vs = visualState.NewProperty<VSValue>(null, to.propertyPath, from, to);
                if (vs != null)
                {
                    visualState.listValue.Add(vs);
                    return true;
                }

                
            }

            return false;
        }

        public static PropertyModification GetCurModificationTo(PropertyModification mor)
        {
            PropertyModification f = new PropertyModification();
            f.target = mor.target;
            f.propertyPath = mor.propertyPath;
            var p = mor.propertyPath;
            var arr = p.Split('.');
            //arr = arr[0].ToString().ToLower() + arr.Substring(1);
            var targetT = mor.target.GetType();
#if UNITY_4_5
            if (p == "m_Color.rgba")
            {
                //mor.value = Reverse(mor.value);
                mor.value = long.Parse(mor.value).ToString("X");
                object data = GetFieldOrPropertyData(targetT, mor.target, getRealPropertyName(arr[0]));
                Color col = (Color) data;
                f.value = ReverseColor(ToHtmlStringRGBA(col));
                return f;
            }
#endif
            if (arr.Length == 1)
            {
                object data = GetFieldOrPropertyData(targetT, mor.target, getRealPropertyName(arr[0]));
                if (string.IsNullOrEmpty(mor.value))
                {
                    f.objectReference = (Object) data;
                }
                else
                {
                    f.value = data.ToString();
                }
            }
            else if (arr.Length == 2)
            {
                //struct
                object data = GetFieldOrPropertyData(targetT, mor.target, getRealPropertyName(arr[0]));
                object data1 = GetFieldOrPropertyData(data.GetType(), data, arr[1]);
                if (string.IsNullOrEmpty(mor.value))
                {
                    f.objectReference = (Object) data1;
                }
                else
                {
                    f.value = data1.ToString();
                }
            }

            return f;
        }

        private static string getRealPropertyName(string input)
        {
            input = input.Substring(2);
            return input[0].ToString().ToLower() + input.Substring(1);
        }

        private static object GetFieldOrPropertyData(Type type, object target, string property)
        {
            var field = type.GetField(property, FLAGS);
            if (field == null)
            {
                PropertyInfo info = type.GetProperty(property, FLAGS);
                if (info == null)
                {

                }

                return type.GetProperty(property, FLAGS).GetValue(target, null);
            }
            else
            {
                return field.GetValue(target);
            }
        }
        static HashSet<Type> SupportTweenType = new HashSet<Type>(){
            typeof(Color), 
            typeof(Color32), 
            typeof(Vector2), 
            typeof(Vector3)
        };
        
        public static bool isSupportTween(Type type)
        {
            return SupportTweenType.Contains(type);
        }
        

#endif
    private static object GetValue<T>(object val)
    {
        var valueT = val.GetType();
        var targetT = typeof(T);
        if(valueT == targetT)
        {
            return (T)val;
        }
        else if(valueT == typeof(string))
        {
            var s = val.ToString();
            if(targetT == typeof(int))
            {
                int result;
                int.TryParse(s, out result);
                return result;
            }
            if(targetT == typeof(float))
            {
                float result;
                float.TryParse(s, out result);
                return result;
            }
            if(targetT == typeof(double))
            {
                double result;
                double.TryParse(s, out result);
                return result;
            }
        }
        // Debug.LogWarning("Type not support " + targetT + "  " + valueT);
        return val;
    }

    public static bool GetPercentValue(object val, float amount, out object result)
    {
        var type = val.GetType();
            result = null;
            if(type == typeof(int))
            {
                result = (int) ((int)GetValue<int>(val) * amount);
            }
            if(type == typeof(float))
            {
                result = ((float)GetValue<float>(val) * amount);
            }
            if(type == typeof(double))
            {
                result = ((double)GetValue<double>(val) * amount);
            }

            if(type == typeof(Vector2))  
            {
                result = (Vector2) ((Vector2)GetValue<Vector2>(val) * amount);
            }
            if(type == typeof(Vector3))
            {
                result = (Vector3) ((Vector3)GetValue<Vector3>(val) * amount);
            }
            if(type == typeof(Vector4))
            {
                result = (Vector4) ((Vector4)GetValue<Vector4>(val) * amount);
            }



            if(type == typeof(Color))
            {
                result = (Color) ((Color)GetValue<Color>(val) * amount);
            }
            if(type == typeof(Color32))
            {
                result = (Color32) ((Color)GetValue<Color32>(val) * amount);
            }
            if(result == null)
            {
                result = result = val;
                // Debug.LogWarning("Type: " + type + " is NOT supported, will return " + val);
                return false;
            }
            return true;
    }


    public static bool GetLerpValue(object old, object newV, float amount, out object result)
        {
            //if(!isSupportTween(old.GetType()))
            //{
            //    Debug.LogWarning("Type: " + old.GetType() + " is NOT supported");
            //    return newV;
            //}
            var type = newV.GetType();
            result = null;
            if(type == typeof(int))
            {
                result = (int)Mathf.LerpUnclamped((int) GetValue<int>(old), (int)GetValue<int>(newV), amount);
            }
            if(type == typeof(float))
            {
                result = Mathf.LerpUnclamped((float) GetValue<float>(old), (float)GetValue<float>(newV), amount);
            }
            if(type == typeof(double))
            {
                result = (double)Mathf.LerpUnclamped((float) GetValue<float>(old), (float)GetValue<float>(newV), amount);
            }

            if(type == typeof(Vector2))
            {
                result = Vector2.LerpUnclamped((Vector2) GetValue<Vector2>(old), (Vector2) GetValue<Vector2>(newV), amount);
            }
            if(type == typeof(Vector3))
            {
                result = Vector3.LerpUnclamped((Vector3) GetValue<Vector3>(old), (Vector3) GetValue<Vector3>(newV), amount);
            }
            if(type == typeof(Vector4))
            {
                result = Vector4.LerpUnclamped((Vector4) GetValue<Vector4>(old), (Vector4) GetValue<Vector4>(newV), amount);
            }



            if(type == typeof(Color))
            {
                result = Color.LerpUnclamped((Color) GetValue<Color>(old), (Color) GetValue<Color>(newV), amount);
            }
            if(type == typeof(Color32))
            {
                result = Color32.LerpUnclamped((Color32) GetValue<Color32>(old), (Color32) GetValue<Color32>(newV), amount);
            }

            if(type == typeof(string))
            {
                string newS =((string)newV); 
                result = newS.Substring(0, (int) Mathf.Lerp(0, newS.Length, amount));
            }
            if(result == null)
            {
                // Debug.LogWarning("Type: " + type + " is NOT supported, will return " + newV);
                return false;
            }
            return true;
        }
        public static bool Add2Value(object val1, object val2, ref object result, float amount, Type t = null)
        {
            var type = t == null ? val1.GetType() : t; 
            if(type == typeof(int))
            {
                result = (int)val1 + (int) val2;return true;
            }
            if(type == typeof(float))
            {
                result = (float)val1 + (float) val2;return true;
            }
            if(type == typeof(double))
            {
                result = (double)val1 + (double) val2;return true;
            }

            if(type == typeof(Vector2))
            {
                result = (Vector2)val1 + (Vector2) val2;return true;
            }
            if(type == typeof(Vector3))
            {
                result = (Vector3)val1 + (Vector3) val2;return true;
            }
            if(type == typeof(Vector4))
            {
                result = (Vector4)val1 + (Vector4) val2;return true;
            }



            if(type == typeof(Color))
            {
                result = (Color)val1 + (Color) val2;
                return true;
            }
            if(type == typeof(Color32))
            {
                result = (Color32)((Color)val1 + (Color) val2);
                return true;
            }

            if(type == typeof(string))
            {
                result = (string)val1 + (string) val2;
                return true;
            }
            result = amount >=1 ? val2 : val1;

            // Debug.LogWarning("Type: " + type + " is NOT supported, will return " + val2);
            return false;
        }


        public static void UpdateProgressState(ref List<AmountData> statesAmount, int targetState, float amount, int nStates, int curState, bool isComplete)
        {
            if (statesAmount == null || statesAmount.Count != nStates)
            {
                statesAmount = new List<AmountData>();
                for (int i = 0; i < nStates; i++)
                {
                    statesAmount.Add(new AmountData()
                    {
                        amount = (i == curState) ? 1 : 0,
                        wasIgnore = i != curState && i != targetState
                    });
                }
            }
            // if (isComplete)
            // {
            //     statesAmount[targetState].amount = 1;
            //     for (int i = 0; i < nStates; i++)
            //     {
            //         statesAmount[i].amount = (targetState == i) ? 1 : 0;
            //     }
            //     return;
            // }



            var delta = amount - statesAmount[targetState].amount;
            int sign = (int)Mathf.Sign(delta);
            statesAmount[targetState].amount = amount;
            int divCount = 0;

            for (int i = 0; i < nStates; i++)
            {
                if (targetState == i) continue;

                // if state percent negative
                // if (sign > 0 && statesAmount[i].amount <= 0) continue;
                if (statesAmount[i].wasIgnore) continue;

                divCount++;
            }
            // Debug.Log(divCount + " "+ sign + "  " + delta);
            for (int i = 0; i < nStates; i++)
            {
                if (targetState == i) continue;
                // if state percent negative
                // if (sign > 0 && statesAmount[i].amount <= 0)
                // {
                //     statesAmount[i].amount = 0;
                //     continue;
                // }

                if (statesAmount[i].wasIgnore) continue;

                statesAmount[i].amount -= delta / divCount;
            }
        }
    
    }

    
}