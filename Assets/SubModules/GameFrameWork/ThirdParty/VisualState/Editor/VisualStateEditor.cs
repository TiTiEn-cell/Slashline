using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

namespace vietlabs.vs
{
    [CustomEditor(typeof(VisualState))]
    public class VisualStateEditor : Editor
    {
        private SerializedProperty onBeforeChange;
        private SerializedProperty onAfterChange;

        void OnEnable()
        {
            onBeforeChange = serializedObject.FindProperty("OnBeforeChange");
            onAfterChange = serializedObject.FindProperty("OnAfterChange");
        }
        void DrawStates(VisualState vs)
        {
            if (vs.stateNames == null || vs.nStates < 2)
            {
                if (GUILayout.Button("Create On/Off"))
                {
                    vs.stateNames = new List<string>()
                    {
                        "off", "on"
                    };

                    vs.Resize(2);
                    EditorUtility.SetDirty(vs);
                }

                if (GUILayout.Button("Create 3 States"))
                {
                    vs.stateNames = new List<string>()
                    {
                        "off", "on", "half"
                    };

                    vs.Resize(3);
                    EditorUtility.SetDirty(vs);
                }

                return;
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(vs.stateName, EditorStyles.largeLabel, GUILayout.Width(100f));
                var idx = EditorGUILayout.IntSlider(vs.state, 0, vs.nStates - 1);
                if (vs.state != idx) {
                    vs.SetState(idx);
                    EditorUtility.SetDirty(vs);
                }

            }
            GUILayout.EndHorizontal();
        }

        void DrawMonitor(VisualState vs)
        {
            var isMonitor = VisualState.monitoring == vs;
            var v = GUILayout.Toggle(isMonitor, "Monitor");
            if (v != isMonitor)
            {
                if (v)
                {
                    vs.StartMonitor();
                } else {
                    vs.StopMonitor();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            //if (VisualState.devMode) base.OnInspectorGUI();
            //if (Application.isPlaying) return;

            EditorGUILayout.PropertyField(onBeforeChange);
            EditorGUILayout.PropertyField(onAfterChange);

            var vs = (VisualState)target;
            if (vs == null) return;

            if (VisualState.monitoring != null && VisualState.monitoring != vs)
            {
                EditorGUILayout.HelpBox("Monitoring : " + VisualState.monitoring.gameObject, MessageType.Warning);
                return;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("States", EditorStyles.boldLabel);
            if (vs.stateNames == null || vs.nStates <= 0)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("");
                    if (GUILayout.Button("Create State..."))
                    {
                        vs.Create2State();
                        VisualStateEditorWindow.Init();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(VisualState.PaddingLeftElement);
                    GUILayout.Label(vs.stateNames[vs.state]);
                    // var idx = EditorGUILayout.IntSlider(vs.state, 0, vs.nStates - 1);
                    // if (vs.state != idx)
                    // {
                    //     vs.SetState(idx);
                    // }

                    var idx = EditorGUILayout.Slider(vs.state2, 0, vs.nStates - 1);
                    if (vs.state2 != idx)
                    {
                        vs.SetState2(idx);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("");
                    if (GUILayout.Button("Edit State..."))
                    {
                        VisualStateEditorWindow.Init(vs);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
            //DrawStates(vs);
            //DrawMonitor(vs);
            //var list = vs.changedTargets;
            //for (var i =0 ;i < list.Count; i++)
            //{
            //	var c = list[i];
            //	c.OnGUI(vs);
            //}

            //if (GUILayout.Button("Refresh"))
            //{
            //	vs.RebuildTargets();
            //	vs.RefreshState();
            //}
        }
	}
}

