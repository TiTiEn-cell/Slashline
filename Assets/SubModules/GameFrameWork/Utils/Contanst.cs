using System;

public static class Contanst
{
    public const int ON = 1;
    public const int OFF = 0;

}

public static class SceneName
{
    public const string Initial = "Initial";
    public const string GamePlay = "GamePlay";
    public const string Main = "Main";
}

public class PlayerPrefKeys
{
    public static string EnableMusic = "EnableMusic";
    public static string EnableSoundFx = "EnableSoundFx";
    public static string EnableHaptic = "EnableHaptic";
}

public class FileExtension
{
    public static string TXT = ".txt";
}

public static class AnimName
{
    public const string ButtonSettingsOn = "on";
    public const string ButtonSettingsOff = "off";

}


#region ENUM

public enum GameState
{
    Tututorial,
    Ready,
    Playing,
    Pausing,
    End,
}


[Serializable]
public enum Placement
{
    Home = 1,
    GamePlay = 2,
}

[Serializable]
public enum ItemID
{
    MONEY = 1,
}

[Serializable]
public enum PlayerAnimationIndex : byte
{
    Movement = 0,
    Idle = 1,

    none = byte.MaxValue
}

#endregion



