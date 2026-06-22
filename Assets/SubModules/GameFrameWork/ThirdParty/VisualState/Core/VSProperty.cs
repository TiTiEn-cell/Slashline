using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace vietlabs.vs
{
    public enum VSPropertyType
    {
        Normal,
        Ignore,
        Lock
    }

    [Serializable]
    public class VSPropertyBase
    {
#if UNITY_EDITOR
        public string info;
        internal void UpdateInfo()
        {
            info = (target != null) ? string.Format("{0}.{1}", target.name, property) : ("." + property);
        }
#endif

        public string property;

        public AnimOverrideData overrideData = new AnimOverrideData();
        public float amount;
        public Object target;
        public VSPropertyType type;

        internal VSGetSet field;
        public virtual void SetValue(object value) { }
        public virtual void SetState(int state) { }
        public virtual void Resize(int size) { }
        public virtual void AddState(int stateClone) { }
        public virtual void RemoveState(int index) { }
        public virtual bool IsEmptyState() { return false; }
        public virtual bool IsStateEqualDefault(int state) { return false; }

        public virtual object GetCurrentValue()
        {
            return null;
        }
        public virtual object GetStateValue(int state)
        {
            return null;
        }
        public virtual object GetStateValuePercent(int state, float percent)
        {
             object val = null;
            VisualState_Unity.GetPercentValue(GetStateValue(state),percent, out val);
            return val;
        }
        public virtual void SetToState(int oldState, int newState, float percent)
        {

        }
        public virtual void SetValueAmount(int targetState, float amount, int nStates, int currentState, List<AmountData> defaultAmount, bool isComplete)
        {

        }

        public bool isNormal { get { return type == VSPropertyType.Normal; } }
        public bool isIgnored { get { return type == VSPropertyType.Ignore; } }
        public bool isLocked { get { return type == VSPropertyType.Lock; } }

#if UNITY_EDITOR
        public bool isExpand = true;
        public bool cacheIsEqualDefault;

        public virtual void RevertDefaultAndIgnore() { SetState(-1); type = VSPropertyType.Ignore; }
        public virtual void SetStateValueToDefault(int state) { }
        public virtual void WriteDefault(int nStates, PropertyModification prop) { }
        public virtual void WriteChange(int index, PropertyModification prop) { }
        public virtual void OnGUI(int nStates, int state) { }

        public virtual void OnWindowGUI(int nStates, int state, VisualState parent)
        {
            overrideData.type = (OverrideType)EditorGUILayout.EnumPopup("override type", overrideData.type);
            switch (overrideData.type)
            {
                case OverrideType.Override:
                    {
                        if (overrideData.curve == null)
                        {
                            overrideData.curve = new AnimationCurve();
                        }
                        overrideData.curve = EditorGUILayout.CurveField(overrideData.curve);

                    }
                    break;
                case OverrideType.OverrideWithDelayTime:
                    {
                        overrideData.DelayTime = EditorGUILayout.Slider
                        (
                            "delayTime",
                            overrideData.DelayTime,
                            0,
                            parent.transitionData.time
                            );

                    }
                    break;
            }
        }


#endif
    }

    [Serializable]
    public class VSValue : VSPropertyBase
    {

        public bool isReference;

        public override void SetState(int state)
        {
#if UNITY_EDITOR
            if (Application.isPlaying && isIgnored) return;
#else
			if (isIgnored) return; // Do not change value if being ignored
#endif

            if (field == null)
            {
                field = VSGetSet.Create(target, property);
                if (field == null)
                {
                    type = VSPropertyType.Ignore;
                    //Debug.LogWarning("Property not found (might only exist in Editor) : " + property);
                    return;
                }
            }

            if (isReference)
            {
                SetRef(state);
            }
            else
            {
                SetValue(state);
            }
            VisualState_Unity.CheckUpdateDirtyState(target);

            //if (target is UIWidget)
            //{
            //	(target as UIWidget).MarkAsChanged();
            //}

            //if (target is UIButton) 
            //{
            //	(target as UIButton).RefreshSpriteCache ();
            //}
        }
        public override void SetToState(int oldState, int newState, float amount)
        {
            if (field == null)
            {
                field = VSGetSet.Create(target, property);
                if (field == null)
                {
                    type = VSPropertyType.Ignore;
                    //Debug.LogWarning("Property not found (might only exist in Editor) : " + property);
                    return;
                }
            }

            if (isReference)
            {
                currentRef = defaultRef;

                if (currentRef != null)
                {
                    object val1;
                    object val2;
                    object result = GetStateValue(newState);

                    if (VisualState_Unity.GetPercentValue(GetStateValue(oldState), 1 - amount, out val1))
                    {
                        if (VisualState_Unity.GetPercentValue(result, amount, out val2))
                        {
                            VisualState_Unity.Add2Value(val1, val2, ref result, amount);
                        }
                    }
                    field.SetValue(result);
                }
                else
                {
                    field.SetValue(null);
                }
            }
            else
            {
                currentValue = defaultValue;
                var dataType = field.dataType;
                if (dataType.IsEnum)
                {
                    var intValue = int.Parse(currentValue);
                    field.SetValue(Enum.ToObject(dataType, intValue));
                    return;
                }
                object val1;
                object val2;
                object result = GetStateValue(newState);

                if (VisualState_Unity.GetPercentValue(GetStateValue(oldState), 1 - amount, out val1))
                {
                    if (VisualState_Unity.GetPercentValue(result, amount, out val2))
                    {
                        VisualState_Unity.Add2Value(val1, val2, ref result, amount);
                    }
                }
                field.SetValue(result);
            }
        }
        public override void SetValue(object value)
        {
            if (isReference)
            {
                if (currentRef != null)
                {
                    field.SetValue(currentRef);
                }
                else
                {
                    field.SetValue(null);
                }
            }
            else
            {
                var dataType = field.dataType;
                if (dataType == typeof(string))
                {
                    field.SetValue(value);
                    VisualState_Unity.CheckUpdateDirtyState(target);
                    return;
                }

                if (dataType.IsEnum)
                {
                    var intValue = (int)(value);
                    field.SetValue(Enum.ToObject(dataType, intValue));
                    VisualState_Unity.CheckUpdateDirtyState(target);
                    return;
                }

                if (VSGetSet_GO.SPECIAL_SETTER.Contains(property))
                {
                    specialSetter();
                }
                else if (VSGetSet_GO.SPECIAL_PROPERTIES.Contains(property))
                {
                    field.SetValue(value);
                }
                else
                {
                    var floatValue = (float)(value);
                    field.SetValue(Convert.ChangeType(floatValue, field.dataType));
                }
            }
            VisualState_Unity.CheckUpdateDirtyState(target);

        }




        public override void Resize(int size)
        {
            if (isReference)
            {
                stateRefs = VSUtils.Resize<Object>(stateRefs, size, defaultRef);
            }
            else
            {
                stateValues = VSUtils.Resize<string>(stateValues, size, defaultValue);
            }
        }
        public override void AddState(int stateClone)
        {
            if (isReference)
            {
                Object currentRef = defaultRef;
                if (stateClone >= 0 && stateClone < stateRefs.Count)
                {
                    currentRef = stateRefs[stateClone];
                }
                stateRefs.Add(currentRef);
            }
            else
            {
                string currentValue = defaultValue;
                if (stateClone >= 0 && stateClone < stateValues.Count)
                {
                    currentValue = stateValues[stateClone];
                }
                stateValues.Add(currentValue);
            }
        }
        public override void RemoveState(int index)
        {
            if (isReference)
            {
                stateRefs.RemoveAt(index);
            }
            else
            {
                stateValues.RemoveAt(index);
            }
        }
        public override bool IsEmptyState()
        {
            if (isReference)
            {
                return stateRefs == null || stateRefs.Count <= 0;
            }
            else
            {
                return stateValues == null || stateValues.Count <= 0;
            }
        }
        public override bool IsStateEqualDefault(int state)
        {
            if (isReference)
            {
                if (defaultRef != null)
                {
                    return defaultRef.Equals(stateRefs[state]);
                }
                return stateRefs[state] == null;
            }
            else
            {
                if (!string.IsNullOrEmpty(defaultValue))
                {
                    return defaultValue.Equals(stateValues[state]);
                }
                return string.IsNullOrEmpty(stateValues[state]);

            }
        }

#if UNITY_EDITOR
        public override void SetStateValueToDefault(int state)
        {
            if (isReference)
            {
                stateRefs[state] = defaultRef;
            }
            else
            {
                stateValues[state] = defaultValue;
            }
        }
#endif
        public override object GetStateValue(int state)
        {


            if (isReference) return stateRefs[state];
            if (field == null) field = VSGetSet.Create(target, property);


            currentValue = defaultValue;

#if UNITY_EDITOR
            if (!isIgnored) // In EditMode : isIgnore = set to defaultValue
#endif
            {
                if (state >= 0 && state < stateValues.Count)
                {
                    currentValue = stateValues[state];
                }
            }


            var dataType = field.dataType;
            if (dataType == typeof(string)) return stateValues[state];

            if (dataType.IsEnum)
            {
                var intValue = int.Parse(currentValue);
                return Enum.ToObject(dataType, intValue);
            }

            var floatValue = float.Parse(currentValue.ToString());
            return Convert.ChangeType(floatValue, field.dataType);

        }

        // --------------- SIMPLE VALUE SUPPORT -----------------

        public string defaultValue;
        public string currentValue;
        public List<string> stateValues;

        public override void SetValueAmount(int targetState, float amount, int nStates, int currentState, List<AmountData> defaultAmount, bool isComplete)
        {
            var Override = overrideData.type == OverrideType.Override;

            if (Override)
            {
                amount = overrideData.curve.Evaluate(amount);
                VisualState_Unity.UpdateProgressState
                (
                    ref overrideData.nStateAmount,
                    targetState,
                    amount,
                    nStates,
                    currentState,
                    isComplete
                );
            }

            List<AmountData> nStateAmount = Override ? overrideData.nStateAmount : defaultAmount;

            object result = null;
            for (int k = 0; k < nStates; k++)
            {
                if (k == 0)
                {
                    result = GetStateValuePercent(k, nStateAmount[k].amount);
                    continue;
                }
                object val2 = GetStateValuePercent(k, nStateAmount[k].amount);
                VisualState_Unity.Add2Value(result, val2, ref result, nStateAmount[k].amount);
            }
            SetValue(result);

        }

        void SetValue(int state)
        {
            currentValue = defaultValue;

#if UNITY_EDITOR
            if (!isIgnored) // In EditMode : isIgnore = set to defaultValue
#endif
            {
                if (state >= 0 && state < stateValues.Count)
                {
                    currentValue = stateValues[state];
                }
            }
            var dataType = field.dataType;
            if (dataType == typeof(string))
            {
                field.SetValue(currentValue);
                return;
            }

            if (dataType.IsEnum)
            {
                var intValue = int.Parse(currentValue);
                field.SetValue(Enum.ToObject(dataType, intValue));
                return;
            }
            {
                var floatValue = float.Parse(currentValue);
                field.SetValue(Convert.ChangeType(floatValue, field.dataType));
            }


        }

        private void specialSetter()
        {
            if (property == VSGetSet_GO.M_BIT)
            {
                long val = long.Parse(currentValue);
                if (val >= 2147483648)
                {
                    val = val - 4294967296;
                }
                field.SetValue(Convert.ChangeType(val, field.dataType));
                return;
            }
            Debug.LogWarning("Property not implement :: " + property + " target: " + field.invokeTarget);
        }
        // --------------- REFERENCE SUPPORT -----------------

        public Object defaultRef;
        public Object currentRef;
        public List<Object> stateRefs;

        void SetRef(int state)
        {
            currentRef = defaultRef;

#if UNITY_EDITOR
            if (!isIgnored) // In EditMode : isIgnore = set to defaultValue
#endif
            {
                if (state >= 0 && state < stateRefs.Count)
                {
                    currentRef = stateRefs[state];
                }
            }

            if (currentRef != null)
            {
                field.SetValue(currentRef);
            }
            else
            {
                field.SetValue(null);
            }


        }
        public override object GetCurrentValue()
        {
            if (field == null) field = VSGetSet.Create(target, property);
            var v = field.GetValue();
            return v;
        }
        // --------------- EDITOR -----------------

#if UNITY_EDITOR
        public override void WriteDefault(int nStates, PropertyModification prop)
        {
            // IMPORTANT NOTE : DO NOT USE prop.propertyPath (this instance might be a field of struct and not containing current property)
            if (field == null)
            {
                isReference = false;
            }
            else
            {
                isReference = typeof(Object).IsAssignableFrom(field.dataType);
            }

            if (isReference)
            {
                defaultRef = prop.objectReference;
            }
            else
            {
                defaultValue = prop.value;
            }
            Resize(nStates);
        }

        public override void WriteChange(int index, PropertyModification prop)
        {
            if (field == null) field = VSGetSet.Create(prop.target, prop.propertyPath);

            var n = isReference ? stateRefs.Count : stateValues.Count;
            if (n <= index)
            {
                Debug.LogWarning("Something might be wrong : setting index " + index + " of " + n);
                Resize(index + 1);
            }

            if (isLocked || isIgnored)
            {
                Debug.LogWarning("Locked, do not change value! " + prop);

                #if UNITY_EDITOR
                UpdateInfo();
                #endif
                return;
            }

            if (isReference)
            {
                stateRefs[index] = prop.objectReference;
            }
            else
            {
                stateValues[index] = prop.value;
            }

            #if UNITY_EDITOR
            UpdateInfo();
            #endif
        }

        public override void OnGUI(int nStates, int state)
        {
            var c = GUI.backgroundColor;

#if UNITY_EDITOR
            var nCount = isReference ? stateRefs.Count : stateValues.Count;
            if (nCount < nStates)
            {
                Resize(nStates);
            }
#endif
            GUILayout.BeginVertical();
            {
                for (var i = 0; i < nStates; i++)
                {
                    if (i == state) GUI.backgroundColor = VisualState.ActiveColor;
                    if (isReference)
                    {
                        stateRefs[i] = EditorGUILayout.ObjectField(stateRefs[i], typeof(Object), true);
                    }
                    else
                    {
                        stateValues[i] = EditorGUILayout.TextField(stateValues[i]);
                    }

                    if (i == state) GUI.backgroundColor = c;
                }
            }
            GUILayout.EndVertical();
        }
        public override void OnWindowGUI(int nStates, int state, VisualState parent)
        {
            base.OnWindowGUI(nStates, state, parent);
            var c = GUI.backgroundColor;
            var nCount = isReference ? stateRefs.Count : stateValues.Count;
            if (nCount < nStates)
            {
                Resize(nStates);
            }
            GUI.backgroundColor = VisualState.ActiveColor;
            if (isReference)
            {
                stateRefs[state] = EditorGUILayout.ObjectField(stateRefs[state], typeof(Object), true);
            }
            else
            {
                stateValues[state] = EditorGUILayout.TextField(stateValues[state]);
            }

            GUI.backgroundColor = c;
        }
#endif
    }

    [Serializable]
    public class VSStruct : VSPropertyBase
    {
        public List<VSValue> children;
        AnimationCurve curve;
        float curveTime;
        object beginValue, targetValue;
        float countTime;

        public object defaultValue;
        public override void SetState(int state)
        {
            if (field == null) field = VSGetSet.Create(target, property);
            if (field == null)
            {
                #if UNITY_EDITOR
                Debug.LogWarning("[Editor] Property <" + property + "> not found on " + target);
                #endif
                return;
            }

            field.SetValue(GetStateValue(state));

            // countTime = 0;
            // curve = transitionData.curve;
            // curveTime = curve.keys[curve.keys.Length -1].time;
            // beginValue = GetCurrentValue();
            // targetValue = GetStateValue(state);

            // UnityEngine.Debug.Log("VS:: beginV " + beginValue + " targetV " + targetValue + "  curveTime " + curveTime);
            // if(Application.isPlaying) 
            // {
            //     VisualStateHelper.Instance.OnUpdate -= Update;
            //     VisualStateHelper.Instance.OnUpdate += Update;
            // }
            // else
            // {
            //     EditorApplication.update -= Update;
            //     EditorApplication.update += Update;
            // }

        }
        public override void SetValueAmount(int targetState, float amount, int nStates, int currentState, List<AmountData> defaultAmount, bool isComplete)
        {
            var Override = overrideData.type == OverrideType.Override;

            if (Override)
            {
                amount = overrideData.curve.Evaluate(amount);
                VisualState_Unity.UpdateProgressState
                (
                    ref overrideData.nStateAmount,
                    targetState,
                    amount,
                    nStates,
                    currentState,
                    isComplete
                );
            }

            List<AmountData> nStateAmount = Override ? overrideData.nStateAmount : defaultAmount;

            object result = null;
            for (int k = 0; k < nStates; k++)
            {
                if (k == 0)
                {
                    result = GetStateValuePercent(k, nStateAmount[k].amount);
                    continue;
                }
                object val2 = GetStateValuePercent(k, nStateAmount[k].amount);
                VisualState_Unity.Add2Value(result, val2, ref result, nStateAmount[k].amount);
            }
            SetValue(result);

        }
        public override void SetValue(object value)
        {
            if (field == null) field = VSGetSet.Create(target, property);
            var v = field.GetValue();
            for (var i = 0; i < children.Count; i++)
            {

                var c = children[i];

                if (c.field == null) c.field = VSGetSet.Create(v, c.property);
                c.field.invokeTarget = v;
                //children[i].SetValue(value); 
                // children[i].SetState(state);
            }
            field.SetValue(value);
            VisualState_Unity.CheckUpdateDirtyState(target);
        }
        public override void SetToState(int oldState, int newState, float amount)
        {
            if (field == null) field = VSGetSet.Create(target, property);
            var v = field.GetValue();
            if (defaultValue == null) defaultValue = v;
            for (var i = 0; i < children.Count; i++)
            {

                var c = children[i];

                if (c.field == null) c.field = VSGetSet.Create(v, c.property);
                c.SetToState(oldState, newState, amount);
            }
            object val1;
            object val2;
            object result = GetStateValue(newState);

            if (VisualState_Unity.GetPercentValue(GetStateValue(oldState), 1 - amount, out val1))
            {
                if (VisualState_Unity.GetPercentValue(GetStateValue(newState), amount, out val2))
                {
                    VisualState_Unity.Add2Value(val1, val2, ref result, amount);
                }
            }
            field.SetValue(result);
            VisualState_Unity.CheckUpdateDirtyState(target);
        }


        public override void Resize(int size)
        {
            if (children == null || children.Count == 0) return;
            for (var i = 0; i < children.Count; i++)
            {
                children[i].Resize(size);
            }
        }
        public override void AddState(int stateClone)
        {
            if (children == null || children.Count == 0) return;
            for (var i = 0; i < children.Count; i++)
            {
                children[i].AddState(stateClone);
            }
        }
        public override void RemoveState(int index)
        {
            if (children == null || children.Count == 0) return;
            for (var i = 0; i < children.Count; i++)
            {
                children[i].RemoveState(index);
            }
        }
        public override bool IsEmptyState()
        {
            if (children == null || children.Count == 0) return true;
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i].IsEmptyState() == false)
                {
                    return false;
                }
            }
            return true;
        }
        public override bool IsStateEqualDefault(int state)
        {
            if (children == null || children.Count == 0) return true;
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i].IsStateEqualDefault(state) == false)
                {
                    return false;
                }
            }
            return true;
        }

#if UNITY_EDITOR

        public override void SetStateValueToDefault(int state)
        {
            if (children == null || children.Count == 0) return;
            for (var i = 0; i < children.Count; i++)
            {
                children[i].SetStateValueToDefault(state);
            }
        }

#endif

        // ------------------------- EDITOR -----------------------

        public override object GetStateValue(int state)
        {
            if (field == null) field = VSGetSet.Create(target, property);

            // Fix null reference reported in Jan3
            // https://developer.cloud.unity3d.com/diagnostics/orgs/amanotespteltd/projects/f1e18c71-5fa3-4d32-841a-034a99577514/crashes/4512478ed85c3695ebb29144f715bab2/
            if (field == null) return null;

            var v = field.GetValue();
            for (var i = 0; i < children.Count; i++)
            {
                var c = children[i];

                // Fix null reference reported in Jan3
                // https://developer.cloud.unity3d.com/diagnostics/orgs/amanotespteltd/projects/f1e18c71-5fa3-4d32-841a-034a99577514/crashes/4512478ed85c3695ebb29144f715bab2/  
                if (c == null) continue;

                if (c.field == null) c.field = VSGetSet.Create(v, c.property);

                // Fix null reference reported in Jan3
                // https://developer.cloud.unity3d.com/diagnostics/orgs/amanotespteltd/projects/f1e18c71-5fa3-4d32-841a-034a99577514/crashes/4512478ed85c3695ebb29144f715bab2/
                if (c.field == null) continue;

                c.field.invokeTarget = v;
                c.SetState(state);
            }
            return v;
        }
        public override object GetCurrentValue()
        {
            if (field == null) field = VSGetSet.Create(target, property);
            var v = field.GetValue();
            return v;
        }

#if UNITY_EDITOR

        public override void OnGUI(int nStates, int state)
        {
            var so = new SerializedObject(target);
            so.Update();
            GUILayout.BeginVertical();
            {

                for (var i = 0; i < nStates; i++)
                {
                    if (i == state)
                    {
                        try
                        {
                            EditorGUILayout.PropertyField(so.FindProperty(property), GUIContent.none);
                        }
                        catch (Exception) { }

                    }
                    else
                    {
                        GUILayout.TextField(GetStateValue(i).ToString());
                    }
                }
            }
            GUILayout.EndVertical();
        }
        public override void OnWindowGUI(int nStates, int state, VisualState parent)
        {
            //if (children == null || children.Count == 0) return;
            //for (var i = 0; i < children.Count; i++)
            //{
            //    children[i].OnWindowGUI(nStates, state);
            //}
            base.OnWindowGUI(nStates, state, parent);
            var so = new SerializedObject(target);
            so.Update();
            try
            {
                EditorGUI.BeginChangeCheck();
                SerializedProperty pro = so.FindProperty(property);
                EditorGUILayout.PropertyField(pro, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                }
            }
            catch (Exception) { }
            so.ApplyModifiedProperties();
        }
        private void ApplyValueChanged(int state)
        {
            if (children == null || children.Count == 0) return;
            for (var i = 0; i < children.Count; i++)
            {
                children[i].SetStateValueToDefault(state);
            }
        }
        VSValue GetChild(int nStates, PropertyModification prop)
        {
            if (children == null) children = new List<VSValue>();

            var childProp = prop.propertyPath.Split('.')[1];
            var child = children.FirstOrDefault(item => item.property == childProp);
            if (child != null) return child;

            if (nStates == 0)
            {
                Debug.LogWarning("Something wrong ! nState should not be zero! " + prop.propertyPath);
                return null;
            }

            child = new VSValue()
            {
                property = childProp,
                field = VSGetSet.Create(field.GetValue(), childProp)
            };

            child.WriteDefault(nStates, prop);
            children.Add(child);

            return child;
        }

        public override void WriteDefault(int nStates, PropertyModification prop)
        {
            defaultValue = prop.objectReference;
            GetChild(nStates, prop);
        }

        public override void WriteChange(int index, PropertyModification prop)
        {
            if (isLocked) return;
            GetChild(0, prop).WriteChange(index, prop);
            UpdateInfo();
        }

#endif
    }

#if UNITY_EDITOR
    public class VSTarget
    {
        public bool isExpand = true;
        public Object target;
        public List<VSPropertyBase> properties;
        private VisualState cacheVS;
        public void OnWindowGUI(VisualState vs)
        {
            cacheVS = vs;
            bool isAllEqual = true;
            for (var i = 0; i < properties.Count; i++)
            {
                var p = properties[i];
                if (p.isIgnored && !VisualState.devMode) continue;
                p.cacheIsEqualDefault = p.IsStateEqualDefault(vs.state);
                if (!p.cacheIsEqualDefault)
                {
                    isAllEqual = false;
                }
            }
            if (isAllEqual && VisualState.isHideDefault)
            {
                return;
            }
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(VisualState.PaddingLeftElement * 2);
                bool cacheEnable = GUI.enabled;
                GUI.enabled = false;
                EditorGUILayout.ObjectField(target, target.GetType(), false);
                GUI.enabled = cacheEnable;
            }
            GUILayout.EndHorizontal();

            //if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect.x += 2 + VisualState.PaddingLeftElement;
                isExpand = EditorGUI.Foldout(lastRect, isExpand, "");

            }

            if (isExpand)
            {
                for (var i = 0; i < properties.Count; i++)
                {
                    var p = properties[i];
                    if (p.isIgnored && !VisualState.devMode) continue;

                    if (p.cacheIsEqualDefault && VisualState.isHideDefault && !VisualState.devMode)
                    {
                        //Debug.Log("property " + p.property + " EqualDefault");
                        continue;
                    }


                    GUILayout.Space(8f);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(VisualState.PaddingLeftElement * 2);
                        GUILayout.Label(p.property, GUILayout.Width(130f)); //m_sprite, m_Size,...

                        var e = (VSPropertyType)EditorGUILayout.EnumPopup(p.type);
                        //int curIndex = p.type == VSPropertyType.Normal ? 0 : 1;
                        //int selected = EditorGUILayout.Popup(curIndex, VisualState.EnumProperty);
                        //var e = selected == 0 ? VSPropertyType.Normal: VSPropertyType.Lock;
                        if (e != p.type)
                        {
                            if (e == VSPropertyType.Ignore)
                            {
                                p.RevertDefaultAndIgnore();
                            }
                            else
                            {
                                p.type = e;
                            }
                        }
                        GUIStyle st = new GUIStyle();
                        st.alignment = TextAnchor.LowerRight;
                        if (GUILayout.Button(EditorGUIUtility.IconContent("_Popup"), st, GUILayout.Width(VisualState.ButtonRemoveWidth), GUILayout.Height(VisualState.SingleHeight)))
                        {
                            GenericMenu menu = new GenericMenu();

                            // forward slashes nest menu items under submenus
                            menu.AddItem(new GUIContent("Reset"), false, OnReset, i);
                            menu.AddSeparator("");
                            menu.AddItem(new GUIContent("Remove"), false, OnRemove, i);

                            menu.AddSeparator("");
                            menu.AddItem
                            (
                                new GUIContent("Dont Override"),
                                p.overrideData.type == OverrideType.None,
                                DontOverride, i
                            );
                            menu.AddItem
                            (
                                new GUIContent("Override Curve"),
                                p.overrideData.type == OverrideType.Override,
                                OverrideCurve, i
                            );
                            menu.AddItem(
                                new GUIContent("Override With Delay Time"),
                                p.overrideData.type == OverrideType.OverrideWithDelayTime,
                                OverrideWithTime, i);
                            menu.ShowAsContext();

                            return;
                        }
                    }

                    GUILayout.EndHorizontal();

                    if (p.isIgnored) continue;


                    GUILayout.BeginHorizontal();
                    {
                        if (p.isLocked) GUI.enabled = false;
                        GUILayout.Space(VisualState.PaddingLeftElement * 2);
                        p.OnWindowGUI(vs.nStates, vs.state, vs);
                        if (p.isLocked) GUI.enabled = true;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(5);
            VisualState.HorizontalLine(Color.grey);
        }
        void OnReset(object index)
        {
            int i = (int)index;
            properties[i].SetState(-1);
            properties[i].SetStateValueToDefault(cacheVS.state);
            cacheVS.setDirty();
        }
        void OnRemove(object index)
        {
            if (EditorUtility.DisplayDialog("Remove Property?",
                "Are you sure you want to remove this property?", "Yes", "Cancel"))
            {
                int i = (int)index;
                cacheVS.removeProperty(i, properties[i] is VSValue ? true : false);
            }

        }
        void DontOverride(object index)
        {

            {
                int i = (int)index;
                cacheVS.setOverrideProperty
                (
                    i,
                    properties[i] is VSValue ? true : false,
                    OverrideType.None
                );
            }

        }
        void OverrideCurve(object index)
        {

            {
                int i = (int)index;
                cacheVS.setOverrideProperty
                (
                    i,
                    properties[i] is VSValue ? true : false,
                    OverrideType.Override
                );
            }

        }
        void OverrideWithTime(object index)
        {
            {
                int i = (int)index;
                cacheVS.setOverrideProperty
                (
                    i,
                    properties[i] is VSValue ? true : false,
                    OverrideType.OverrideWithDelayTime
                );
            }

        }
    }

#endif

    public class VSUtils
    {
        static public List<T> Resize<T>(List<T> list, int size, T defaultValue)
        {
            if (list == null) list = new List<T>();
            while (list.Count < size)
            {
                list.Add(defaultValue);
            }

            if (list.Count > size) list.RemoveRange(size, list.Count - size);
            return list;
        }

        static public void ResizeProperty<T>(List<T> list, int size) where T : VSPropertyBase
        {
            for (var i = 0; i < list.Count; i++)
            {
                list[i].Resize(size);
            }
        }

        static public void SetState<T>(List<T> list, int state) where T : VSPropertyBase
        {
            for (var i = 0; i < list.Count; i++)
            {
                //if (list[i].isIgnored) continue;
                list[i].SetState(state);
            }
        }

        static public void SetState2<T>(List<T> list, float state) where T : VSPropertyBase
        {
            int min = Mathf.FloorToInt(state);
            int max = Mathf.CeilToInt(state);

            for (var i = 0; i < list.Count; i++)
            {
                list[i].SetToState(min, max, state-min);
            }
        }

        public static void AddState<T>(List<T> list, int stateClone) where T : VSPropertyBase
        {
            for (var i = 0; i < list.Count; i++)
            {
                //if (list[i].isIgnored) continue;
                list[i].AddState(stateClone);
            }
        }
        public static void RemoveState<T>(List<T> list, int index) where T : VSPropertyBase
        {
            for (var i = 0; i < list.Count; i++)
            {
                //if (list[i].isIgnored) continue;
                list[i].RemoveState(index);
            }
        }
    }
}
