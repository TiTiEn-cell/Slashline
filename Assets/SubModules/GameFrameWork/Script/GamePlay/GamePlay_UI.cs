using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GamePlay_UI : MonoBehaviour
{
    [SerializeField] List<UIAnimControl> listUIAnimControls;

    private bool isShowed = false;

    public void InitGamePlayUI()
    {
        Debug.LogError("--> Init ui game play");
        //ShowUI();
    }

    public void ShowUI()
    {
        if (isShowed) return;
        isShowed = true;
        foreach (var animControl in listUIAnimControls)
        {
            animControl.Show();
        }
    }

    public void HideUI()
    {
        if (!isShowed) return;
        isShowed = false;
        foreach (var animControl in listUIAnimControls)
        {
            animControl.Hide();
        }
    }
}
