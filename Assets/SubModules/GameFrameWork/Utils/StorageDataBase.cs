using System;
using UnityEngine;

namespace GFramework.Utils
{
    [Serializable]
    public class StorageDataBase
    {
        private static string saveName = "StorageData";

        /// <summary>
        /// Convert this class to JSon string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string ToJSonString<T>() where T : StorageDataBase
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// Convert JSon string to this class structure
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJSonString<T>(string json) where T : StorageDataBase
        {
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// Save data store on this class to device with specific name provided by parametter "name"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public void SaveData<T>(string name) where T : StorageDataBase
        {
            string sJSonData = ToJSonString<T>();
            //Debug.LogError("sJSonData Save: " + sJSonData);
            PlayerPrefs.SetString(name, sJSonData);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load data store on the device to this class structure with specific name provided by parametter "name"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T LoadData<T>(string name) where T : StorageDataBase
        {
            string sJSonData = "";
            if (PlayerPrefs.HasKey(name))
            {
                sJSonData = PlayerPrefs.GetString(name);
                //Debug.LogError("sJSonData Load: " + sJSonData);
                return StorageDataBase.FromJSonString<T>(sJSonData);
            }

            return null;
        }

        /// <summary>
        /// Save data store on this class to device with default name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SaveData<T>() where T : StorageDataBase
        {
            SaveData<T>(saveName);
        }

        /// <summary>
        /// Load data store on the device to this class structure with default name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T LoadData<T>() where T : StorageDataBase
        {
            return LoadData<T>(saveName);
        }
    }
}