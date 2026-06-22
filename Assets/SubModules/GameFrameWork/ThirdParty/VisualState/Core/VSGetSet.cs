using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

using Object = UnityEngine.Object;
using UnityEngine.UI;

namespace vietlabs.vs
{
    public class VSGetSet
    {
        internal object invokeTarget;
        internal Type dataType;

        public virtual object GetValue()
        {
            return null;
        }

        public virtual void SetValue(object data)
        {
        }

        // ----------------- STATTIC -------------------

        public const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public |
                                   BindingFlags.NonPublic;


        static public VSGetSet Create(object target, string property)
        {
            if (target == null)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning("target is null");
#endif
                return null;
            }

            if (string.IsNullOrEmpty(property))
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning("property is null");
#endif
                return null;
            }
            
            if (/*(target is GameObject || target is GUIText) &&*/ VSGetSet_GO.SPECIAL_PROPERTIES.Contains(property))
            {
                return new VSGetSet_GO(target, property);
            }
            //if (VSGetSet_GO.PROPERTIES_CHANGED_NAME_VSGO.ContainsKey(property))
            //{
            //    return new VSGetSet_GO(target, VSGetSet_GO.PROPERTIES_CHANGED_NAME_VSGO[property]);
            //}


            var targetT = target.GetType();
            var propertyInfo = targetT.GetProperty(property, FLAGS);
            if (propertyInfo != null)
            {
                return new VSGetSet_Property()
                {
                    invokeTarget = target,
                    property = propertyInfo,
                    dataType = propertyInfo.PropertyType
                };
            }


            if (VSGetSet_GO.PROPERTIES_CHANGED_NAME.ContainsKey(property))
            {

                //Some property difference between display name and real name
                property = VSGetSet_GO.PROPERTIES_CHANGED_NAME[property];
                propertyInfo = targetT.GetProperty(property, FLAGS);
                if (propertyInfo != null)
                {
                    return new VSGetSet_Property()
                    {
                        invokeTarget = target,
                        property = propertyInfo,
                        dataType = propertyInfo.PropertyType
                    };
                }
            }

            

            var fieldInfo = targetT.GetField(property, FLAGS);
            if (fieldInfo != null)
            {
                return new VSGetSet_Field()
                {
                    invokeTarget = target,
                    field = fieldInfo,
                    dataType = fieldInfo.FieldType
                };
            }

            if (property.StartsWith("m_"))
            {
                //IMPORTANT : WORKAROUND FOR UNITY'S BUILT-IN CLASSES
                property = property[2].ToString().ToLower() + property.Substring(3);
                propertyInfo = targetT.GetProperty(property, FLAGS);
                if (propertyInfo != null)
                {
                    return new VSGetSet_Property()
                    {
                        invokeTarget = target,
                        property = propertyInfo,
                        dataType = propertyInfo.PropertyType
                    };
                }
            }
            return null;
        }
    }

    internal class VSGetSet_GO : VSGetSet
    {
        const string IS_ACTIVE = "m_IsActive";
        const string M_COLOR_RGBA = "m_Color.rgba";
        public const string M_BIT = "m_Bits";
        internal static HashSet<string> SPECIAL_SETTER = new HashSet<string>()
        {
            M_BIT
        };
        internal static HashSet<string> SPECIAL_PROPERTIES = new HashSet<string>()
        {
            IS_ACTIVE,M_COLOR_RGBA
        };

        internal static Dictionary<string, string> PROPERTIES_CHANGED_NAME = new Dictionary<string, string>()
        {

            //camera
            { "field of view","fieldOfView"},
            {"m_BackGroundColor", "backgroundColor" },
            {"orthographic size", "orthographicSize" },
            {"near clip plane", "nearClipPlane" },
            {"far clip plane","farClipPlane" },
            {"m_NormalizedViewPortRect","rect" },
            {"m_OcclusionCulling","useOcclusionCulling" },
            {"m_HDR","allowHDR" },
            {"m_Bits","m_value" },

            //text
            {"m_FontData.m_FontStyle","fontStyle" },
            {"m_FontData.m_FontSize","fontSize" },
            {"m_FontData.m_LineSpacing","lineSpacing" },
            {"m_FontData.m_Alignment","alignment" },
            {"m_FontData.m_RichText","m_value" }

        };

        public string propertyName;

        public VSGetSet_GO(object target, string property)
        {
            invokeTarget = target;
            propertyName = property;
            RefreshDataType();
        }

        VSGetSet_GO RefreshDataType()
        {
            if (propertyName == IS_ACTIVE)
            {
                dataType = typeof(bool);
                return this;
            }
            else if (propertyName == M_COLOR_RGBA)
            {
                dataType = typeof(string);
                return this;
            }

            Debug.LogWarning("Property not implemented : " + propertyName);

            return null;
        }

        public override object GetValue()
        {

            if (invokeTarget == null)
            {
                Debug.LogWarning("Something wrong, invoke target or property is null");
                return null;
            }

            if (propertyName == IS_ACTIVE)
            {
                return (invokeTarget as GameObject).activeSelf;
            }

            if (propertyName == M_COLOR_RGBA && invokeTarget is Text)
            {
                return (invokeTarget as  Text).color;
            }
            Debug.LogWarning("Property not implement :: " + propertyName + " target: "+ invokeTarget);
            return null;
        }

        public override void SetValue(object data)
        {
            if (invokeTarget == null) return;
            var obj = invokeTarget as Object;

            if (invokeTarget is Object && obj == null)
            {
                Debug.LogWarning(this + " InvokeTarget should not be null !");
                return;
            }

            if (propertyName == IS_ACTIVE)
            {
                (invokeTarget as GameObject).SetActive((bool) data);
                return;
            }
            if (propertyName == M_COLOR_RGBA && invokeTarget is Text)
            {
                Color32 col;
                VisualState_Unity.TryParseHtmlString((string) data, out col);
                (invokeTarget as Text).color = col;
                return;
            }
            Debug.LogWarning("Property not implement :: " + propertyName + " target: " + invokeTarget);
        }
    }

    internal class VSGetSet_Field : VSGetSet
    {
        internal FieldInfo field;

        public override object GetValue()
        {
            if (invokeTarget == null) return null;

            var obj = invokeTarget as Object;

            if (invokeTarget is Object && obj == null)
            {
                //Because Unity overloads == null operator
                //Debug.LogWarning("Something wrong, invoke target or property is null");
                return null;
            }

            return field.GetValue(invokeTarget);
        }

        public override void SetValue(object data)
        {
            if (invokeTarget == null) return;

            var obj = invokeTarget as Object;
            if (invokeTarget is Object && obj == null)
            {
                //Because Unity overloads == null operator
                //Debug.LogWarning("Something wrong, invoke target or property is null");
                return;
            }

            field.SetValue(invokeTarget, data);
        }
    }

    internal class VSGetSet_Property : VSGetSet
    {
        internal PropertyInfo property;

        public override object GetValue()
        {
            if (invokeTarget == null) return null;

            var obj = invokeTarget as Object;
            if (invokeTarget is Object && obj == null)
            {
                //Because Unity overloads == null operator
                //Debug.LogWarning("Something wrong, invoke target or property is null");
                return null;
            }

            return property.GetValue(invokeTarget, null);
        }

        public override void SetValue(object data)
        {
            if (invokeTarget == null) return;

            var obj = invokeTarget as Object;
            if (invokeTarget is Object && obj == null)
            {
                //Because Unity overloads == null operator
                //Debug.LogWarning("Something wrong, invoke target or property is null");
                return;
            }

            property.SetValue(invokeTarget, data, null);
        }
    }
}