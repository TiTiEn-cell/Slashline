using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace GFramework.GUI
{
    [CustomEditor(typeof(GUIHandlerBase), true)]
    public class GUIHandlerBaseInspector : Editor
    {
        private GUIHandlerBase guiLoader;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                //DrawDefaultInspector();

                guiLoader = (GUIHandlerBase)target;

                switch (guiLoader.guiAnimType)
                {
                    case GUIAnimationType.None:
                        DrawPropertiesExcluding(serializedObject, 
                            new string[] {"showAnim", "showCurve", "hideAnim", "hideCurve", "showAnimClip", "hideAnimClip"});
                        break;
                    case GUIAnimationType.AnimationClip:
                        DrawPropertiesExcluding(serializedObject,
                            new string[] { "showAnim", "showCurve", "hideAnim", "hideCurve"});
                        break;
                    case GUIAnimationType.Scripting:
                        DrawPropertiesExcluding(serializedObject,
                            new string[] { "showAnimClip", "hideAnimClip" });
                        break;
                }

                serializedObject.ApplyModifiedProperties();

                GUILayout.Label("____________________________", EditorStyles.largeLabel);

                GUILayout.Label("Load GUI on Editor", EditorStyles.largeLabel);

                EditorGUILayout.BeginHorizontal(UnityEngine.GUI.skin.box);
                {
                    if (GUILayout.Button("Load", GUILayout.Width(80), GUILayout.Height(20)))
                    {
                        guiLoader.Load();
                    }
                    if (GUILayout.Button("UnLoad", GUILayout.Width(80), GUILayout.Height(20)))
                    {
                        //Debug.LogError("Unload");
                        guiLoader.UnLoad();
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Show/Hide GUI on Editor", EditorStyles.largeLabel);

                EditorGUILayout.BeginHorizontal(UnityEngine.GUI.skin.box);
                {
                    if (GUILayout.Button("Show", GUILayout.Width(80), GUILayout.Height(20)))
                    {
                        if (Application.isPlaying)
                            guiLoader.Show();
                        else
                            guiLoader.ShowEditor();
                    }
                    if (GUILayout.Button("Hide", GUILayout.Width(80), GUILayout.Height(20)))
                    {
                        if (Application.isPlaying)
                            guiLoader.Hide();
                        else
                            guiLoader.HideEditor();
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Label("Reset GUI on Editor", EditorStyles.largeLabel);

                EditorGUILayout.BeginHorizontal(UnityEngine.GUI.skin.box);
                {
                    if (GUILayout.Button("Reset", GUILayout.Width(80), GUILayout.Height(20)))
                    {
                        guiLoader.Reset();
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Label("____________________________", EditorStyles.largeLabel);

                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndVertical();
            if (UnityEngine.GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        //public void OnSceneGUI()
        //{
        //}
    }

    [CustomPropertyDrawer(typeof(GUIHandlerBase))]
    public class GUIHandlerBaseInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);

            // Draw label
            //pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width * 0.85f, pos.height), prop, true);

            if (UnityEngine.GUI.Button(new Rect(pos.x + pos.width * 0.85f, pos.y, pos.width * 0.075f, pos.height), new GUIContent("S", "Show")))
            {
                GUIHandlerBase obj = prop.objectReferenceValue as GUIHandlerBase;
                if (obj != null)
                {
                    if (Application.isPlaying)
                        obj.Show();
                    else
                        obj.ShowEditor();
                }
            }
            if (UnityEngine.GUI.Button(new Rect(pos.x + pos.width * 0.925f, pos.y, pos.width * 0.075f, pos.height), new GUIContent("H", "Hide")))
            {
                GUIHandlerBase obj = prop.objectReferenceValue as GUIHandlerBase;
                if (obj != null)
                {
                    if (Application.isPlaying)
                        obj.Hide();
                    else
                        obj.HideEditor();
                }
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
