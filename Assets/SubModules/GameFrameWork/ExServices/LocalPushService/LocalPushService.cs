#if USE_MOBILE_PUSH_NOTIFICATION
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

using UnityEngine;
using GFramework.Utils;

namespace GFramework.Services
{
    [System.Serializable]
    public class LocalizationNotiData
    {
        public string id;
        public string title;
        public string content;
    }

    [System.Serializable]
    public class LocalizationNitification
    {
        public List<LocalizationNotiData> data = new List<LocalizationNotiData>();
    }

    public class LocalPushService : SingletonMono<LocalPushService>
    {
        public LocalizationNitification localizationNotification = new LocalizationNitification();

        public string CHANNEL_ID = "";
        public string CHANNEL_NAME = "CHANNEL NAME";
        public int ID_DAILY_1 = 1;
        public int ID_DAILY_2 = 2;
        public int ID_DAILY_3 = 3;
        public int ID_DAILY_4 = 4;
        public int ID_DAILY_5 = 5;
        public int ID_8h = 6;
        public int ID_12h = 7;
        public int ID_72h = 8;

        private bool isInited = false;

        public Action<int> OnNotificationReceiveByID;

        public void Init(LocalizationNitification config, Action<int> onNotificationReceiveByID)
        {
            if (isInited) return;

            if (config != null)
            {
                localizationNotification = config;
            }

            OnNotificationReceiveByID = onNotificationReceiveByID;
#if UNITY_ANDROID
            var channel = new AndroidNotificationChannel()
            {
                Id = CHANNEL_ID,
                Name = CHANNEL_NAME,
                Importance = Importance.Default,
                Description = "Generic notifications",
            };

            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            AndroidNotificationCenter.OnNotificationReceived -= OnNotificationReceived;
            AndroidNotificationCenter.OnNotificationReceived += OnNotificationReceived;

            var lastNoti = AndroidNotificationCenter.GetLastNotificationIntent();
            if (lastNoti != null)
            {
                AndroidNotificationCenter.CancelDisplayedNotification(lastNoti.Id);
                var id = lastNoti.Id;
                var channel_name = lastNoti.Channel;
                var notification = lastNoti.Notification;
                OnNotificationReceiveByID?.Invoke(id);
            }
#elif UNITY_IOS
            var lastNoti = iOSNotificationCenter.GetLastRespondedNotification();
            if (lastNoti != null)
            {
                var id = 0;
                if(!int.TryParse(lastNoti.Identifier, out id))
                {
                    id = 0;
                }
                OnNotificationReceiveByID?.Invoke(id);
            }

            iOSNotificationCenter.OnNotificationReceived -= OnNotificationReceived;
            iOSNotificationCenter.OnNotificationReceived += OnNotificationReceived;
#endif
            isInited = true;
        }

        public void CancelAllNotifications()
        {
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
            iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
        }

        private void CancelNotifications(int id)
        {
            Debug.Log("CancelNotifications");

#if UNITY_ANDROID
            AndroidNotificationCenter.CancelNotification(id);
#elif UNITY_IOS
            iOSNotificationCenter.RemoveScheduledNotification(id.ToString());
#endif

        }

#if UNITY_ANDROID
        private void OnNotificationReceived(AndroidNotificationIntentData data)
        {
            var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (notificationIntentData != null)
            {
                var id = notificationIntentData.Id;
                var channel = notificationIntentData.Channel;
                var notification = notificationIntentData.Notification;

                Debug.Log(string.Format("OnNotificationReceived -> Title: {0} Text: {1}", notification.Title, notification.Text));
            }
        }
#endif

#if UNITY_IOS
        private void OnNotificationReceived(iOSNotification data)
        {
            var notificationIntentData = iOSNotificationCenter.GetLastRespondedNotification();
            if (notificationIntentData != null)
            {
                var id = notificationIntentData.Identifier;
                var title = notificationIntentData.Title;
                var text = notificationIntentData.Body;

                Debug.Log(string.Format("OnNotificationReceived -> Title: {0} Text: {1}", title, text));
            }
        }
#endif

        public void CreateNotification(int id, string title, string content, DateTime fireTime, string data = "")
        {
            Debug.Log(string.Format("CreateNotification -> Title: {0} Text: {1}, fireTime: {2}", title, content, fireTime.ToString("dd/MM/yyyy HH:mm:ss")));

#if UNITY_ANDROID
            var notification = new AndroidNotification();
            notification.Title = title;
            notification.Text = content;
            notification.FireTime = fireTime;
            notification.IntentData = data;
            notification.LargeIcon = "icon_large";
            notification.SmallIcon = "icon_small";

            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, CHANNEL_ID, id);
#elif UNITY_IOS

            var notification = new iOSNotification()
            {
                Identifier = id.ToString(),
                Title = title,
                Body = content,
                Subtitle = title,
                ShowInForeground = true,
                CategoryIdentifier = CHANNEL_ID,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                Trigger = new iOSNotificationCalendarTrigger()
                {
                    Day = fireTime.Day,
                    Month = fireTime.Month,
                    Year = fireTime.Year,
                    Hour = fireTime.Hour,
                    Minute = fireTime.Minute,
                    Second = fireTime.Second,
                    Repeats = false
                }
            };

            iOSNotificationCenter.RemoveScheduledNotification(id.ToString());
            iOSNotificationCenter.ScheduleNotification(notification);
#endif
        }

        public LocalizationNotiData GetLocalizationNotification(string id)
        {
            if (localizationNotification == null) return null;
            if (localizationNotification.data == null) return null;

            return localizationNotification.data.Find(x => x.id.Equals(id));
        }

        // private void OnApplicationPause(bool pause)
        // {
        //     if (isInited == false) return;

        //     if (pause)
        //     {
        //         var hours_8 = GetLocalizationNotification("noti_8_hours");
        //         var hours_12 = GetLocalizationNotification("noti_12_hours");
        //         var hours_72 = GetLocalizationNotification("noti_72_hours");
        //         if (hours_8 != null) CreateNotification(ID_8h, hours_8.title, hours_8.content, DateTime.Now.AddHours(8));
        //         if (hours_12 != null) CreateNotification(ID_12h, hours_12.title, hours_12.content, DateTime.Now.AddHours(12));
        //         if (hours_72 != null) CreateNotification(ID_72h, hours_72.title, hours_72.content, DateTime.Now.AddDays(3));
        //     }
        // }

        public IEnumerator RequestAuthorization(Action<bool> OnFinish = null)
        {
#if UNITY_IOS
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            using (var req = new AuthorizationRequest(authorizationOption, true))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                };

                string res = "\n RequestAuthorization:";
                res += "\n finished: " + req.IsFinished;
                res += "\n granted :  " + req.Granted;
                res += "\n error:  " + req.Error;
                res += "\n deviceToken:  " + req.DeviceToken;

                if (req.IsFinished)
                {
                    OnFinish?.Invoke(req.IsFinished);
                }
                Debug.Log(res);
            }
#endif
            yield break;
        }
    }
}
#else
namespace GFramework.Services
{
    public class LocalPushService : SingletonMono<LocalPushService>
    {
        
    }
}
#endif