using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using System;
using GFramework.Utils;

namespace GFramework.GUI
{
    [CustomEditor(typeof(GUIManagerBase), true)]
    public class GUIManagerBaseInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            // Update the serializedProperty
            serializedObject.Update();

            DrawDefaultInspector();

            GUILayout.Label("____________________________", EditorStyles.largeLabel);

            GUIManagerBase myScript = (GUIManagerBase)target;

            GUILayout.Label("Create new GUI", EditorStyles.largeLabel);

            GUILayout.Space(20f);

            myScript.prefabPath = EditorGUILayout.TextField("Prefab Path", myScript.prefabPath);

            myScript.destPrefabPath = EditorGUILayout.TextField("Dest Prefab Path", myScript.destPrefabPath);

            myScript.destSourcePath = EditorGUILayout.TextField("Source Path", myScript.destSourcePath);

            myScript.newGuiName = EditorGUILayout.TextField("New Gui Name", myScript.newGuiName);

            GUILayout.Label("____________________________", EditorStyles.largeLabel);

            myScript.createWithTemplate = EditorGUILayout.Toggle("Create With Template", myScript.createWithTemplate);

            if (myScript.createWithTemplate)
            {
                myScript.template = EditorGUILayout.ObjectField("Template", myScript.template, typeof(GUIBase), true) as GUIBase;

                GUILayout.Space(20f);

                if (GUILayout.Button("Create GUI Classes With Template"))
                {
                    CreateNewGUIClassWithTemplate(myScript);
                }

                if (GUILayout.Button("Create GUI Objects With Template"))
                {
                    CreateNewGUIObjectWithTemplate(myScript);
                }
            }
            else
            {
                GUILayout.Space(20f);

                if (GUILayout.Button("Create GUI Classes"))
                {
                    CreateNewGUIClass(myScript);
                }

                if (GUILayout.Button("Create GUI Objects"))
                {
                    CreateNewGUIObject(myScript);
                }
            }

            GUILayout.Label("____________________________", EditorStyles.largeLabel);

            GUILayout.Label("Build GUI", EditorStyles.largeLabel);

            myScript.destPath = EditorGUILayout.TextField("Dest Path", myScript.destPath);

            GUILayout.Space(20f);

            if (GUILayout.Button("Build GUI"))
            {
                BuildGUI(myScript);
            }

            GUILayout.Label("____________________________", EditorStyles.largeLabel);
            EditorGUILayout.Separator();

            serializedObject.ApplyModifiedProperties();
        }

        public void BuildGUI(GUIManagerBase guiManager)
        {
            StreamWriter fileWriter = null;

            try
            {
                fileWriter = new StreamWriter(Application.dataPath + "/" + guiManager.destPath + "/" + guiManager.destName, false);

                fileWriter.WriteLine("using UnityEngine;");
                fileWriter.WriteLine("using System.Collections;");
                fileWriter.WriteLine("");
                fileWriter.WriteLine("public static class GUIName");
                fileWriter.WriteLine("{");

                for (int i = 0; i < guiManager.listHandler.Count; i++)
                {
                    if (guiManager.listHandler[i] != null)
                        fileWriter.WriteLine("\tpublic static readonly int GUI_" + guiManager.listHandler[i].name + " = " + i + ";");
                }
                fileWriter.WriteLine("}");

                fileWriter.Close();

                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Notice", "Build GUI successfully !!!", "OK");
            }
            catch (Exception ex)
            {
                if (fileWriter != null)
                    fileWriter.Close();
                Debug.LogError("Error:" + ex.Message);
            }
        }

        public void CreateNewGUIClass(GUIManagerBase guiManager)
        {
            if (!CheckConditionCreateGUI(guiManager))
                return;

            CreateGUIHandlerScript(guiManager);
            CreateGUIScript(guiManager);

            AssetDatabase.Refresh();
        }

        public void CreateNewGUIObject(GUIManagerBase guiManager)
        {
            if (!CheckConditionCreateGUIObject(guiManager))
                return;

            string prefabFullName = "Assets/" + guiManager.destPrefabPath + "/" + guiManager.newGuiName + ".prefab";
            if (File.Exists(prefabFullName))
            {
                EditorUtility.DisplayDialog("Notice", "GUI prefab already exits !!!", "OK");
                return;
            }

            GameObject rootGUI = GameObject.Find("GUIRoot");

            if (rootGUI == null)
            {
                rootGUI = new GameObject();
                rootGUI.name = "GUIRoot";
                rootGUI.AddComponent<GlobalData>();
            }

            //Create GUI Handler object
            GameObject guiObject = new GameObject(guiManager.newGuiName + "Handler");
            guiObject.transform.position = Vector3.zero;
            guiObject.transform.SetParent(guiManager.transform);

            GUIHandlerBase guiHandlerBase =
                guiObject.AddComponent(Type.GetType(guiManager.newGuiName + "Handler, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")) as GUIHandlerBase;

            guiHandlerBase.guiCamera = guiManager.guiCamera;
            guiHandlerBase.guiName = guiManager.newGuiName;

            if (guiManager.listHandler.Count <= 0)
            {
                guiManager.listHandler.Add(guiHandlerBase);
            }
            else
            {
                if (guiManager.listHandler[guiManager.listHandler.Count - 1] == null)
                    guiManager.listHandler[guiManager.listHandler.Count - 1] = guiHandlerBase;
                else
                    guiManager.listHandler.Add(guiHandlerBase);
            }

            //Create canvas prefab
            // UnityEngine.Object emptyObj = PrefabUtility.SaveAsPrefabAsset(new GameObject(), prefabFullName);

            GameObject parentsObject = new GameObject(guiManager.newGuiName, typeof(RectTransform));
            GUIBase guiScript = parentsObject.AddComponent(Type.GetType(guiManager.newGuiName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")) as GUIBase;

            parentsObject.AddComponent<Canvas>();
            parentsObject.AddComponent<CanvasScaler>();
            parentsObject.AddComponent<GraphicRaycaster>();
            // remember to add scipt CanvasHelper
            parentsObject.AddComponent<CanvasHelper>();


            parentsObject.transform.SetParent(rootGUI.transform);
            parentsObject.layer = 5; //UI Layer

            parentsObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            parentsObject.GetComponent<Canvas>().pixelPerfect = true;

            parentsObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            parentsObject.GetComponent<CanvasScaler>().referenceResolution = guiManager.ScreenType == GUIManagerBase.EScreenType.Landscape ? new Vector2(1920, 1080) : new Vector2(1080, 1920);
            parentsObject.GetComponent<CanvasScaler>().scaleFactor = 0.5f;

            GameObject controlGroup = new GameObject("Control");

            RectTransform rectTransform = controlGroup.AddComponent<RectTransform>();
            controlGroup.AddComponent<CanvasGroup>();

            //For streched anchor
            rectTransform.SetParent(parentsObject.transform);

            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);

            Animation animationController = controlGroup.AddComponent<Animation>();
            guiScript.animController = animationController;

            guiHandlerBase.guiPrefabObj = PrefabUtility.SaveAsPrefabAsset(parentsObject, prefabFullName);

            //Link handler and canvas gui
            GameObject.DestroyImmediate(parentsObject);
            guiHandlerBase.ShowEditor();
        }

        public void CreateNewGUIClassWithTemplate(GUIManagerBase guiManager)
        {
            if (!CheckConditionCreateGUI(guiManager))
                return;

            if (guiManager.template == null)
            {
                EditorUtility.DisplayDialog("Notice", "Missing template !!!", "OK");
                return;
            }

            CreateGUIHandlerScript(guiManager);

            //Create GUI Class with template
            CloneScript(guiManager, MonoScript.FromMonoBehaviour(guiManager.template), guiManager.newGuiName);

            AssetDatabase.Refresh();
        }

        private void CloneScript(GUIManagerBase guiManager, MonoScript ms, string newName)
        {
            string scriptGuiFilePath_Source = AssetDatabase.GetAssetPath(ms);

            //Debug.LogError("scriptGuiFilePath_Source: " + scriptGuiFilePath_Source);
            //Debug.LogError("Script Name: " + ms.name);

            string scriptGuiFilePath_Dest = Application.dataPath + "/" + guiManager.destSourcePath + "/" + newName + ".cs";

            StreamWriter fileWriter = null;
            string[] fileData = File.ReadAllLines(scriptGuiFilePath_Source);

            try
            {
                fileWriter = new StreamWriter(scriptGuiFilePath_Dest, false);

                for (int i = 0; i < fileData.Length; i++)
                {
                    if (fileData[i].Contains(ms.name))
                    {
                        fileData[i] = fileData[i].Replace(ms.name, guiManager.newGuiName);
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
        }

        public void CreateNewGUIObjectWithTemplate(GUIManagerBase guiManager)
        {
            if (!CheckConditionCreateGUIObject(guiManager))
                return;

            string prefabFullName = "Assets/" + guiManager.destPrefabPath + "/" + guiManager.newGuiName + ".prefab";
            if (File.Exists(prefabFullName))
            {
                EditorUtility.DisplayDialog("Notice", "GUI prefab already exits !!!", "OK");
                return;
            }

            GameObject rootGUI = GameObject.Find("GUIRoot");

            if (rootGUI == null)
            {
                rootGUI = new GameObject();
                rootGUI.name = "GUIRoot";
                rootGUI.AddComponent<GlobalData>();
            }

            //Create GUI Handler object
            GameObject guiObject = new GameObject(guiManager.newGuiName + "Handler");
            guiObject.transform.position = Vector3.zero;
            guiObject.transform.SetParent(guiManager.transform);

            GUIHandlerBase guiHandlerBase =
                guiObject.AddComponent(Type.GetType(guiManager.newGuiName + "Handler, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")) as GUIHandlerBase;

            guiHandlerBase.guiCamera = guiManager.guiCamera;
            guiHandlerBase.guiName = guiManager.newGuiName;

            if (guiManager.listHandler.Count <= 0)
            {
                guiManager.listHandler.Add(guiHandlerBase);
            }
            else
            {
                if (guiManager.listHandler[guiManager.listHandler.Count - 1] == null)
                    guiManager.listHandler[guiManager.listHandler.Count - 1] = guiHandlerBase;
                else
                    guiManager.listHandler.Add(guiHandlerBase);
            }

            string prefabPath_Source = AssetDatabase.GetAssetPath(guiManager.template);
            //Debug.LogError("prefabPath_Source: " + prefabPath_Source);

            //FileUtil.CopyFileOrDirectory(prefabPath_Source, prefabFullName);
            AssetDatabase.CopyAsset(prefabPath_Source, prefabFullName);
            AssetDatabase.Refresh();

            //Get MonoScript of new GUI Class
            GameObject tmpGO = new GameObject("tempOBJ");
            GUIBase guiScript = tmpGO.AddComponent(Type.GetType(guiManager.newGuiName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")) as GUIBase;
            MonoScript guiSMonocript = MonoScript.FromMonoBehaviour(guiScript);
            DestroyImmediate(tmpGO);

            //Replace the sccrip to prefab, however keep the data on it
            GUIBase prefabObj = AssetDatabase.LoadAssetAtPath<GUIBase>(prefabFullName);
            SerializedObject so = new SerializedObject(prefabObj);
            SerializedProperty scriptProperty = so.FindProperty("m_Script");
            so.Update();
            scriptProperty.objectReferenceValue = guiSMonocript;
            so.ApplyModifiedProperties();

            guiHandlerBase.guiPrefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFullName);

            //Link handler and canvas gui
            guiHandlerBase.ShowEditor();
        }

        private bool CheckConditionCreateGUI(GUIManagerBase guiManager)
        {
            if (guiManager.newGuiName == null || guiManager.newGuiName == "")
            {
                EditorUtility.DisplayDialog("Notice", "Empty New GUI Name !!!", "OK");
                return false;
            }

            Type guiType = Type.GetType(guiManager.newGuiName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            Type handlerType = Type.GetType(guiManager.newGuiName + "Handler, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

            if (guiType != null || handlerType != null)
            {
                EditorUtility.DisplayDialog("Notice", "GUI Class already exits !!!", "OK");
                return false;
            }

            return true;
        }

        private bool CheckConditionCreateGUIObject(GUIManagerBase guiManager)
        {
            if (guiManager.newGuiName == null || guiManager.newGuiName == "")
            {
                EditorUtility.DisplayDialog("Notice", "Empty New GUI Name !!!", "OK");
                return false;
            }

            Type handlerType = Type.GetType(guiManager.newGuiName + "Handler, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            Type guiType = Type.GetType(guiManager.newGuiName + ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

            if (guiType == null || handlerType == null)
            {
                EditorUtility.DisplayDialog("Notice", "Missing GUI Class !!!", "OK");
                return false;
            }

            if (guiManager.transform.Find(guiManager.newGuiName))
            {
                EditorUtility.DisplayDialog("Notice", "GUI already exits !!!", "OK");
                return false;
            }

            return true;
        }

        //Create GUI Script
        public void CreateGUIScript(GUIManagerBase guiManager)
        {
            StreamWriter fileWriter = null;

            try
            {
                fileWriter = new StreamWriter(Application.dataPath + "/" + guiManager.destSourcePath + "/" + guiManager.newGuiName + ".cs", false);

                fileWriter.WriteLine("using UnityEngine;");
                fileWriter.WriteLine("using System.Collections;");
                fileWriter.WriteLine("using GFramework.GUI;");
                fileWriter.WriteLine("");
                fileWriter.WriteLine("public class " + guiManager.newGuiName + " : GUIBase");
                fileWriter.WriteLine("{");

                fileWriter.WriteLine("\tprivate Data data;");

                fileWriter.WriteLine("\tpublic override bool Show(params object[] @parameter)");
                fileWriter.WriteLine("\t{");

                fileWriter.WriteLine("\t\tif (parameter != null && parameter.Length > 0)");
                fileWriter.WriteLine("\t\t{");
                fileWriter.WriteLine("\t\t\tdata = parameter[0] as Data;");
                fileWriter.WriteLine("\t\t}");

                fileWriter.WriteLine("\t\treturn base.Show(@parameter);");
                fileWriter.WriteLine("\t}");
                fileWriter.WriteLine("");

                fileWriter.WriteLine("\tpublic override void Hide(params object[] @parameter)");
                fileWriter.WriteLine("\t{");
                fileWriter.WriteLine("\t\tbase.Hide(@parameter);");
                fileWriter.WriteLine("\t}");

                fileWriter.WriteLine("\tpublic class Data");
                fileWriter.WriteLine("\t{");
                fileWriter.WriteLine("\t}");

                fileWriter.WriteLine("}");

                fileWriter.Close();
            }
            catch (Exception ex)
            {
                if (fileWriter != null)
                    fileWriter.Close();
                Debug.LogError("Error:" + ex.Message);
            }
        }

        //Create GUI Handler Script
        public void CreateGUIHandlerScript(GUIManagerBase guiManager)
        {
            StreamWriter fileWriter = null;
            try
            {
                fileWriter = new StreamWriter(Application.dataPath + "/" + guiManager.destSourcePath + "/" + guiManager.newGuiName + "Handler.cs", false);
                fileWriter.WriteLine("using UnityEngine;");
                fileWriter.WriteLine("using System.Collections;");
                fileWriter.WriteLine("using GFramework.GUI;");
                fileWriter.WriteLine("");
                fileWriter.WriteLine("public class " + guiManager.newGuiName + "Handler : GUIHandlerBase");
                fileWriter.WriteLine("{");

                fileWriter.WriteLine("\tpublic override bool Show(params object[] @parameter)");
                fileWriter.WriteLine("\t{");
                fileWriter.WriteLine("\t\treturn base.Show(@parameter);");
                fileWriter.WriteLine("\t}");
                fileWriter.WriteLine("");

                fileWriter.WriteLine("\tpublic override void Hide(params object[] @parameter)");
                fileWriter.WriteLine("\t{");
                fileWriter.WriteLine("\t\tbase.Hide(@parameter);");
                fileWriter.WriteLine("\t}");



                fileWriter.WriteLine("}");

                fileWriter.Close();
            }
            catch (Exception ex)
            {
                if (fileWriter != null)
                    fileWriter.Close();
                Debug.LogError("Error:" + ex.Message);
            }
        }
    }
}
