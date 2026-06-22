
using System;
using UnityEngine;
using GFramework.Utils;
using System.Collections;

namespace GFramework.GameData
{
    /// <summary>
    /// T1: this
    /// T2: UserData
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class UserDataServiceBase<T1, T2> : SingletonMono<T1> where T1 : MonoBehaviour where T2 : UserDataBase, new()
    {
        public const string USER_DATA_PREF = "USER_DATA_PREF";

        public Action OnUserDataChanged;

        protected T2 userData;
        public T2 UserData { get { return userData; } }

        public bool IsInited { get; protected set; }
        public int currentQuality { get; set; }

        protected bool isDirty = false;
        private float timer = 0f;
        private float saveInterval = 5f;
        private float totalTime = 0f;


        private bool isNewUser = false;
        public bool IsNewUser { get { return isNewUser; } }

        public float timeShowFullAds { get; set; }

        protected virtual bool GamePlayIsPlaying
        {
            get
            {
                return false;
            }
        }

        protected virtual void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                Save(true);
            }
        }

        protected virtual void Update()
        {
            timer += Time.deltaTime;
            if (timer > saveInterval)
            {
                if (!GamePlayIsPlaying)
                {
                    Save();
                }
                totalTime += timer;

                timer = 0f;
            }
        }

        protected virtual void CreateNewUserData()
        {
            userData = new T2();
            isNewUser = true;
        }

        public virtual IEnumerator Init()
        {
            userData = StorageDataBase.LoadData<T2>(USER_DATA_PREF);

            if (userData == null) CreateNewUserData();

            yield return new WaitForEndOfFrame();

            Debug.Log("-----> Init! " + JsonUtility.ToJson(userData));

            IsInited = true;
        }

        public virtual void Save(bool force = false)
        {
            if (userData == null) return;
            if (isDirty == false && force == false) return;

            isDirty = false;
            userData.SaveData<T2>(USER_DATA_PREF);

            Debug.Log("-----> save! " + JsonUtility.ToJson(userData));
        }

        public void SetDirty()
        {
            isDirty = true;
        }

    }
}

