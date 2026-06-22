using UnityEngine;
using System;
using System.Collections.Generic;

//Pool module v 1.5.0

namespace Watermelon
{
    /// <summary>
    /// Basic pool class. Contains pool settings and references to pooled objects.
    /// </summary>
    [Serializable]
    public class Pool
    {
        /// <summary>
        /// Pool name, use it get pool reference at PoolManager.
        /// </summary>
        public string name;

        /// <summary>
        /// Reference to object which shood be pooled.
        /// </summary>
        public GameObject objectToPool;
        /// <summary>
        /// List of multiple pool prefabs.
        /// </summary>
        public List<GameObject> objectsToPoolList = new List<GameObject>();
        /// <summary>
        /// List of multiple pool weights.
        /// </summary>
        public List<int> weightsList = new List<int>();
        /// <summary>
        /// Number of objects which be created be deffault.
        /// </summary>
        public int poolSize = 10;
        /// <summary>
        /// True means: if there is no available object, the new one will be added to a pool.
        /// Otherwise will be returned null.
        /// </summary>
        public bool willGrow = true;
        /// <summary>
        /// Type of pool.
        /// Single - classic pool with one object. Multiple - pool with multiple objects returned randomly using weights.
        /// </summary>
        public PoolType poolType = PoolType.Single;
        /// <summary>
        /// Custom objects container for this pool's objects.
        /// </summary>
        public Transform objectsContainer;
        /// <summary>
        /// List of pooled objects.
        /// </summary>
        public List<GameObject> pooledObjects;
        /// <summary>
        /// List of pooled objects for multiple pull.
        /// </summary>
        public List<List<GameObject>> multiPooledObjects;

#if UNITY_EDITOR
        /// <summary>
        /// Number of objects that where active at one time.
        /// </summary>
        public int maxItemsUsedInOneTime = 0;
#endif

        public enum PoolType
        {
            Single = 0,
            Multiple = 1,
        }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        public Pool()
        {
            name = string.Empty;
        }


        /// <summary>
        /// Returns reference to pooled object if it's currently available.
        /// </summary>
        /// <param name="activateObject">If true object will be set as active.</param>
        /// <returns>Pooled object or null if there is no available objects and new one can not be created.</returns>
        public GameObject GetPooledObject(bool activateObject = true)
        {
            return GetPooledObject(true, activateObject, false, Vector3.zero);
        }

        /// <summary>
        /// Returns reference to pooled object if it's currently available.
        /// </summary>
        /// <param name="position">Sets object to specified position.</param>
        /// <param name="activateObject">If true object will be set as active.</param>
        /// <returns>Pooled object or null if there is no available objects and new one can not be created.</returns>
        public GameObject GetPooledObject(Vector3 position, bool activateObject = true)
        {
            return GetPooledObject(true, activateObject, true, position);
        }

        public GameObject GetPooledObjectByName(string name, bool activateObject = true)
        {
            if (poolType == PoolType.Single)
            {
                return GetPooledObjectSingleType(true, activateObject, false, Vector3.zero);
            }

            return GetPooledObjectMultiType(name, true, activateObject, false, Vector3.zero);
        }

        /// <summary>
        /// Returns reference to pooled object if it's currently available.
        /// </summary>
        /// <param name="activateObject">If true object will be set as active.</param>
        /// <returns>Pooled object or null if there is no available objects and new one can not be created.</returns>
        public GameObject GetHierarchyPooledObject(bool activateObject = true)
        {
            return GetPooledObject(true, activateObject, false, Vector3.zero);
        }

        /// <summary>
        /// Returns reference to pooled object if it's currently available.
        /// </summary>
        /// <param name="position">Sets object to specified position.</param>
        /// <param name="activateObject">If true object will be set as active.</param>
        /// <returns>Pooled object or null if there is no available objects and new one can not be created.</returns>
        public GameObject GetHierarchyPooledObject(Vector3 position, bool activateObject = true)
        {
            return GetPooledObject(false, activateObject, true, position);
        }

        /// <summary>
        /// Internal implementation of GetPooledObject and GetHierarchyPooledObject methods.
        /// </summary>
        /// <param name="checkTypeActiveSelf">Which type of checking object's activation state is used: active self or active in hierarchy.</param>
        /// <param name="activateObject">If true object will be set as active.</param>
        /// <param name="position">Sets object to specified position.</param>
        /// <returns></returns>
        private GameObject GetPooledObject(bool checkTypeActiveSelf, bool activateObject, bool setPosition, Vector3 position)
        {
            if (poolType == PoolType.Single)
            {
                return GetPooledObjectSingleType(checkTypeActiveSelf, activateObject, setPosition, position);
            }
            else
            {
                return GetPooledObjectMultiType(checkTypeActiveSelf, activateObject, setPosition, position);
            }
        }

        /// <summary>
        /// Internal implementation of GetPooledObject and GetHierarchyPooledObject methods for Single type pool.
        /// </summary>
        /// <param name="checkTypeActiveSelf">Which type of checking object's activation state is used: active self or active in hierarchy.</param>
        /// <param name="activateObject">If true object will be set as active.</param>
        /// <param name="position">Sets object to specified position.</param>
        /// <returns></returns>
        private GameObject GetPooledObjectSingleType(bool checkTypeActiveSelf, bool activateObject, bool setPosition, Vector3 position)
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (pooledObjects[i] != null && (checkTypeActiveSelf ? !pooledObjects[i].activeSelf : !pooledObjects[i].activeInHierarchy))
                {
                    if (setPosition)
                    {
                        pooledObjects[i].transform.position = position;
                    }
                    pooledObjects[i].SetActive(activateObject);

                    return pooledObjects[i];
                }
            }

            if (willGrow)
            {
                GameObject inst = PoolManager.AddObjectToPoolSingleType(this);
                if (setPosition)
                {
                    inst.transform.position = position;
                }
                inst.SetActive(activateObject);

                return inst;
            }

            return null;
        }

        /// <summary>
        /// Internal implementation of GetPooledObject and GetHierarchyPooledObject methods for Multi type pool.
        /// </summary>
        /// <param name="checkTypeActiveSelf">Which type of checking object's activation state is used: active self or active in hierarchy.</param>
        /// <param name="activateObject">If true object will be set as active.</param>
        /// <param name="position">Sets object to specified position.</param>
        /// <returns></returns>
        private GameObject GetPooledObjectMultiType(bool checkTypeActiveSelf, bool activateObject, bool setPosition, Vector3 position)
        {
            int randomPoolIndex = 0;
            bool randomValueWasInRange = false;
            int randomValue = UnityEngine.Random.Range(0, 101);
            int currentValue = 0;

            for (int i = 0; i < weightsList.Count; i++)
            {
                currentValue += weightsList[i];

                if (randomValue <= currentValue)
                {
                    randomPoolIndex = i;
                    randomValueWasInRange = true;
                    break;
                }
            }

            if (!randomValueWasInRange)
            {
                Debug.LogError("[Pool Manager] Random value(" + randomValue + ") is out of weights sum range at pool: \"" + name + "\"");
            }

            Debug.Log("Random index: " + randomPoolIndex);

            List<GameObject> objectsList = multiPooledObjects[randomPoolIndex];

            for (int i = 0; i < objectsList.Count; i++)
            {
                if (objectsList[i] != null && (checkTypeActiveSelf ? !objectsList[i].activeSelf : !objectsList[i].activeInHierarchy))
                {
                    if (setPosition)
                    {
                        objectsList[i].transform.position = position;
                    }
                    objectsList[i].SetActive(activateObject);

                    return objectsList[i];
                }
            }

            if (willGrow)
            {
                GameObject inst = PoolManager.AddObjectToPoolMultiType(this, randomPoolIndex);
                if (setPosition)
                {
                    inst.transform.position = position;
                }
                inst.SetActive(activateObject);

                return inst;
            }

            return null;
        }

        private GameObject GetPooledObjectMultiType(string type, bool checkTypeActiveSelf, bool activateObject, bool setPosition, Vector3 position)
        {
            var obj = objectsToPoolList.Find(x => x.name == type);
            if (obj == null) return null;

            var poolIndex = objectsToPoolList.IndexOf(obj);
            if (poolIndex == -1) return null;

            List<GameObject> objectsList = multiPooledObjects[poolIndex];

            for (int i = 0; i < objectsList.Count; i++)
            {
                if (objectsList[i] != null && (checkTypeActiveSelf ? !objectsList[i].activeSelf : !objectsList[i].activeInHierarchy))
                {
                    if (setPosition)
                    {
                        objectsList[i].transform.position = position;
                    }
                    objectsList[i].SetActive(activateObject);

                    return objectsList[i];
                }
            }

            if (willGrow)
            {
                GameObject inst = PoolManager.AddObjectToPoolMultiType(this, poolIndex);
                if (setPosition)
                {
                    inst.transform.position = position;
                }
                inst.SetActive(activateObject);

                return inst;
            }

            return null;
        }

        /// <summary>
        /// Disables all active objects from this pool.
        /// </summary>
        public void ReturnToPoolEverything()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                pooledObjects[i].SetActive(false);
            }
        }
        public void ClearPool()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                GameObject.Destroy(pooledObjects[i]);
            }
            pooledObjects.Clear();
        }
    }
}