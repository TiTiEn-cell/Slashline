#if USE_LOCALIZATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;

namespace GFramework.Services
{
    [System.Serializable]
    public class LocaleInfo {
        public string Id;
        public string Locale;
    }
    
    public class LocalizedManager : SingletonMono<LocalizedManager>
    {
        public LocalizationSettings LocalizedSettings;
        public List<LocaleInfo> LocaleList;
        public string CurrentLocale;

        private bool iWatingLocale;

        public static Dictionary<SystemLanguage, LocaleInfo> LOCALE_NAMES = new Dictionary<SystemLanguage, LocaleInfo>() {
            {SystemLanguage.English, new LocaleInfo() {Id = "EN", Locale = "English (en)"}},
            {SystemLanguage.Japanese,  new LocaleInfo() {Id = "JA", Locale = "Korean (ko)"}},
            {SystemLanguage.Korean,  new LocaleInfo() {Id = "KO", Locale = "Japanese (ja)"}},
            {SystemLanguage.Vietnamese,  new LocaleInfo() {Id = "VI", Locale = "Vietnamese (vi)"}}
        };

        public static string GetDefaultLocaleId {
            get {
                var currentLocale = (LocaleInfo)null;
                if(LOCALE_NAMES.TryGetValue(Application.systemLanguage, out currentLocale)) {
                    return currentLocale.Id;
                }
                else {
                    return LOCALE_NAMES[SystemLanguage.English].Id;
                }
            }
        }

        void Start() {
            LocalizedSettings.OnSelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        public IEnumerator Init(string localeId) {

            Debug.Log("LocalizationManager:: " + localeId);

            LocaleList = new List<LocaleInfo>();
            foreach (var item in LOCALE_NAMES) {
                LocaleList.Add(item.Value);
            }

            yield return LocalizedSettings.GetInitializationOperation();
            yield return new WaitForEndOfFrame();

            ChangeLocale(localeId);
            yield return new WaitForEndOfFrame();
        }

        public void ChangeLocale(string localeId) {
            LocalizedSettings.SetSelectedLocale(LocalizedSettings.GetAvailableLocales().GetLocale(localeId.ToLower()));
            CurrentLocale = localeId;
        }

        private void OnSelectedLocaleChanged(UnityEngine.Localization.Locale locale) {
            Debug.Log("OnSelectedLocaleChanged: " + locale.LocaleName);
            iWatingLocale = true;
        }
    }
}
#endif
