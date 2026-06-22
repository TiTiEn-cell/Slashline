using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GFramework.GUI
{
    [CustomEditor(typeof(CloneScript), true)]
    public class CloneScriptInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CloneScript myScript = (CloneScript)target;

            GUILayout.Space(20f);

            if (GUILayout.Button("Clone Classes"))
            {
                CloneClass(myScript);
            }

            if (GUILayout.Button("Change Object"))
            {
                ChangeObject(myScript);
            }
        }

        private void CloneClass(CloneScript cloneScrip)
        {
            if (cloneScrip == null || cloneScrip.script == null)
            {
                EditorUtility.DisplayDialog("Notice", "Missing Script !!!", "OK");
                return;
            }

            if (cloneScrip.newName == null || cloneScrip.newName == "")
            {
                EditorUtility.DisplayDialog("Notice", "Empty New Name !!!", "OK");
                return;
            }

            Type scriptType = Type.GetType(cloneScrip.newName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

            if (scriptType != null)
            {
                EditorUtility.DisplayDialog("Notice", "Script already exits !!!", "OK");
                return;
            }

            string scriptGuiFilePath_Source = AssetDatabase.GetAssetPath(cloneScrip.script);

            //Debug.LogError("scriptGuiFilePath_Source: " + scriptGuiFilePath_Source);
            //Debug.LogError("Script Name: " + ms.name);

            string scriptGuiFilePath_Dest = Application.dataPath + "/" + cloneScrip.newPath + "/" + cloneScrip.newName + ".cs";

            StreamWriter fileWriter = null;
            string[] fileData = File.ReadAllLines(scriptGuiFilePath_Source);

            try
            {
                fileWriter = new StreamWriter(scriptGuiFilePath_Dest, false);

                for (int i = 0; i < fileData.Length; i++)
                {
                    if (fileData[i].Contains(cloneScrip.script.name))
                    {
                        fileData[i] = fileData[i].Replace(cloneScrip.script.name, cloneScrip.newName);
                    }

                    fileWriter.WriteLine(fileData[i]);
                }

                fileWriter.Close();
            }
            catch (Exception ex)
            {
                if (fileWriter != null)
                    fileWriter.Close();
                Debug.LogError("Error:" + ex.Message);
            }

            AssetDatabase.Refresh();
        }

        public void ChangeObject(CloneScript cloneScrip)
        {
            GameObject tmpGOSub = new GameObject("tempOBJ");
            MonoBehaviour scriptSub = tmpGOSub.AddComponent(Type.GetType(cloneScrip.newName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")) as MonoBehaviour;
            MonoScript monocriptSub = MonoScript.FromMonoBehaviour(scriptSub);
            DestroyImmediate(tmpGOSub);

            UnityEngine.Object suBObj = cloneScrip.transform.GetComponent(Type.GetType(cloneScrip.script.name + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"));
            
            if (suBObj == null)
            {
                EditorUtility.DisplayDialog("Notice", "Script " + cloneScrip.script.name + "was not found !!!", "OK");
                return;
            }
            else
            {
                SerializedObject subSO = new SerializedObject(suBObj);
                SerializedProperty subScriptProperty = subSO.FindProperty("m_Script");

                subSO.Update();
                subScriptProperty.objectReferenceValue = monocriptSub;
                subSO.ApplyModifiedProperties();
            }
        }
    }
}