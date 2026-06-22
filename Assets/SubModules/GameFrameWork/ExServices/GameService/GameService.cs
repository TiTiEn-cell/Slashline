using System;
using System.Collections;
using System.Collections.Generic;
using GFramework.Services;
using GFramework.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Watermelon;

public class GameService : SingletonMono<GameService>
{
    public bool IsStartLoadGamePlay { get; set; }
    public List<string> CurPoolList { get; set; }

    public bool IsDebug = true;
    public Placement CurrentPlacement;

    private float progress = 0;
    private float progressMax;
    public int MaxLevelReloadConfig = 1;
    public int MinLevelReloadConfig = 1;

    private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();


    private List<int> ListGUINotAllowClose = new List<int>()
    {

    };

    void Start()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = IsDebug;
#endif

        GUIManager.instance.ShowGUI(GUIName.GUI_LoadingGame_ScreenHandler, new LoadingGame_Screen.Data() { GetProgress = () => progress });
        StartCoroutine(IServiceProcessing());
    }

    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime;
        progress = Mathf.Clamp01(Mathf.Clamp(progress, 0, progressMax));
    }

    private IEnumerator IServiceProcessing()
    {
        progressMax = 0.1f;
#if UNITY_EDITOR
        // yield return GameConfigManager.instance.ReloadLevelConfig();
#endif

        progressMax = 0.2f;
         //yield return InitFireBase();

        progressMax = 0.25f;
         //yield return ICheckRemoteConfig();

        progressMax = 0.3f;
         //yield return ICheckUpdateVersion();

        progressMax = 0.35f;
         //InitAppFlyer();

        progressMax = 0.4f;
        MessageBus.Instance.Initialize();
        AudioController.instance.InitHaptic();

        progressMax = 0.5f;
        yield return UserDataService.Instance.Init();

        progressMax = 0.6f;
        //IAPManager.Instance.Init();
        //InitAdsMax();

        progressMax = 0.7f;
        // InitGGAds();

        progressMax = 0.8f;

        progressMax = 0.9f;

        InitPoolManager();
        yield return new WaitForSeconds(1f);
        progressMax = 1f;
        CurrentPlacement = Placement.Home;
        //yield return CheckShowOpenAppAds();
        yield return IShowTransition();
        GUIManager.instance.ShowGUI(GUIName.GUI_Main_ScreenHandler);
        GUIManager.instance.HideGUI(GUIName.GUI_LoadingGame_ScreenHandler);
        GUIManager.instance.HideGUI(GUIName.GUI_Transition_ScreenHandler);
    }

    public void PlayGame(bool isReplay = false)
    {
        Debug.LogError("--> Play game");
        ShowGamePlay(isReplay);
    }

    public void ShowBannerAds()
    {
        #if USE_ADS
        if (UserDataService.instance.CurrentLevel > FirebaseRemoteConfig.AB_inter_start_level)
        {
            AdsManager.instance.ShowBannerAds();
        }
        #endif
    }

    /*public void ReloadLevelConfig()
    {
        StartCoroutine(IReloadLevelDataConfig());
    }*/

    /*IEnumerator IReloadLevelDataConfig()
    {
        //GUIManager.instance.ShowGUI(GUIName.GUI_LoadingAds_ScreenHandler);
        yield return GameConfigManager.instance.ReloadLevelConfig(() =>
        {
            //GUIManager.instance.HideGUI(GUIName.GUI_LoadingAds_ScreenHandler);
        });
    }*/

    /*public void ReloadMapDataConig()
    {
        StartCoroutine(IReloadMapDataConfig());
    }*/


    /*IEnumerator IReloadMapDataConfig()
    {
        DebugLogService.instance.EnableDebugLog();
        //GUIManager.instance.ShowGUI(GUIName.GUI_LoadingAds_ScreenHandler);
        yield return GameConfigManager.instance.ReloadMapDataConfig(() =>
        {
            //GUIManager.instance.HideGUI(GUIName.GUI_LoadingAds_ScreenHandler);
            DebugLogService.instance.DisableDebuLog();
            ShowHome();
        });
    }*/

    public void SetMaxLevelReloadConfig(int level)
    {
        MaxLevelReloadConfig = level;
    }

    public int GetMaxLevelReloadConfig()
    {
        return MaxLevelReloadConfig;
    }

    public void SetMinLevelReloadConfig(int level)
    {
        MinLevelReloadConfig = level;
    }

    public int GetMinLevelReloadConfig()
    {
        return MinLevelReloadConfig;
    }

    private void InitAdsMax()
    {
#if USE_MAX
        AdsManager.instance.IsDisableAds = !UIConfig.IsEnableAds || UserDataService.Instance.IsDisableAds;
        AdsManager.instance.OnShowUILoadingAction = (isShow) =>
               {
                   if (isShow) GUIManager.instance.ShowGUI(GUIName.GUI_LoadingAds_ScreenHandler);
                   else GUIManager.instance.HideGUI(GUIName.GUI_LoadingAds_ScreenHandler);
               };
        AdsManager.Instance.Init(
                   key: FirebaseRemoteConfig.AdsMaxData.Key,
                   rewardId: FirebaseRemoteConfig.AdsMaxData.RewardId,
                   interstitialId: FirebaseRemoteConfig.AdsMaxData.InterstitialId,
                   bannerId: FirebaseRemoteConfig.AdsMaxData.BannerId,
                   userId: AppsFlyerService.instance.GetAppsFlyerId()
               );
#endif
    }

    private void InitGGAds()
    {
#if USE_GOOGLE_MOBILE_ADS
        GoogleAdsManager.instance.Init(new GoogleAdsConfig()
        {
            EnableOpenAds = !IsDebug && FirebaseRemoteConfig.GoogleMobileAdsData.EnableOpenAds,
            EnableOpenAds = FirebaseRemoteConfig.GoogleMobileAdsData.EnableOpenAds,
            OpenAdsId = FirebaseRemoteConfig.GoogleMobileAdsData.OpenAdsId,
            OpenAdsCapingTime = FirebaseRemoteConfig.GoogleMobileAdsData.OpenAdsCapingTime,

            CollapsibleAdsId = FirebaseRemoteConfig.GoogleMobileAdsData.CollapsibleAdsId,
            CollapsibleAdsCapingTime = FirebaseRemoteConfig.GoogleMobileAdsData.CollapsibleAdsCapingTime,
        });
#endif
    }



    #region Start game play
    private void ShowGamePlay(bool isReplay)
    {
        if (IsStartLoadGamePlay)
        {
            return;
        }

        IsStartLoadGamePlay = true;

        StartLoadGamePlay();
    }

    public void ShowHome()
    {
        StartCoroutine(IShowHome());
    }

    public void ShowSettings()
    {
        //GUIManager.instance.ShowGUI(GUIName.GUI_Settings_PopupHandler);
    }

    private void StartLoadGamePlay()
    {
        StartCoroutine(IStartLoadGamePlay());
    }

    private void InitPoolManager()
    {
        PoolManager.instance.Init();
    }


    private IEnumerator IStartLoadGamePlay()
    {
        yield return IShowTransition();
        GUIManager.instance.CloseAll(ListGUINotAllowClose);

        //TryClearGamePlay();

        /*try
        {
            CreatePool();
        }
        catch (Exception ex)
        {
            Debug.LogError("create_pool:: " + ex.Message);
            ShowHome();
            yield break;
        }*/

        yield return StartCoroutine(ILoadScene(SceneName.GamePlay));
        GamePlayManager.Instance.InitGamePlay();
        CurrentPlacement = Placement.GamePlay;
        IsStartLoadGamePlay = false;
    }

    /*private bool TryClearGamePlay()
    {
        bool needClear = false;
        if (SceneManager.GetActiveScene().name == SceneName.GamePlay && GamePlayManager.IsInstanceValid())
        {
            GamePlayManager.Instance.ClearAll();
            needClear = true;
        }
        return needClear;
    }*/

    #endregion

    private IEnumerator IShowHome()
    {
        CurrentPlacement = Placement.Home;
#if USE_ADS
        AdsManager.instance.StartCount = false;
#endif

        yield return IShowTransition();
        GUIManager.instance.CloseAll(ListGUINotAllowClose);

        //TryClearGamePlay();

        yield return ILoadScene(SceneName.Main, () =>
        {
            GUIManager.instance.ShowGUI(GUIName.GUI_Main_ScreenHandler);
            GUIManager.instance.HideGUI(GUIName.GUI_Transition_ScreenHandler);
        });
    }



    private IEnumerator IShowTransition()
    {
        GUIManager.instance.ShowGUI(GUIName.GUI_Transition_ScreenHandler);
        yield return new WaitForSeconds(0.35f);
    }

    private IEnumerator ILoadScene(string sceneName, Action onCompleteCallback = null)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        onCompleteCallback?.Invoke();
    }

    /*IEnumerator CheckShowOpenAppAds()
    {
#if USE_GOOGLE_MOBILE_ADS
        if (FirebaseRemoteConfig.GoogleMobileAdsData.EnableOpenAds && UserDataService.instance.CurrentLevel > FirebaseRemoteConfig.OpenAdsFirstShowLevel)
        {
            yield return GoogleAdsManager.instance.IShowAppOpenAd();
        }
#endif
        yield return null;
    }


    IEnumerator InitFireBase()
    {
#if USE_FIREBASE_REMOTE_CONFIG
        FirebaseManager.instance.Init();
        yield return StartCoroutine(ICheckInitFirebase());
#endif
        yield return null;
    }


    private IEnumerator ICheckInitFirebase()
    {
        while (FirebaseManager.instance.IsInitialized == false)
        {
            yield return waitForEndOfFrame;
        }
    }

    private IEnumerator ICheckRemoteConfig()
    {
        var time = Time.realtimeSinceStartup;

        //check timeout
        while (Time.realtimeSinceStartup - time < 5)
        {
            if (FirebaseManager.instance.IsRemoteConfigFetchComplete || Application.internetReachability == NetworkReachability.NotReachable)
            {
                break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }

        FirebaseRemoteConfig = FirebaseManager.instance.ParseConfig<FirebaseConfig>();

#if UNITY_EDITOR

        Debug.LogError("---> FirebaseRemoteConfig is generated by default");
        FirebaseRemoteConfig = new FirebaseConfig();
        // if (FirebaseManager.instance.IsRemoteConfigFetchComplete)
        {
            FileUtilities.SaveTextFile(JsonUtility.ToJson(FirebaseRemoteConfig), "configs/firebaseconfig.txt", false, true);
            UnityEditor.AssetDatabase.Refresh();
        }
#endif

    }

    private IEnumerator ICheckUpdateVersion()
    {
#if USE_FIREBASE_REMOTE_CONFIG
        if (FirebaseRemoteConfig != null && FirebaseRemoteConfig.IsReviewing == false)
        {
            var isWaitingCompleted = true;
            //check forceUpdate
            if (FirebaseRemoteConfig.UpdateVersion.ForceUpdate.GetValueGreaterEqualTo(Application.version))
            {
                isWaitingCompleted = false;
                GUIManager.instance.ShowGUI(GUIName.GUI_MessagePopupHandler, new MessagePopup.Data()
                {
                    TitleId = "NEW UPDATE",
                    MessageId = "Update the game now for more conten!",
                    AutoClose = false,
                    OnOkAction = () =>
                    {
                        Application.OpenURL(FirebaseRemoteConfig.UpdateVersion.GetUpdateURL());
                    }
                });
            }
            else if (FirebaseRemoteConfig.UpdateVersion.NotifyUpdate.GetValueGreaterEqualTo(Application.version))
            {
                isWaitingCompleted = false;
                GUIManager.instance.ShowGUI(GUIName.GUI_MessagePopupHandler, new MessagePopup.Data()
                {
                    TitleId = "UPDATE",
                    MessageId = "Update the game now for more conten!",
                    AutoClose = false,
                    OnOkAction = () =>
                    {
                        Application.OpenURL(FirebaseRemoteConfig.UpdateVersion.GetUpdateURL());
                    },
                    OnCloseAction = () =>
                    {
                        isWaitingCompleted = true;
                    }
                });
            }

            while (isWaitingCompleted == false)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
#endif
        yield return null;
    }

    private void InitAppFlyer()
    {
#if USE_APPSFLYER
        Debug.LogError("---> init af with key: " + FirebaseRemoteConfig.AppsFlyerData.AppKey);
        AppsFlyerService.instance.Init(
                       devKey: FirebaseRemoteConfig.AppsFlyerData.AppKey,
                       appId: FirebaseRemoteConfig.AppsFlyerData.AppId
               );
#endif
    }*/


    private void CreatePool()
    {
        CurPoolList ??= new List<string>();
        var listPoolRemove = new List<string>();

        if (CurPoolList != null)
        {
            listPoolRemove.AddRange(CurPoolList);
        }

        CurPoolList.Clear();

        var poolList = new List<Pool>();

        /*// pool for passenger
        var passengerAsset = GameConfigManager.instance.ItemConfig.GetPassengerAsset();
        foreach (var asset in passengerAsset.ListPassengerDefine)
        {
            if (asset == null)
            {
                Debug.LogError("---> something null, recheck here");
                continue;
            }

            poolList.Add(new Pool()
            {
                name = $"{Contanst.PoolObjectName.Passenger}_{asset.ItemColor}",
                poolType = Pool.PoolType.Single,
                objectToPool = asset.ItemTransform.gameObject,
                poolSize = 1,
                willGrow = true
            });
        }

        var jetPack = GameConfigManager.instance.ItemConfig.GetJetPackPrefab();
        if (jetPack != null)
        {
            poolList.Add(new Pool()
            {
                name = Contanst.PoolObjectName.JetPack,
                poolType = Pool.PoolType.Single,
                objectToPool = jetPack.gameObject,
                poolSize = 2,
                willGrow = true
            });
        }

        var effectHit = GameConfigManager.instance.EffectAssetConfig.GetEffectAssetByName(EffectName.Effect_Hit_Smoke);
        if (effectHit != null)
        {
            poolList.Add(new Pool()
            {
                name = EffectName.Effect_Hit_Smoke,
                poolType = Pool.PoolType.Single,
                objectToPool = effectHit.gameObject,
                poolSize = 5,
                willGrow = true
            });
        }
        // off emoji
        var emojiPrefab = GameConfigManager.instance.EmojiConfig.EmojiPrefab;
        if (emojiPrefab != null)
        {
            poolList.Add(new Pool()
            {
                name = Contanst.PoolObjectName.Emoji,
                poolType = Pool.PoolType.Single,
                objectToPool = emojiPrefab.gameObject,
                poolSize = 5,
                willGrow = true
            });
        }*/


        // Debug.LogError("------------------ REMOVE POOL ---------------");
        foreach (var item in poolList)
        {
            //Debug.LogError(item.name);
            listPoolRemove.Remove(item.name);
            CurPoolList.Add(item.name);
        }

        foreach (var item in listPoolRemove)
        {
            //Debug.LogError(item);
            PoolManager.instance.RemovePool(item);
        }
        // Debug.LogError("------------------ END REMOVE POOL ---------------");

        // Debug.LogError("------------------ ADD POOL ---------------");
        foreach (var item in poolList)
        {
            PoolManager.instance.InitializePool(item);
        }
        // Debug.LogError("------------------ END ADD POOL ---------------");
    }

}
