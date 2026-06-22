using System;
using System.Collections.Generic;
using GFramework.GameData;

[Serializable]
public class UserData : UserDataBase
{
    //Level

    public bool IsDisableAds = false;
    public int CurrentFakeLevel = 1;

    public string PlayerName;

    public Storage MyStorage = new Storage();
    public List<string> ListIAPPurchared = new List<string>();

    public UserData()
    {
        SetDefault();
    }

    // public List<Skill> ListSkill = new List<Skill>();
    public void SetDefault()
    {
        IsDisableAds = false;
        CurrentFakeLevel = 1;
        PlayerName = "Player";
        MyStorage = new Storage();
    }

}
