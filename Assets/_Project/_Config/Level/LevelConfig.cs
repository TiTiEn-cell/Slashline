using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Project Config/LevelConfig", order = 1)]
public class LevelConfig : ScriptableObject
{
    public List<LevelDatasConfig> LevelDatasConfig;

    public LevelDatasConfig GetDataConfigByLevel(int _level)
    {
        var maxLevel = MaxLevel();
        if (_level > maxLevel)
        {
            Debug.LogWarning($"Warning: Reach max level [{_level}/{maxLevel}], level would be random");
            _level = UnityEngine.Random.Range(30, maxLevel); //rd from level 30 to end
        }
        var level = LevelDatasConfig.Find(x => x.Level == _level);
        if (level == null)
        {
            Debug.LogError($"Error: Can not find this level: {_level}, return level 1");
            return LevelDatasConfig[0];
        }

        return level;
    }

    public int MaxLevel()
    {
        return LevelDatasConfig.Count;
    }
}


[Serializable]
public class LevelDatasConfig
{
    public int Level;
    public List<SubLevelConfig> SubLevelsConfig;

    public int TotalSubLevel()
    {
        if (SubLevelsConfig == null)
        {
            return 0;
        }
        return SubLevelsConfig.Count;
    }

    public SubLevelConfig GetSubLevel(int subLevel)
    {
        if (subLevel >= TotalSubLevel())
        {
            Debug.LogError($"Error: Can not find this sub level: {subLevel}, return sub level 0");
            return SubLevelsConfig[0];
        }

        return SubLevelsConfig[subLevel];
    }
}

[Serializable]
public class SubLevelConfig
{
    public int Level;
    public int Reward;
    public string MapDataID;
    public string MapDataLoadLink;
    public bool IsEnd = false;
}

[Serializable]
public class SubLevelConfigParse
{
    public int Level;
    public int Reward;
    public string MapDataID;
    public string MapDataLoadLink;
}