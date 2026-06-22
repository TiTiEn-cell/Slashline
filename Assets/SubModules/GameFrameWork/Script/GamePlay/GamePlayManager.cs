using System.Collections;
using DG.Tweening;
using GFramework.Services;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    //public static bool IsInstanceValid() { return Instance != null; }
    public static GamePlayManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    [SerializeField] private GamePlay_UI gamePlay_UI;
    //[SerializeField] private UICheatGP UICheatGP;

    private bool isClearAll = false;
    private int maxPerPool = 5;

    public void InitGamePlay()
    {
        //AdsManager.instance.StartCount = true;
        StartCoroutine(IStartGame());
    }

    public GamePlay_UI GetGamePlay_UI()
    {
        return gamePlay_UI;
    }

    /*public void InitCheat(bool active)
    {
        UICheatGP.InitCheat(active);
    }*/

    public void Processing(bool isWin)
    {
        StartCoroutine(IProcess(isWin));
    }

    public void BackToHome()
    {
        StartCoroutine(ITryBackHOme());
    }
        
    private IEnumerator IProcess(bool isWin)
    {
        AudioController.instance.StopBgMusic();
        gamePlay_UI.HideUI();
        yield return new WaitForSeconds(0.5f);

        if (isWin)
        {
            //yield return IShowInterstitalAds();



            Debug.LogError("---> Win");

        }
        else
        {
            Debug.LogError("---> Lose");
            UserDataService.instance.Save(true);


        }

    }


    private IEnumerator ITryBackHOme()
    {
        yield return new WaitForSeconds(0.1f);
        //yield return IShowInterstitalAds(isBackToHome: true);
        GameService.instance.ShowHome();
    }

    /*public IEnumerator IShowInterstitalAds(bool isBackToHome = false)
    {
        var intervalAds = GameService.instance.FirebaseRemoteConfig.AB_inter_capping;
        if (UserDataService.instance.CurrentLevel < GameService.instance.FirebaseRemoteConfig.AB_inter_start_level)
        {
            Debug.LogError("---> IS ads not pass level first show");
            yield break;
        }

        if (AdsManager.instance.GetCurrentTime < intervalAds)
        {
            Debug.LogError("---> IS ads not pass caping time");
            yield break;
        }

        if (!AdsMAXHelper.instance.IsInterstitialAdsReady())
        {
            Debug.LogError("---> IS ads not ready");
            yield break;
        }

        var isWaitingAds = true;
        AdsManager.instance.ShowInterstitialAds((isSuccess) =>
            {
                isWaitingAds = false;
            },
            isBackToHome ? AdsPlacement.BackToHome : AdsPlacement.EndGame);

        while (isWaitingAds)
        {
            yield return null;
        }
    }*/


    /*public void ClearAll()
    {
        if (isClearAll)
        {
            Debug.LogError("ClearAll before !!");
            return;
        }

        Debug.LogError("ClearAll called");

        var pools = Watermelon.PoolManager.instance.pools;
        foreach (var item in pools)
        {
            if (item != null && item.pooledObjects != null)
            {
                for (int i = 0; i < item.pooledObjects.Count;)
                {
                    if (i > maxPerPool)
                    {
                        Destroy(item.pooledObjects[i]);
                    }

                    if (item.pooledObjects[i] == null)
                    {
                        item.pooledObjects.RemoveAt(i);
                        continue;
                    }

                    var monoBehavior = item.pooledObjects[i].GetComponent<MonoBehaviour>();
                    if (monoBehavior != null) monoBehavior.CancelInvoke();

                    item.pooledObjects[i].transform.DOKill();
                    item.pooledObjects[i].transform.SetParent(Watermelon.PoolManager.instance.transform);
                    item.pooledObjects[i].SetActive(false);
                    item.pooledObjects[i].transform.position = Vector3.one * 5000;
                    i++;
                }
            }
        }

        isClearAll = true;
    }*/

    private IEnumerator IStartGame()
    {
        AudioController.instance.PlayBgMusic(SoundName.BGM);
        Debug.LogError("--> Init game play");
        yield return LevelManager.Instance.InitLevel();
        gamePlay_UI.InitGamePlayUI();
        GUIManager.instance.HideGUI(GUIName.GUI_Transition_ScreenHandler);
    }
}
