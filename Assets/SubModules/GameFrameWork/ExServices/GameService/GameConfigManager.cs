using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GFramework.Services;
using GFramework.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class GameConfigManager : SingletonMono<GameConfigManager>
{
    public bool IsUsingOnlineConfig;
    [HideInInspector] public bool IsDesignMode;

    public LevelConfig LevelsConfig;
    //public ShapeConfig ShapeConfig;
    //public GamePlayConfig GamePlayConfigs;
    //public ItemConfig ItemConfig;
    //public RewardConfig RewardConfig;
    //public ProgressionConfig ProgressionConfig;
    //public TaskConfig TaskConfig;

    [SerializeField] private string LevelConfig_GoogleSheetLink;

    public IEnumerator ReloadLevelDataConfig(Action onDownloadCompleted = null)
    {

        if (IsUsingOnlineConfig)
        {
            yield return GetLevelData(onDownloadCompleted);
        }
        else
        {
            onDownloadCompleted?.Invoke();
        }
    }

    /*public IEnumerator ReloadMapDataConfig(Action onDownloadCompleted = null)
    {
        if (IsUsingOnlineConfig)
        {
            yield return GetAllMapData(onDownloadCompleted);
        }
        else
        {
            onDownloadCompleted?.Invoke();
        }
    }*/


    public IEnumerator GetLevelData(Action onDownloadCompleted = null)
    {
       /* string urlLevelConfig = GetGoogleSheetLink(LevelConfig_GoogleSheetLink, "31005502");
        yield return GetText(urlLevelConfig, (isSuccess, data) =>
        {
            if (isSuccess)
            {
                string destination = "LevelDataConfig.text";
                string savePath = SaveFile(destination, data);
                var levels = CsvUtil.LoadObjects<SubLevelConfigParse>(savePath);
                LevelsConfig.LevelDatasConfig.Clear();

                var levelDatas = (LevelDatasConfig)null;
                var sublevelDatas = (SubLevelConfig)null;
                var currentLevel = 0;

                foreach (var item in levels)
                {
                    if (currentLevel != item.Level)
                    {
                        if (sublevelDatas != null)
                        {
                            sublevelDatas.IsEnd = true;
                        }

                        if (levelDatas != null)
                        {
                            LevelsConfig.LevelDatasConfig.Add(levelDatas);
                        }

                        levelDatas = new LevelDatasConfig()
                        {
                            Level = item.Level,
                            SubLevelsConfig = new List<SubLevelConfig>(),
                        };
                    }
                    currentLevel = item.Level;

                    sublevelDatas = new SubLevelConfig()
                    {
                        Level = item.Level,
                        MapDataID = item.MapDataID,
                        SongID = item.SongID,
                        MapDataLoadLink = item.MapDataLoadLink,
                        CameraBuffer = item.CameraBuffer,
                        CameraOffset_y = item.CameraOffset_y,
                        TimeSingPerCat = item.TimeSingPerCat,
                        Reward = item.Reward,
                    };
                    levelDatas.SubLevelsConfig.Add(sublevelDatas);
                }

                if (sublevelDatas != null)
                {
                    sublevelDatas.IsEnd = true;
                }

                if (levelDatas != null)
                {
                    LevelsConfig.LevelDatasConfig.Add(levelDatas);
                }
                Debug.LogError($"Success {destination}, in {savePath}");
                DebugLogService.Log($"----> Success {destination}, in {savePath}");
            }
        });

        // string urlRewardConfigConfig = GetGoogleSheetLink(LevelConfig_GoogleSheetLink, "1844349500");
        // yield return GetText(urlRewardConfigConfig, (isSuccess, data) =>
        // {
        //     if (isSuccess)
        //     {
        //         string destination = "RewardConfig.text";
        //         string savePath = SaveFile(destination, data);
        //         var rewardConfigs = CsvUtil.LoadObjects<RewardData>(savePath);
        //         RewardConfig.SetData(rewardConfigs);

        //         Debug.LogError($"Success {destination}, in {savePath}");
        //         DebugLogService.Log($"----> Success {destination}, in {savePath}");
        //     }
        // });

        string urlProgressionConfig = GetGoogleSheetLink(LevelConfig_GoogleSheetLink, "1451400145");
        yield return GetText(urlProgressionConfig, (isSuccess, data) =>
        {
            if (isSuccess)
            {
                string destination = "ProgressionConfig.text";
                string savePath = SaveFile(destination, data);
                var progressionDataConfigs = CsvUtil.LoadObjects<ProgressionConfigData>(savePath);
                ProgressionConfig.SetData(progressionDataConfigs);

                Debug.LogError($"Success {destination}, in {savePath}");
                DebugLogService.Log($"----> Success {destination}, in {savePath}");
            }
        });

        string urlRoomConfig = GetGoogleSheetLink(LevelConfig_GoogleSheetLink, "19286197");
        yield return GetText(urlRoomConfig, (isSuccess, data) =>
        {
            if (isSuccess)
            {
                string destination = "RoomConfig.text";
                string savePath = SaveFile(destination, data);
                var RoomDataConfigs = CsvUtil.LoadObjects<Room>(savePath);
                //TaskConfig.listRooms.Clear();
                TaskConfig.SetData(RoomDataConfigs);

                Debug.LogError($"Success {destination}, in {savePath}");
                DebugLogService.Log($"----> Success {destination}, in {savePath}");
            }
        });

        string urlTaskConfig = GetGoogleSheetLink(LevelConfig_GoogleSheetLink, "345070390");
        yield return GetText(urlTaskConfig, (isSuccess, data) =>
        {
            if (isSuccess)
            {
                string destination = "TaskConfig.text";
                string savePath = SaveFile(destination, data);
                var TaskDataConfigs = CsvUtil.LoadObjects<TaskDetailParse>(savePath);


                foreach (var item in TaskDataConfigs)
                {
                    var room = TaskConfig.GetRoom(item.RoomType);
                    if (room == null)
                    {
                        Debug.Log("Room null");
                        continue;
                    }
                    else if (room.RoomType == item.RoomType)
                    {
                        var taskDetail = new TaskDetail
                        {
                            furnitureName = item.FurnitureName,
                            cost = item.Cost,
                            roomType = item.RoomType,
                            TaskID = item.TaskID,
                        };
                        room.tasks.Add(taskDetail);
                    }
                }


                Debug.LogError($"Success {destination}, in {savePath}");
                DebugLogService.Log($"----> Success {destination}, in {savePath}");
            }
        });*/

#if UNITY_EDITOR
        /*EditorUtility.SetDirty(LevelsConfig);
        // EditorUtility.SetDirty(RewardConfig);
        EditorUtility.SetDirty(ProgressionConfig);
        EditorUtility.SetDirty(TaskConfig);*/
#endif
        onDownloadCompleted?.Invoke();
        Debug.LogError("Finish load level config");
        DebugLogService.Log("----> Finish load level config");
        yield return null;
    }

    /*private IEnumerator GetAllMapData(Action onDownloadCompleted = null)
    {
        Debug.LogError("Start load map data config, please wait!");
        DebugLogService.Log("---> Start load map data config, please wait!");

        var maxLevelNeedLoad = GameService.instance.MaxLevelReloadConfig;
        var minLevelNeedLoad = GameService.instance.MinLevelReloadConfig;
        var loadAll = maxLevelNeedLoad == 0 && minLevelNeedLoad == 0;
        var cancelLoad = maxLevelNeedLoad == -1 && minLevelNeedLoad == -1;
        if (loadAll)
        {
            foreach (var level in LevelsConfig.LevelDatasConfig)
            {
                yield return GetMapData(level);
            }
        }
        else
        {
            foreach (var level in LevelsConfig.LevelDatasConfig)
            {
                if (level.Level >= minLevelNeedLoad && level.Level <= maxLevelNeedLoad)
                {
                    yield return GetMapData(level);
                }
            }
        }

        onDownloadCompleted?.Invoke();
        Debug.LogError("Finish load map data config");
        DebugLogService.Log("----> Finish load map data config");
    }*/

    /*private IEnumerator GetMapData(LevelDatasConfig level)
    {
        foreach (var subLevel in level.SubLevelsConfig)
        {
            yield return GetText(subLevel.MapDataLoadLink, (success, data) =>
            {
                string destination = $"{subLevel.MapDataID}{FileExtension.TXT}";
                string savePath = SaveFile(destination, data);
                Debug.LogError($"Success load: {destination}, save in {savePath}");

#if UNITY_EDITOR

                var mapDataScriptable = Resources.Load<LevelDataDesign>("LevelDataDesign/" + subLevel.MapDataID);
                if (mapDataScriptable == null)
                {
                    mapDataScriptable = ScriptableObject.CreateInstance<LevelDataDesign>();
                    AssetDatabase.CreateAsset(mapDataScriptable, $"Assets/Resources/LevelDataDesign/{subLevel.MapDataID}.asset");
                    AssetDatabase.SaveAssets();
                }
                var levelDataParse = ParseMapData.TryParseLevelData(subLevel.MapDataID);
                mapDataScriptable.SetUpData(levelDataParse);
                // subLevel.LevelDataDesign = mapDataScriptable;
                EditorUtility.SetDirty(mapDataScriptable);
#elif UNITY_ANDROID
            var tryParseData = ParseMapData.TryParseLevelData(subLevel.MapDataID);
#endif
            });
        }
    }*/

    private string GetGoogleSheetLink(string idsheet, string idgid)
    {
        string url = string.Format("https://docs.google.com/spreadsheets/d/{0}/export?format=csv&id={0}&gid={1}", idsheet, idgid);
        return url;
    }

    private string SaveFile(string filename, string data)
    {
        var savePath = Path.Combine(Application.persistentDataPath, filename);

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        File.WriteAllText(savePath, data);

        return savePath;
    }

    IEnumerator GetText(string url, Action<bool, string> onResultCallback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
                onResultCallback?.Invoke(false, www.error);
            }
            else
            {
                onResultCallback?.Invoke(true, www.downloadHandler.text);
            }
        }
    }
}
