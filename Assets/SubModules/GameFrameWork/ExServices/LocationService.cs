using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace GFramework.Services
{
    public class LocationService : SingletonMono<LocationService>
    {
        public static readonly string COUNTRY_CODE_JAPAN = "JP";
        public static readonly string COUNTRY_CODE_CHINA = "CN";
        public static readonly string COUNTRY_CODE_VN = "VN";
        public static readonly string COUNTRY_CODE_ERROR = "error";

        [Serializable]
        public class Location
        {
            public string status;
            public string country;
            public string countryCode;

            public string timezone;

            public bool IsSuccess
            {
                get
                {
                    return string.IsNullOrEmpty(status) == false && status == "success";
                }
            }
        }

        [Serializable]
        public class WorldTimeData {
            public string utc_datetime;
            public string datetime;
        }

        [SerializeField]
        private string serverURL = "http://ip-api.com/json";

        [SerializeField]
        private string serverWebURL = "http://ip-api.com/json";

        [SerializeField]
        private string worldTimeURL = "http://worldtimeapi.org/api/timezone/";

        [SerializeField]
        private string worldTimeWebURL = "http://worldtimeapi.org/api/timezone/";

        [SerializeField]
        private string functionName;
        private string countryCode = String.Empty;
        private string languageCountryCode = String.Empty;

        private string timezone = string.Empty;
        private DateTime? curWorldTime;
        public DateTime? CurWorldTime {
            get {return curWorldTime;}
        }

        public Action OnGetWorldTimeCompletedAction;

        public bool isInit = false;
        public bool isWorldTimeSuccess = false;

        private void Start()
        {
            RefeshLocation();
        }

        public void RefeshLocation()
        {
            if (isInit == false)
            {
                languageCountryCode = Application.systemLanguage.ToCountryCode();
                StartCoroutine(GetLocation((data) =>
                {
                    if (data == null || data.IsSuccess == false)
                    {
                        Debug.Log("RefeshLocation1:: " + languageCountryCode);
                        countryCode = languageCountryCode;
                    }
                    else
                    {
                        Debug.Log("RefeshLocation2:: " + data.countryCode);
                        countryCode = data.countryCode;
                        timezone = data.timezone;
                        isInit = true;

                        StartCoroutine(IGetWorldTime((worldData)=> {
                            if(worldData != null) {
                                curWorldTime = DateTime.Parse(worldData.utc_datetime);
                                isWorldTimeSuccess = true;
                                OnGetWorldTimeCompletedAction?.Invoke();
                                OnGetWorldTimeCompletedAction = null;
                            }
                        }));
                    }
                }));
            }
            else {
                if(isWorldTimeSuccess == false) {
                    StartCoroutine(IGetWorldTime((worldData)=> {
                        if(worldData != null) {
                            curWorldTime = DateTime.Parse(worldData.utc_datetime);
                            isWorldTimeSuccess = true;
                            OnGetWorldTimeCompletedAction?.Invoke();
                        }
                    }));
                }
            }
        }

        public void GetLocation(Action<string> callback)
        {
            StartCoroutine(GetLocation((data) =>
            {
                if(data == null || data.IsSuccess == false)
                {
                    callback?.Invoke(languageCountryCode);
                }
                else
                {
                    callback?.Invoke(data.countryCode);
                }
            }));
        }

        public string GetCountryCode()
        {
            Debug.Log("GetCountryCode:: " + countryCode);

            return string.IsNullOrEmpty(countryCode) ? languageCountryCode : countryCode;
        }

        private IEnumerator GetLocation(Action<Location> onCallback)
        {

#if UNITY_WEBGL
            string url = string.Format("{0}{1}", serverWebURL, functionName);
#else
            string url = string.Format("{0}{1}", serverURL, functionName);
#endif

            UnityWebRequest webRequest = UnityWebRequest.Get(url);

            yield return webRequest.SendWebRequest();

            if (webRequest.isDone == false || webRequest.downloadHandler == null)
            {
                Debug.Log("Error While Sending: " + webRequest.error);
                onCallback?.Invoke(null);
                yield break;
            }

            string response = webRequest.downloadHandler.text;
            Debug.Log($"Received response from server: {response}");

            Location res = null;
            if(string.IsNullOrEmpty(response) == false) {
                try {
                    res = JsonUtility.FromJson<Location>(response);
                    Debug.Log("res:: " + res.countryCode);
                }
                catch (Exception e) {
                    Debug.LogError("GetLocation error:: " + e);
                }
            }

            onCallback?.Invoke(res);
        }

        private IEnumerator IGetWorldTime(Action<WorldTimeData> onCallback) {

#if UNITY_WEBGL
            string url = string.Format("{0}{1}", worldTimeWebURL, timezone);
#else
            string url = string.Format("{0}{1}", worldTimeURL, timezone);
#endif
            Debug.LogError("IGetWorldTime url:: " + url);

            UnityWebRequest webRequest = UnityWebRequest.Get(url);

            yield return webRequest.SendWebRequest();

            if (webRequest.isDone == false || webRequest.downloadHandler == null)
            {
                Debug.Log("Error While Sending: " + webRequest.error);
                onCallback?.Invoke(null);
                yield break;
            }

            var response = webRequest.downloadHandler.text;
            Debug.Log($"IGetWorldTime from server: {response}");

            WorldTimeData res = null;

            if(string.IsNullOrEmpty(response) == false) {
                try {
                    res = JsonUtility.FromJson<WorldTimeData>(response);
                    Debug.Log("IGetWorldTime:: " + res.utc_datetime);
                }
                catch (Exception e) {
                    Debug.LogError("IGetWorldTime error:: " + e);
                }
            }

            onCallback?.Invoke(res);
        }

        public static readonly Dictionary<SystemLanguage, string> COUTRY_CODES = new Dictionary<SystemLanguage, string>()
    {
        { SystemLanguage.Afrikaans, "ZA" },
        { SystemLanguage.Arabic    , "SA" },
        { SystemLanguage.Basque    , "US" },
        { SystemLanguage.Belarusian    , "BY" },
        { SystemLanguage.Bulgarian    , "BJ" },
        { SystemLanguage.Catalan    , "ES" },
        { SystemLanguage.Chinese    , "CN" },
        { SystemLanguage.Czech    , "HK" },
        { SystemLanguage.Danish    , "DK" },
        { SystemLanguage.Dutch    , "BE" },
        { SystemLanguage.English    , "US" },
        { SystemLanguage.Estonian    , "EE" },
        { SystemLanguage.Faroese    , "FU" },
        { SystemLanguage.Finnish    , "FI" },
        { SystemLanguage.French    , "FR" },
        { SystemLanguage.German    , "DE" },
        { SystemLanguage.Greek    , "JR" },
        { SystemLanguage.Hebrew    , "IL" },
        { SystemLanguage.Icelandic    , "IS" },
        { SystemLanguage.Indonesian    , "ID" },
        { SystemLanguage.Italian    , "IT" },
        { SystemLanguage.Japanese    , "JP" },
        { SystemLanguage.Korean    , "KR" },
        { SystemLanguage.Latvian    , "LV" },
        { SystemLanguage.Lithuanian    , "LT" },
        { SystemLanguage.Norwegian    , "NO" },
        { SystemLanguage.Polish    , "PL" },
        { SystemLanguage.Portuguese    , "PT" },
        { SystemLanguage.Romanian    , "RO" },
        { SystemLanguage.Russian    , "RU" },
        { SystemLanguage.SerboCroatian    , "SP" },
        { SystemLanguage.Slovak    , "SK" },
        { SystemLanguage.Slovenian    , "SI" },
        { SystemLanguage.Spanish    , "ES" },
        { SystemLanguage.Swedish    , "SE" },
        { SystemLanguage.Thai    , "TH" },
        { SystemLanguage.Turkish    , "TR" },
        { SystemLanguage.Ukrainian    , "UA" },
        { SystemLanguage.Vietnamese    , "VN" },
        { SystemLanguage.ChineseSimplified    , "CN" },
        { SystemLanguage.ChineseTraditional    , "CN" },
        { SystemLanguage.Unknown    , "US" },
        { SystemLanguage.Hungarian    , "HU" },
    };
    }

    public static class ISystemLanguageExtensions
    {
        public static string ToCountryCode(this SystemLanguage language)
        {
            string result;
            if (LocationService.COUTRY_CODES.TryGetValue(language, out result))
            {
                return result;
            }
            else
            {
                return LocationService.COUTRY_CODES[SystemLanguage.Unknown];
            }
        }
    }
}
