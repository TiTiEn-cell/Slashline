using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace GFramework.GUI
{
    /// <summary>
    /// Singleton base class
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        private static readonly T singleton = new T();

        /// <summary>
        /// Instance of singleton
        /// </summary>
        public static T instance
        {
            get
            {
                return singleton;
            }
        }
    }

    /// <summary>
    /// Global singleton for mono behavior object that exists through entire application
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [CLSCompliant(false)]
#pragma warning disable CS3021 // Type or member does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
#pragma warning restore CS3021 // Type or member does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    {
        private static T singleton;
        
        /// <summary>
        /// Awake method
        /// </summary>
        public virtual void Awake()
        {
            if (singleton == null || singleton == this)
            {
                singleton = (T)(MonoBehaviour)this;
                if (transform.parent == null)
                {
                    GameObject.DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                GameObject.Destroy(gameObject);
            }

            SceneManager.sceneLoaded += OnEventLevelWasLoaded;
        }

        public virtual void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnEventLevelWasLoaded;
        }

        /// <summary>
        /// Instance of singleton
        /// </summary>
        public static T instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = (T)FindObjectOfType(typeof(T));
                }

                return singleton;
            }
        }

        /// <summary>
        /// The event call after LoadLevel method was call
        /// </summary>
        /// <param name="level"></param>
        public virtual void OnEventLevelWasLoaded(Scene scn, LoadSceneMode mode)
        {
            // Remove duplicated instances
            T[] objs = GameObject.FindObjectsOfType<T>();
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] != singleton)
                {
                    GameObject.DestroyImmediate(objs[i].gameObject);
                }
            }
        }
    }
}