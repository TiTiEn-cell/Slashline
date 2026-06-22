using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Singleton
    public static LevelManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }
    #endregion
    public IEnumerator InitLevel()
    {
        Debug.LogError("--> Init level manager");
       yield return null;
    }

}

