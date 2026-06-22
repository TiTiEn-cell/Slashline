using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace vietlabs.vs
{
    public interface IVisualStateListener
    {
        void OnVSChange();
    }
    [Serializable]
    public class AmountData{
        public float amount;
        public bool wasIgnore;
    }
    [Serializable]
    public class TransitionData{
        public AnimationCurve curve = new AnimationCurve();
        public float time = 0f;

        public List<AmountData> nStateAmount;
    }
    public enum OverrideType
    {
        None,
        Override,
        OverrideWithDelayTime
    }
    [Serializable]
    public class AnimOverrideData
    {
        public OverrideType type;
        public AnimationCurve curve;
        public float DelayTime;
        public List<AmountData> nStateAmount;
    }
    [Serializable]
    public class VisualState : MonoBehaviour
    {
        public TransitionData transitionData = new TransitionData();

        public List<string> stateNames;
        public List<VSValue> listValue;
        public List<VSStruct> listStruct;
        public List<MonoBehaviour> listeners;

         [SerializeField]
        private StateChangeEvent OnBeforeChange = new StateChangeEvent();
        [SerializeField]
        private StateChangeEvent OnAfterChange = new StateChangeEvent();
        [Serializable]
        public class StateChangeEvent : UnityEvent
        {
        }

        [SerializeField] private int _state;
        private int _oldState;
        public int state
        {
            get { return _state; }
        }

        public int targetState;

        
        void Start()
        {
            // Debug.Log(nStateAmount.Count);
        }
        public void RegistAfterchange(UnityAction onComplete)
        {
            OnAfterChange.AddListener(onComplete);
        }
        public void UnregistAfterchange(UnityAction onComplete)
        {
            OnAfterChange.RemoveListener(onComplete);
        }
        private bool haveTween{
            get{
                return transitionData.time > 0 && transitionData.curve != null && transitionData.curve.keys.Length > 0;

            }
        }
        public void SetState(int stateIndex, bool force = false)
        {
            

             if(haveTween)
             {
                 if(isTweening) return;
                    isTweening = true;
             }
            
            if (!force && _state == stateIndex) return;
            countTime = 0;
            if (stateIndex < 0)
            {
                stateIndex = 0;
            }
            if (stateIndex >= stateNames.Count - 1)
            {
                stateIndex = stateNames.Count - 1;
            }
            _oldState = _state;
            _state = stateIndex;
            
            sign = 1;

            if(transitionData != null && transitionData.nStateAmount!= null) transitionData.nStateAmount.Clear();
            targetState = stateIndex;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                refreshCount = 1;
                EditorApplication.update -= RefreshState;
                EditorApplication.update += RefreshState;
            }
            else
#endif
            {
                RefreshState();
            }
        }
        public void SetState(string stateName, bool force = false)
        {
            int index = stateNames.IndexOf(stateName);
            if (index >= 0)
            {
                SetState(index,force);
            }
        }

        public void SetStateWithoutTransition(int state)
        {
            state = Mathf.Clamp(state, 0, nStates);

            _oldState = state;
            _state = state;

            isTweening = false;
            targetState = state;
            if (transitionData != null && transitionData.nStateAmount != null) transitionData.nStateAmount.Clear();

            VSUtils.SetState(listValue, _state);
            VSUtils.SetState(listStruct, _state);

            if (listeners != null)
            {
                for (var i = 0; i < listeners.Count; i++)
                {
                    var m = listeners[i];
                    if (m is IVisualStateListener)
                    {
                        (m as IVisualStateListener).OnVSChange();
                        continue;
                    }
                }
            }
        
            OnAfterChange.Invoke();
        }


        public float state2 = 0;

        public void SetState2(float value)
        {
            this.state2 = Mathf.Max(0, Mathf.Min(nStates-1, value));

            VSUtils.SetState2(listValue, value);
            VSUtils.SetState2(listStruct, value);

            if (listeners != null)
            {
                for (var i = 0; i < listeners.Count; i++)
                {
                    var m = listeners[i];
                    if (m is IVisualStateListener)
                    {
                        (m as IVisualStateListener).OnVSChange();
                        continue;
                    }
                }
            }
        
            OnAfterChange.Invoke();
        }
        
        public int nStates { get { return stateNames.Count; } }
        public string stateName { get { return (_state < 0 || _state >= stateNames.Count) ? string.Empty : stateNames[_state]; } }

        public void Create2State()
        {
            {
               
                stateNames = new List<string>()
                    {
                        "Normal", "Highlight"
                    };
                Resize(2);
                SetState(1);
            }
        }
        public void Resize(int size)
        {
            if (Application.isPlaying) return;
            if (listStruct == null)
            {
                listStruct = new List<VSStruct>();
            }
            if (listValue == null)
            {
                listValue = new List<VSValue>();
            }
            VSUtils.Resize(stateNames, size, string.Empty);
            VSUtils.ResizeProperty(listValue, size);
            VSUtils.ResizeProperty(listStruct, size);

             
             #if UNITY_EDITOR
             EditorUtility.SetDirty(this);
             #endif
        }
        public void AddState(int cloneIndex)
        {
            if (Application.isPlaying) return;

            stateNames.Add(copyName(stateNames[cloneIndex]));
            VSUtils.AddState(listValue, cloneIndex);
            VSUtils.AddState(listStruct, cloneIndex);
        }
        private string copyName(string source)
        {
            string num = "";
            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (!Char.IsDigit(source[i]))
                {
                    break;
                }
                if (string.IsNullOrEmpty(num))
                {
                    num += source[i].ToString();
                }
                else
                {
                    num = num.Insert(0, source[i].ToString());
                }
                
            }
            if (string.IsNullOrEmpty(num))
            {
                return source + " 1";
            }
            return source.Substring(0, source.Length - num.Length) + (int.Parse(num) + 1);

        }

        #if UNITY_EDITOR

        public void RemoveState(int index)
        {
            if (Application.isPlaying) return;
            stateNames.RemoveAt(index);
            VSUtils.RemoveState(listValue, index);
            VSUtils.RemoveState(listStruct, index);
            CheckAndRemoveField();
            dirty = true;
        }

        #endif

        public void CheckAndRemoveField()
        {

            for (int i = listValue.Count - 1; i >= 0; i--)
            {
                if (listValue[i].IsEmptyState())
                {
                    listValue.RemoveAt(i);
                }
            }
            for (int i = listStruct.Count - 1; i >= 0; i--)
            {
                if (listStruct[i].IsEmptyState())
                {
                    listStruct.RemoveAt(i);
                }
            }

            //check and cleanup all action equal default value
            for (int i = listValue.Count - 1; i >= 0; i--)
            {
                bool isAllEqualDefault = true;
                for (int j = 0; j < nStates; j++)
                {
                    if(!listValue[i].IsStateEqualDefault(j))
                    {
                        isAllEqualDefault = false;
                        break;
                    }
                }
                if (isAllEqualDefault)
                {
                    listValue.RemoveAt(i);
                }

            }
            for (int i = listStruct.Count - 1; i >= 0; i--)
            {
                bool isAllEqualDefault = true;
                for (int j = 0; j < nStates; j++)
                {
                    if (!listStruct[i].IsStateEqualDefault(j))
                    {
                        isAllEqualDefault = false;
                        break;
                    }
                }
                if (isAllEqualDefault)
                {
                    listStruct.RemoveAt(i);
                }

            }

        }

        #if UNITY_EDITOR
        private void Reset()
        {
            if (listValue!= null)
            {
                listValue.Clear();
                listValue = null;
            }
            if (listStruct != null)
            {
                listStruct.Clear();
                listStruct = null;
            }
            if (stateNames != null)
            {
                stateNames.Clear();
                stateNames = null;
            }
            setDirty();
        }
        #endif
        
        
        
        public void RefreshState()
        {
            OnBeforeChange.Invoke();
#if UNITY_EDITOR
            EditorApplication.update -= RefreshState;
#endif
            if(haveTween)
            {
                 if(Application.isPlaying) 
                {
                    VisualStateHelper.Instance.OnUpdate -= MediateUpdate;
                    VisualStateHelper.Instance.OnUpdate += MediateUpdate;
                }
                #if UNITY_EDITOR
                else
                {
                    EditorApplication.update -= MediateUpdate;
                    EditorApplication.update += MediateUpdate;
                }
                #endif
            }
            else
            {
                VSUtils.SetState(listValue, _state);
                VSUtils.SetState(listStruct, _state);

                if (listeners != null)
                {
                     for (var i = 0; i < listeners.Count; i++)
                     {
                         var m = listeners[i];
                         if (m is IVisualStateListener)
                         {
                             (m as IVisualStateListener).OnVSChange();
                             continue;
                         }
                     }
                 }
            
                 OnAfterChange.Invoke();
            }
            // Debug.Log("Refresh state !");
            

            
        }
        private bool isTweening;
        private float step;
        private float curAmount;
        public void BeginDrag()
        {   
            if(isTweening) return;
            step = 1f / (nStates - 1);
            transitionData.nStateAmount.Clear();

            // _oldState = state;

            curAmount = step * _oldState;
            // Debug.Log(curAmount + "  " + step + "  _oldState: " + _oldState);
        }



        public void OnDrag(float delta)
        {
            if(isTweening) return;
            curAmount += delta / (nStates - 1);
            curAmount = Mathf.Clamp(curAmount, 0 , 1);
            int i = 0;
           
           var old = _oldState;
            for(float f = 0; f < 1; f += step, i++)
            {
                if(curAmount >= f)
                {
                    _oldState = i;
                    targetState = i + 1;
                }
            }

            if(old != _oldState)
            {
                // BeginDrag();
                transitionData.nStateAmount.Clear();
            }
             
             var realAmount = Mathf.Lerp(0,1,(curAmount % step)/step);
            //  Debug.Log("delta: " + delta+" curAmount: " + curAmount + "  realAmount: " + realAmount + " _oldState: "+_oldState +" targetState: " + targetState);
            UpdateTween(realAmount);
        }
        public bool OnEndDrag(float sign)
        {
            if(isTweening) return false;
            var realAmount = Mathf.Lerp(0,1,(curAmount % step)/step);

            curAmount = realAmount <.5f ? _oldState *step : targetState*step;
            // Debug.Log("END");
            var signTween = -1;
            var shouldNext = true;
            if(sign > 0)//scroll ve
            {
                signTween = realAmount < 0.9f ? -1 : 1;
                shouldNext = realAmount < 0.9f;
            }
            else
            {
                if(realAmount > 0.1f) signTween = 1;
                shouldNext = realAmount > 0.1f;
            }

            // Debug.Log("curAmount: " + curAmount + "  " + _oldState + " targetState: " + targetState + "  step: " + step + "  realAmount: " + realAmount + " "+signTween);
            tweenToComplete(realAmount,signTween);
            return shouldNext;
        }

        
        float countTime;
        int sign = 1;
        public bool isTweenToEnd 
        {
            get
            {
                return sign > 0;
            }
        }
        private void MediateUpdate()
        {

            // Debug.Log("MediateUpdate");
            UpdateTween();
        }
        public void setTargetState(int oldState, int targetState)
        {
            this._oldState = oldState;
            this.targetState = targetState;
            transitionData.nStateAmount.Clear();
            countTime = 0;
            sign = 1;
        }
        public void tweenToComplete(float amount, int sign)
        {
            isTweening = true;
            this.sign = sign;
            countTime = amount * transitionData.time;
            RefreshState();
        }
       
        public void UpdateTween(float amount = -10)
        {            
            bool manual = amount > -9;
            if(amount <= -9)
            {
                amount = Mathf.Lerp(0, 1,countTime / transitionData.time);
            }
            
            
            float defaultAmount =  transitionData.curve.Evaluate( amount);
            bool isComplete =isTweenToEnd? countTime >= transitionData.time : countTime <= 0;
            if(manual) isComplete = false;
            // Debug.Log("manual:" + manual +" amount: " + amount + " vs: " + defaultAmount + " isComplete: " + isComplete + "  isTweenToEnd: " + isTweenToEnd + " countTime: " + countTime);
            

            //update amount each state
           VisualState_Unity.UpdateProgressState
           (
               ref  transitionData.nStateAmount,
               targetState, 
               defaultAmount, 
               nStates,
               _oldState,
               isComplete
            );

            //update value
            RefreshValue(listStruct, defaultAmount, isComplete);
            RefreshValue(listValue, defaultAmount, isComplete);

            if(!manual)
                countTime += Time.deltaTime * sign;
            if(isComplete)
            {
                _state =isTweenToEnd? targetState : _oldState;

                _oldState = _state;
                countTime =isTweenToEnd? transitionData.time : 0;
                defaultAmount =isTweenToEnd ? 1 : 0;
                CompleteTween();
                isTweening = false;
                // Debug.Log("complete: " + _state + "  " + targetState + "  " + _oldState);
                
            }
            
            if (listeners != null)
            {
                for (var i = 0; i < listeners.Count; i++)
                {
                    var m = listeners[i];
                    if (m is IVisualStateListener)
                    {
                        (m as IVisualStateListener).OnVSChange();
                        continue;
                    }
                }
            }
        }
        // private void RefreshValue<T>(List<T> listSource)where T: VSPropertyBase
        // {
            
        //     for(int i = 0; i < listSource.Count; i++)
        //     {
        //         object result = listSource[i].GetStateValuePercent(0, nStateAmount[0]);
        //         for(int k = 1; k < nStateAmount.Count; k++)
        //         {
        //             object val2 = listSource[i].GetStateValuePercent(k, nStateAmount[k]);
        //             VisualState_Unity.Add2Value(result, val2, ref result);
        //         }
        //          listSource[i].SetValue(result);
        //     }
        // }
        
        private void RefreshValue<T>(List<T> listSource, float defaultAmount, bool isComplete)where T: VSPropertyBase
        {
            var amount = defaultAmount;
            

            
            for(var i = 0; i < listSource.Count; i++)
            {
                var item = listSource[i];
                item.SetValueAmount(targetState, amount, nStates, _oldState, transitionData.nStateAmount,isComplete);
                
            }
        }


        void CompleteTween()
        {
            if(Application.isPlaying) 
                {
                    VisualStateHelper.Instance.OnUpdate -= MediateUpdate;
                }
                #if UNITY_EDITOR
                else
                {
                    EditorApplication.update -= MediateUpdate;
                }
                #endif
            OnAfterChange.Invoke();
        }
#region Old tween flow
        // class TweenDataHolder
        // {
        //     public object startValue;
        //     public object targetValue;
        //     public VSPropertyBase propertyBase;
        // }
        // List<TweenDataHolder> listTween;
        // void InitTweenData(int state)
        // {
        //     var curve = transitionData.curve;
        //     if(curve.keys.Length > 0)
        //     {
        //         curveTime = curve.keys[curve.keys.Length -1].time;
        //     }
        //     else
        //     {
        //         curveTime = 0;
        //     }
        //     countTime = 0;
            
        //     listTween = new List<TweenDataHolder>();
        //     AppendTween(listTween, state, listValue);
        //     AppendTween(listTween, state, listStruct);
            
        // }
        // private void AppendTween<T>(List<TweenDataHolder> listTween, int state, List<T> listSource)where T: VSPropertyBase
        // {
        //     for(int i = 0; i < listSource.Count; i++) 
        //     {
        //         listTween.Add(new TweenDataHolder()
        //         {
        //             startValue = listSource[i].GetCurrentValue(),
        //             targetValue = listSource[i].GetStateValue(state),
        //             propertyBase = listSource[i]
        //         });
        //         Debug.Log(listSource[i].GetCurrentValue() + "  " + listSource[i].GetStateValue(state) + "  " + listSource[i]);
        //     }
        // }

        // float countTime;
        // float curveTime;
        // int fromState, toState;
       
        // private void UpdateTween()
        // {
        //     var time = Mathf.Lerp(0, curveTime,countTime / transitionData.time);
        //     var amount = transitionData.curve.Evaluate( time);

        //     for(int i =listTween.Count -1; i >= 0; i--)
        //     {
        //         var item = listTween[i];
        //         var overrideData = item.propertyBase.overrideData;
        //         var type = overrideData.type; 
        //         if(type == OverrideType.OverrideWithDelayTime)
        //         {
        //             if(overrideData.DelayTime <= countTime) 
        //             {
        //                 item.propertyBase.SetValue(item.targetValue);
        //                 listTween.RemoveAt(i);
        //             }

        //         }
        //         else
        //         {
        //             var _tempAmount = (type == OverrideType.None) ? amount:overrideData.curve.Evaluate(time); 

        //             var val = VisualState_Unity.GetLerpValue(item.startValue, item.targetValue, _tempAmount);
        //             item.propertyBase.SetValue(val);
        //         }
                
        //     }

            
        //     countTime += Time.deltaTime;
        //     if(countTime >= transitionData.time)
        //     {
        //         CompleteTween();
                
        //     }
        //     if (listeners != null)
        //     {
        //         for (var i = 0; i < listeners.Count; i++)
        //         {
        //             var m = listeners[i];
        //             if (m is IVisualStateListener)
        //             {
        //                 (m as IVisualStateListener).OnVSChange();
        //                 continue;
        //             }
        //         }
        //     }
        // }
        // void CompleteTween()
        // {
        //     for(int i = 0; i < listTween.Count; i++)
        //         {
        //             var item = listTween[i];
        //             item.propertyBase.SetValue(item.targetValue);
        //         }
        //         if(Application.isPlaying) 
        //         {
        //             VisualStateHelper.Instance.OnUpdate -= UpdateTween;
        //         }
        //         #if UNITY_EDITOR
        //         else
        //         {
        //             EditorApplication.update -= UpdateTween;
        //         }
        //         #endif
        //     OnAfterChange.Invoke();
        // }
        #endregion
        // -------------------------- EDITOR SUPPORT ----------------------

#if UNITY_EDITOR
        public static bool isHideDefault = false;
        [NonSerialized] public int refreshCount;
        public static Color ActiveColor = new Color(1f, 0, 0, 0.2f);
        //layout
        public const float PaddingLeftElement = 20;
        public const float ButtonRemoveWidth = 20;
        public static float SingleHeight = EditorGUIUtility.singleLineHeight - 2;
        static GUIStyle horizontalLine;
        public static void HorizontalLine(Color color)
        {
            if (horizontalLine == null)
            {
                horizontalLine = new GUIStyle();
                horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
                horizontalLine.fixedHeight = 1;
                horizontalLine.margin = new RectOffset((int)PaddingLeftElement, 0, 4, 4);
            }
            
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }


        bool dirty;
        List<VSTarget> listTargets;

        public List<VSTarget> changedTargets
        {
            get
            {
                if (listTargets == null || dirty) RebuildTargets();
                return listTargets;
            }
        }
        public void setDirty()
        {
            dirty = true;
        }
        public void RebuildTargets()
        {
            dirty = false;
            if (listValue == null) listValue = new List<VSValue>();
            if (listStruct == null) listStruct = new List<VSStruct>();

            //TODO : Optimize - should reuse VSTargets to prevent GCs ?

            var dict = new Dictionary<Object, VSTarget>();
            AddTarget(listValue, dict);
            AddTarget(listStruct, dict);

            listTargets = dict.Values.ToList();
            for (var i = 0; i < listTargets.Count; i++)
            {
                listTargets[i].properties.Sort((item1, item2) =>
                {
                    return item1.property.CompareTo(item2.property);
                });
            }

            //Debug.Log("Rebuild Targets : " +  listTargets.Count);
        }

        void AddTarget<T>(List<T> list, Dictionary<Object, VSTarget> dict) where T : VSPropertyBase
        {
            for (var i = 0; i < list.Count; i++)
            {
                var c = list[i];
                if (c.target == null) continue;

                VSTarget target;
                if (!dict.TryGetValue(c.target, out target))
                {
                    target = new VSTarget()
                    {
                        target = c.target,
                        properties = new List<VSPropertyBase>()
                    };

                    dict.Add(c.target, target);
                }

                target.properties.Add(c);
            }
        }

        public void removeProperty(int index, bool isListValue)
        {
            if (isListValue)
            {
                listValue.RemoveAt(index);
            }
            else
            {
                listStruct.RemoveAt(index);
            }
            setDirty();
        }
        public void setOverrideProperty(int index, bool isListValue, OverrideType type)
        {
            if (isListValue)
            {
                listValue[index].overrideData.type = type;
            }
            else
            {
                listStruct[index].overrideData.type = type;
            }
            setDirty();
        }
        // ------------------------------ MODIFICATION ----------------------------------

        public static VisualState monitoring;
        public static bool devMode = true;

        [ContextMenu("Toggle DevMode")]
        public void ToggleDevMode()
        {
            devMode = !devMode;
        }

        [ContextMenu("Start Monitor")]
        public void StartMonitor()
        {
            if (Application.isPlaying) return;
            monitoring = this;
            Undo.postprocessModifications -= OnModify;
            Undo.postprocessModifications += OnModify;
        }

        [ContextMenu("Stop Monitor")]
        public void StopMonitor()
        {
            if (Application.isPlaying) return;
            monitoring = null;
        }

        UndoPropertyModification[] OnModify(UndoPropertyModification[] modifications)
        {
            if (Application.isPlaying) return modifications;
            if (monitoring != this)
            {
                Undo.postprocessModifications -= OnModify;
                return modifications;
            }

            for (var i = 0; i < modifications.Length; i++)
            {
                var m = modifications[i];
                PropertyModification[] arr = VisualState_Unity.GetValueModification(m);
                var p1 = arr[0];
                var p2 = arr[1];

                if (!AddChange(p1, p2))
                {
                    //Debug.Log(
                    //	string.Format("target={0} ref={1} prop={2} val={3}", p1.target , p1.objectReference, p1.propertyPath, p1.value) + "\n" +
                    //	string.Format("target={0} ref={1} prop={2} val={3}", p2.target , p2.objectReference, p2.propertyPath, p2.value)
                    //);
                };
            }

            return modifications;
        }

        bool ShouldRecordTarget(Object target)
        {
            if (this == null) return false;
            if (target == this) return false;

            var t = (target is Component) ? (target as Component).transform :
                    (target is GameObject) ? (target as GameObject).transform : null;

            if (t == null) return false;
            if (t == transform) return true;

            while (t.parent != null)
            {
                t = t.parent;

                if (t == transform) return true; //target being manged by this VisualState

                var c = t.GetComponent<VisualState>();
                if (c != null) return false; //this target is managed by other VisualState
            }

            return false;
        }

        bool AddChange(PropertyModification from, PropertyModification to)
        {
            if (Application.isPlaying) return false;

            var t = from.target;
            if (!ShouldRecordTarget(t))
            {
                //Debug.Log("Should not record target : " + t);
                return false; // invalid target
            }

            var p = from.propertyPath;
            // Debug.Log(p);
            

            var changedName = "";
            if(VSGetSet_GO.PROPERTIES_CHANGED_NAME.TryGetValue(p, out changedName))
            {
                p = changedName;
            }

            var arr = p.Split('.');

            if (VisualState_Unity.CheckAddColorGUIText4_5(from, to, this) == true)
            {
                setDirty();
                return true;
            }
            

            if (arr.Length == 1)
            {
                var vs = FindProperty(listValue, t, p);
                if (vs != null)
                {
                    if (vs.isNormal) vs.WriteChange(_state, to);
                    return true;
                }

                // not existed, create new!
                vs = NewProperty<VSValue>(null, p, from, to);
                if (vs != null)
                {
                    listValue.Add(vs);
                    dirty = true;
                }

                return true;
            }

            if (arr.Length == 2)
            {
                var vs = FindProperty(listStruct, t, arr[0]);
                var vsField = vs != null ? vs.field : VSGetSet.Create(from.target, arr[0]);

                if (vs == null)
                {
                    vs = NewProperty<VSStruct>(null, arr[0], from, to);
                    if (vs == null) return false; //unsupported

                    listStruct.Add(vs);
                    dirty = true;
                }

                //check children of vs
                var c = FindProperty(vs.children, null, arr[1]);
                if (c == null)
                {
                    var v = vsField.GetValue();
                    var cField = VSGetSet.Create(v, arr[1]);
                    cField.invokeTarget = v;

                    c = NewProperty<VSValue>(cField, arr[1], from, to); //use the field from VSStruct
                    if (c == null) return false; //unsupported
                    vs.children.Add(c);
                    dirty = true;
                }

                c.WriteChange(_state, to);
                return true;
            }

            return false;
        }

        public T FindProperty<T>(List<T> list, Object tar, string prop) where T : VSPropertyBase
        {
            for (var i = 0; i < list.Count; i++)
            {
                var v = list[i];
                if (v.property == prop)
                {
                    if (tar == null || (v.target == tar)) return v;
                }
            }

            return null;
        }

        public T NewProperty<T>(VSGetSet f, string prop, PropertyModification from, PropertyModification to) where T : VSPropertyBase, new()
        {
            if (f == null)
            {
                f = VSGetSet.Create(from.target, prop);
                if (f == null)
                {
                    Debug.LogWarning("Unsupported : " + from.target + ":" + prop + "  " + from.value +" "+ from.objectReference);
                    
                    // Debug.Log("===Property===");
                    // var type = from.target.GetType();
                    // foreach(var item in type.GetProperties(VSGetSet.FLAGS))
                    // {
                    //     Debug.Log(item.Name);
                    // }

                    // Debug.Log("===Field===");
                    // var fields = type.GetFields(VSGetSet.FLAGS);
                    // foreach(var item in fields)
                    // {
                    //     Debug.Log(item.Name);
                    // }
                    
                    return null; // unsupported
                }
            }
            var result = new T() { property = prop, target = from.target, field = f };
            result.WriteDefault(nStates, from);
            result.WriteChange(_state, to);
            return result;
        }
#endif
    }
}



