using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace vietlabs.vs
{
    #region Extensions
    public static class Extension
    {
        public static bool HasComponent<T>(this GameObject source) where T : Component
        {
            return source.GetComponent<T>() != null;
        }
    }
    #endregion

    public class VisualStateEditorWindow : EditorWindow
    {

        [MenuItem("Window/VietLabs/Visual State")]
        public static void Init()
        {
            VisualStateEditorWindow window = (VisualStateEditorWindow)GetWindow(typeof(VisualStateEditorWindow), false, "Visual State", true);
            window.Show();
        }
        public static void Init(VisualState state)
        {
            VisualStateEditorWindow window = (VisualStateEditorWindow)GetWindow(typeof(VisualStateEditorWindow), false, "Visual State", true);
            window.Show();
            window.target = state;
        }

        private VisualState target;

        private bool isInfocus = true;
        private bool isExpandStates = true;
        private Vector2 scrollContent;
        GUIStyle HeaderStyle
        {
            get
            {
                return EditorStyles.boldLabel;
            }
        }
        #region GUIContent
        // create your style
        
        public static GUIContent previewContent;
        internal  class Constant
        {
            
            private static bool ms_LoadedIcons = false;
            public static GUIStyle MiniButtonLeft;
            static internal void LoadIcons()
            {
                if (ms_LoadedIcons)
                    return;

                ms_LoadedIcons = true;
                MiniButtonLeft = new GUIStyle("ToolbarButton");
            }
        }
        
        #endregion


        private void OnEnable()
        {
            previewContent = EditorGUIUtility.IconContent("Animation.Record", "Enable/disable keyframe recording mode.");

            checkChangeTarget(Selection.activeGameObject);
            
        }
        private void OnTargetChanged()
        {

        }

        private void OnFocus()
        {
            isInfocus = true;
        }
        private void OnLostFocus()
        {
            isInfocus = false;
        }
        void OnInspectorUpdate()
        {
            if (!isInfocus)
            {
                checkChangeTarget(Selection.activeGameObject);
                this.Repaint();
            }

        }
        private void OnGUI()
        {
            Constant.LoadIcons();
            if (target == null)
            {
                return;
            }
            if (VisualState.monitoring != null && VisualState.monitoring != target)
            {
                EditorGUILayout.HelpBox("Monitoring : " + VisualState.monitoring.gameObject, MessageType.Warning);
                return;
            }
            if (target.stateNames == null || target.stateNames.Count <= 0)
            {
                if (GUILayout.Button("Create 2 State"))
                {
                    target.Create2State();
                }
                return;
            }
            DrawRecordButton(target);
            scrollContent = EditorGUILayout.BeginScrollView(scrollContent);
            {
                DrawCurve();
                DrawHeader();
                EditorGUILayout.Space();
                VisualState.HorizontalLine(Color.black);
                EditorGUILayout.LabelField("Properties", HeaderStyle);
                //need check again
                if (target.nStates > 0)
                {
                    DrawKeys();
                }
            }
            EditorGUILayout.EndScrollView();

        }
        private void DrawCurve()
        {
            EditorGUILayout.LabelField("Transition", HeaderStyle);
            var curve = EditorGUILayout.CurveField("Curve",target.transitionData.curve);
            if(curve != target.transitionData.curve)
            {
                target.transitionData.curve = curve;
                EditorUtility.SetDirty(target);
            }
            var time = EditorGUILayout.FloatField("Time", target.transitionData.time);
            if(time != target.transitionData.time)
            {
                target.transitionData.time = time;
                EditorUtility.SetDirty(target);
            }
        }
        private void DrawHeader()
        {
            EditorGUILayout.LabelField("States", HeaderStyle);
            if (isExpandStates)
            {
                bool isStateChanged = false;
                for (int i = 0; i < target.nStates; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(VisualState.PaddingLeftElement);
                        if (EditorGUILayout.Toggle(target.state == i, GUILayout.Width(20f)) && target.state != i)
                        {
                            target.SetState(i);
                            isStateChanged = true;
                        }
                        target.stateNames[i] = EditorGUILayout.TextField(target.stateNames[i]);

                        if (GUILayout.Button("-", GUILayout.Width(VisualState.ButtonRemoveWidth), GUILayout.Height(VisualState.SingleHeight)))
                        {
                            int state = target.state;
                            if (state >= i)
                            {
                                state--;
                            }
                            if (state < 0)
                            {
                                state = 0;
                            }
                            target.SetState(state);
                            target.RemoveState(i);
                            return;
                        }
                        if (GUILayout.Button("Duplicate", GUILayout.Width(VisualState.ButtonRemoveWidth * 3.5f), GUILayout.Height(VisualState.SingleHeight)))
                        {

                            target.AddState(i);
                            target.SetState(target.nStates - 1);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space(VisualState.PaddingLeftElement);
                    var idx = EditorGUILayout.IntSlider(target.state, 0, target.nStates - 1);
                    if (target.state != idx)
                    {
                        target.SetState(idx);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #region Draw Add Button
                /* 
                 * Draw Add Button
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(VisualState.PaddingLeftElement);
                if (GUILayout.Button("Add"))
                {
                    if (target.stateNames == null || target.nStates <= 0)
                    {
                        target.stateNames = new List<string> { "Normal" };
                        target.Resize(1);
                    }
                    else
                    {
                        target.AddState(target.nStates - 1);
                    }
                    target.state = target.nStates - 1;
                    //target.Resize(target.)
                }
                GUILayout.Space(34f);
                EditorGUILayout.EndHorizontal();
                */
                #endregion
                if (isStateChanged)
                {
                    EditorUtility.SetDirty(target);
                }

            }
        }
        private void DrawRecordButton(VisualState vs)
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbarButton);
            {
                PreviewButtonOnGUI(vs);
                GUILayout.Space(5);
                VisualState.isHideDefault = GUILayout.Toggle(VisualState.isHideDefault, "Hide Default", Constant.MiniButtonLeft);
                VisualState.devMode = GUILayout.Toggle(VisualState.devMode, "Show Ignore", Constant.MiniButtonLeft);
            }
            GUILayout.EndHorizontal();

        }
        private void PreviewButtonOnGUI(VisualState vs)
        {
            EditorGUI.BeginChangeCheck();
            Color backupColor = GUI.color;
            if (VisualState.monitoring)
            {
                Color recordedColor = VisualState.ActiveColor;
                //recordedColor.a *= GUI.color.a;
                GUI.color = recordedColor;
            }
            bool isMonitor = GUILayout.Toggle(VisualState.monitoring, previewContent, EditorStyles.toolbarButton, GUILayout.Width(30));
            if (VisualState.monitoring)
            {
                GUI.color = backupColor;
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (isMonitor)
                {
                    vs.StartMonitor();
                }
                else
                {
                    vs.StopMonitor();
                }
            }
        }


        private void DrawKeys()
        {
            //scrollContent = EditorGUILayout.BeginScrollView(scrollContent);
            {
                var list = target.changedTargets;
                for (var i = 0; i < list.Count; i++)
                {
                    var c = list[i];
                    c.OnWindowGUI(target);

                }
            }
            //EditorGUILayout.EndScrollView();

        }

        private void OnSelectionChange()
        {
            checkChangeTarget(Selection.activeGameObject);
            Repaint();
        }

        private void checkChangeTarget(GameObject newSelectionTarget)
        {
            if(VisualState.monitoring != null) return;
            if (newSelectionTarget != null && newSelectionTarget.HasComponent<VisualState>())
            {
                VisualState vsNew = newSelectionTarget.GetComponent<VisualState>();
                if (target == null || target != vsNew)
                {
                    target = vsNew;
                    OnTargetChanged();
                }
            }
        }


    }

}