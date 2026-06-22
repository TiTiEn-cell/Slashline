using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

//Pool module v 1.5.0

namespace Watermelon
{
    [CustomEditor(typeof(PoolManager))]
    sealed internal class PoolManagerEditor : Editor
    {
        // private List<Pool> poolsList = new List<Pool>();
        // private List<int> poolsCacheDeltaList = new List<int>();
        // private List<PoolCache> poolsCacheList = new List<PoolCache>();

        // private SerializedProperty poolsListProperty;

        // private PoolManager poolManagerRef;
        // private Pool selectedPool;
        // private Pool newPool = null;

        // private bool isNameAllowed = true;
        // private bool isNameAlreadyExisting = false;
        // private bool showSettings = false;

        // private const string poolsListPropertyName = "pools";

        // private string searchText = string.Empty;
        // private string prevNewPoolName = string.Empty;
        // private string prevSelectedPoolName = string.Empty;

        // private GUIStyle boxStyle = new GUIStyle();
        // private GUIStyle headerStyle = new GUIStyle();
        // private GUIStyle bigHeaderStyle = new GUIStyle();
        // private GUIStyle centeredTextStyle = new GUIStyle();
        // private GUIStyle multiListLablesStyle = new GUIStyle();

        // private void OnEnable()
        // {
        //     poolManagerRef = (PoolManager)target;

        //     poolsListProperty = serializedObject.FindProperty(poolsListPropertyName);

        //     selectedPool = null;
        //     newPool = null;

        //     ReloadPoolManager();
        //     InitStyles();

        //     Undo.undoRedoPerformed += UndoCallback;
        // }

        // private void OnDisable()
        // {
        //     Undo.undoRedoPerformed -= UndoCallback;
        // }

        // private void UndoCallback()
        // {
        //     UpdatePools();
        // }

        // public void InitStyles()
        // {
        //     boxStyle.border = new RectOffset(5, 5, 4, 4);
        //     boxStyle.margin = new RectOffset(5, 5, 4, 4);
        //     boxStyle.padding = new RectOffset(5, 5, 3, 3);
        //     boxStyle.richText = true;
        //     boxStyle.alignment = TextAnchor.MiddleLeft;

        //     headerStyle.alignment = TextAnchor.MiddleCenter;
        //     headerStyle.fontStyle = FontStyle.Bold;
        //     headerStyle.fontSize = 12;

        //     bigHeaderStyle.alignment = TextAnchor.MiddleCenter;
        //     bigHeaderStyle.fontStyle = FontStyle.Bold;
        //     bigHeaderStyle.fontSize = 14;

        //     centeredTextStyle.alignment = TextAnchor.MiddleCenter;

        //     multiListLablesStyle.fontSize = 8;
        //     multiListLablesStyle.normal.textColor = new Color(0.3f, 0.3f, 0.3f);
        // }

        // public override void OnInspectorGUI()
        // {
        //     serializedObject.Update();

        //     // Control bar /////////////////////////////////////////////////////////////////////////////
        //     EditorGUILayout.BeginVertical(GUI.skin.box);

        //     EditorGUI.indentLevel++;

        //     showSettings = EditorGUILayout.Foldout(showSettings, "Settings");

        //     if (showSettings)
        //     {
        //         EditorGUI.BeginChangeCheck();

        //         if (EditorGUI.EndChangeCheck())
        //         {
        //             EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        //         }

        //         EditorGUI.BeginChangeCheck();


        //         poolManagerRef.objectsContainer = (GameObject)EditorGUILayout.ObjectField("Objects container: ", poolManagerRef.objectsContainer, typeof(GameObject), true);

        //         if (EditorGUI.EndChangeCheck())
        //         {
        //             EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        //         }

        //         EditorGUILayout.Space();
        //     }

        //     EditorGUI.indentLevel--;

        //     if (GUILayout.Button("Add pool", GUILayout.Height(30)))
        //     {
        //         AddNewPool();
        //     }

        //     // Pool creation bar //////////////////////////////////////////////////////////////////////////
        //     if (newPool != null)
        //     {
        //         EditorGUILayout.BeginVertical(GUI.skin.box);

        //         newPool.name = EditorGUILayout.TextField("Name: ", newPool.name);

        //         if (prevNewPoolName != newPool.name)
        //         {
        //             isNameAllowed = IsNameAllowed(newPool.name);
        //         }

        //         if (!isNameAllowed)
        //         {
        //             if (isNameAlreadyExisting)
        //             {
        //                 EditorGUILayout.HelpBox("Name already exists", MessageType.Warning);
        //             }
        //             else
        //             {
        //                 EditorGUILayout.HelpBox("Not allowed name", MessageType.Warning);
        //             }
        //         }

        //         GameObject prefab = (GameObject)EditorGUILayout.ObjectField("Prefab: ", newPool.objectToPool, typeof(GameObject), true);
        //         if (newPool.objectToPool != prefab && newPool.name == string.Empty)
        //         {
        //             newPool.name = prefab.name;
        //         }
        //         newPool.objectToPool = prefab;

        //         newPool.poolSize = EditorGUILayout.IntField("Pool size: ", newPool.poolSize);
        //         newPool.willGrow = EditorGUILayout.Toggle("Will grow: ", newPool.willGrow);

        //         if (isNameAllowed)
        //         {
        //             if (GUILayout.Button("Confirm"))
        //             {
        //                 ConfirmPoolCreation();

        //                 return;
        //             }
        //         }

        //         EditorGUILayout.EndVertical();
        //     }

        //     EditorGUILayout.EndVertical();


        //     // Pools displaying region /////////////////////////////////////////////////////////////////////
        //     EditorGUILayout.BeginVertical();

        //     EditorGUILayout.LabelField("Pool objects", headerStyle);

        //     GUILayout.BeginHorizontal();
        //     EditorGUI.BeginChangeCheck();

        //     searchText = EditorGUILayout.TextField(searchText, GUI.skin.FindStyle("ToolbarSeachTextField"));

        //     if (!string.IsNullOrEmpty(searchText))
        //     {
        //         if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
        //         {
        //             // Remove focus if cleared
        //             searchText = "";
        //             GUI.FocusControl(null);
        //         }
        //     }
        //     else
        //     {
        //         GUILayout.Button(GUIContent.none, GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty"));
        //     }
        //     if (EditorGUI.EndChangeCheck())
        //     {
        //         UpdatePools();
        //     }
        //     GUILayout.EndHorizontal();

        //     if (poolsList.Count == 0)
        //     {
        //         if (string.IsNullOrEmpty(searchText))
        //         {
        //             EditorGUILayout.HelpBox("There's no pools.", MessageType.Info);
        //         }
        //         else
        //         {
        //             EditorGUILayout.HelpBox("Pool \"" + searchText + "\" is not found.", MessageType.Info);
        //         }
        //     }
        //     else
        //     {
        //         for (int i = 0; i < poolsList.Count; i++)
        //         {
        //             Pool pool = poolsList[i];
        //             Rect clickRect = EditorGUILayout.BeginVertical(GUI.skin.box);
        //             EditorGUI.indentLevel++;
        //             if (selectedPool == null || pool.name != selectedPool.name)
        //             {
        //                 EditorGUILayout.LabelField(GetPoolName(i), centeredTextStyle);
        //             }
        //             else
        //             {
        //                 GUILayout.Space(5);

        //                 EditorGUI.BeginChangeCheck();

        //                 // name ///////////
        //                 string oldName = pool.name;
        //                 pool.name = EditorGUILayout.TextField("Name: ", pool.name);

        //                 // type ///////////
        //                 pool.poolType = (Pool.PoolType)EditorGUILayout.EnumPopup("Pool type:", pool.poolType);

        //                 // prefabs field ///////////
        //                 if (pool.poolType == Pool.PoolType.Single)
        //                 {
        //                     pool.objectToPool = (GameObject)EditorGUILayout.ObjectField("Prefab: ", pool.objectToPool, typeof(GameObject), true);
        //                 }
        //                 else
        //                 {
        //                     SerializedProperty list = poolsListProperty.GetArrayElementAtIndex(i).FindPropertyRelative("objectsToPoolList");

        //                     EditorGUILayout.PropertyField(list);
        //                     EditorGUI.indentLevel += 1;

        //                     if (pool.objectsToPoolList.Count != 0 && pool.objectsToPoolList.Count != pool.weightsList.Count)
        //                     {
        //                         pool.weightsList = new List<int>();
        //                         int averagePoints = 100 / pool.objectsToPoolList.Count;
        //                         int additionalPoints = 100 - averagePoints * pool.objectsToPoolList.Count;

        //                         for (int j = 0; j < pool.objectsToPoolList.Count; j++)
        //                         {
        //                             pool.weightsList.Add(averagePoints + (additionalPoints > 0 ? 1 : 0));
        //                             additionalPoints--;
        //                         }
        //                     }

        //                     if (list.isExpanded)
        //                     {
        //                         if (Event.current.type != EventType.DragPerform)
        //                         {
        //                             EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));

        //                             EditorGUILayout.BeginHorizontal();
        //                             EditorGUILayout.LabelField("objects", multiListLablesStyle, GUILayout.MaxHeight(10f));
        //                             GUILayout.Space(-25);
        //                             EditorGUILayout.LabelField("weights", multiListLablesStyle, GUILayout.Width(75), GUILayout.MaxHeight(10f));
        //                             EditorGUILayout.EndHorizontal();

        //                             for (int j = 0; j < pool.objectsToPoolList.Count; j++)
        //                             {
        //                                 EditorGUILayout.BeginHorizontal();

        //                                 pool.objectsToPoolList[j] = (GameObject)EditorGUILayout.ObjectField(pool.objectsToPoolList[j], typeof(GameObject), true);

        //                                 GUILayout.Space(-25);
        //                                 pool.weightsList[j] = EditorGUILayout.DelayedIntField(pool.weightsList[j], GUILayout.Width(75));


        //                                 EditorGUILayout.EndHorizontal();
        //                             }
        //                         }
        //                         EditorGUILayout.Space();
        //                     }
        //                     EditorGUI.indentLevel -= 1;

        //                 }


        //                 // pool size ///////////
        //                 int oldSize = pool.poolSize;
        //                 pool.poolSize = EditorGUILayout.IntField("Pool size: ", pool.poolSize);

        //                 // will grow toggle   |   objects parrent ///////////
        //                 pool.willGrow = EditorGUILayout.Toggle("Will grow: ", pool.willGrow);
        //                 pool.objectsContainer = (Transform)EditorGUILayout.ObjectField("Objects parrent", pool.objectsContainer, typeof(Transform), true);

        //                 if (EditorGUI.EndChangeCheck())
        //                 {
        //                     EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        //                 }

        //                 GUILayout.Space(5);

        //                 // delete button ///////////

        //                 if (GUILayout.Button("Delete"))
        //                 {
        //                     if (EditorUtility.DisplayDialog("This pool will be removed!", "Are you sure?", "Remove", "Cancel"))
        //                     {
        //                         DeletePool(pool);

        //                         EditorApplication.delayCall += delegate
        //                         {
        //                             EditorUtility.FocusProjectWindow();
        //                         };
        //                     }
        //                 }

        //                 GUILayout.Space(5);
        //             }

        //             EditorGUI.indentLevel--;
        //             EditorGUILayout.EndVertical();

        //             if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
        //             {
        //                 if (selectedPool == null || selectedPool != pool)
        //                 {
        //                     selectedPool = pool;
        //                     newPool = null;
        //                 }
        //                 else
        //                 {
        //                     selectedPool = null;
        //                 }
        //             }
        //         }
        //     }

        //     EditorGUILayout.EndVertical();

        //     if (GUI.changed)
        //     {
        //         EditorUtility.SetDirty(target);
        //     }

        //     serializedObject.ApplyModifiedProperties();
        // }

        // private string GetPoolName(int poolIndex)
        // {
        //     string poolName = poolsList[poolIndex].name;

        //     return poolName;
        // }

        // private void AddNewPool()
        // {
        //     newPool = new Pool();

        //     //isNameAllowed = IsNameAllowed(newPool.name);
        //     isNameAllowed = true; // to prevent warning message when just created pool (there will be new check on confirm method)
        // }

        // private void ConfirmPoolCreation()
        // {
        //     if (IsNameAllowed(newPool.name))
        //     {
        //         Undo.RecordObject(target, "Add pool");

        //         poolManagerRef.pools.Add(newPool);

        //         ReloadPoolManager(true);
        //         newPool = null;
        //         prevNewPoolName = string.Empty;

        //         searchText = "";
        //     }
        // }

        // private void DeletePool(Pool poolToDelete)
        // {
        //     Undo.RecordObject(target, "Remove");

        //     poolManagerRef.pools.Remove(poolToDelete);
        //     selectedPool = null;

        //     ReloadPoolManager();
        // }

        // private bool IsNameAllowed(string nameToCheck)
        // {
        //     if (nameToCheck.Equals(string.Empty))
        //     {
        //         isNameAllowed = false;
        //         isNameAlreadyExisting = false;
        //         return false;
        //     }

        //     //if (poolManagerRef.pools.IsNullOrEmpty())
        //     //{
        //     //    isNameAllowed = true;
        //     //    isNameAlreadyExisting = false;
        //     //    return true;
        //     //}

        //     if (poolManagerRef.pools.Find(x => x.name.Equals(nameToCheck)) != null)
        //     {
        //         isNameAllowed = false;
        //         isNameAlreadyExisting = true;
        //         return false;
        //     }
        //     else
        //     {
        //         isNameAllowed = true;
        //         isNameAlreadyExisting = false;
        //         return true;
        //     }
        // }

        // private void ReloadPoolManager(bool sortPool = false)
        // {
        //     poolsList.Clear();

        //     UpdatePools(sortPool);

        //     //UpdateCacheStateList();
        // }

        // private void UpdatePools(bool needToSort = false)
        // {
        //     if (needToSort)
        //     {
        //         poolManagerRef.pools.Sort((x, y) => x.name.CompareTo(y.name));
        //     }

        //     if (poolManagerRef.pools != null)
        //     {
        //         poolsList = poolManagerRef.pools.FindAll(x => x.name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
        //     }
        // }
    }
}