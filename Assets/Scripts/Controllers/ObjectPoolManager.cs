using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
    {
        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

        private static GameObject _objectPoolEmptyHolder;

        private static Dictionary<string, GameObject> _poolFolders = new Dictionary<string, GameObject>();

        private void Awake()
        {
            _objectPoolEmptyHolder = new GameObject("Pooled Objects");
        }

        /// <summary>
        /// To spawn objects in the scene stored in a "folder" gameobject to not clog the hierarchy
        /// </summary>
        /// <param name="poolType"> set to the "folder" you want to store</param>
        /// <returns></returns>
        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LoopupString == objectToSpawn.name);

            // If the pool doesn't exist, create it
            if (pool == null)
            {
                pool = new PooledObjectInfo() { LoopupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }

            // Check if there are any inactive objects in the pool

            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (spawnableObj == null)
            {
                // If there are no inactivate objects, create a new one
                spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
                string goName = spawnableObj.name.Replace("(Clone)", string.Empty);

                // If no parent folder, create a new one with same name and object to spawn
                if (_poolFolders.ContainsKey(goName))
                {
                    spawnableObj.transform.SetParent(_poolFolders[goName].transform);
                }
                else
                {
                    GameObject parentObject = new GameObject(goName);
                    parentObject.transform.SetParent(_objectPoolEmptyHolder.transform);
                    _poolFolders.Add(goName, parentObject);
                    spawnableObj.transform.SetParent(_poolFolders[goName].transform);
                }
            }
            else
            {
                // If there is an inactive object, reactive it
                spawnableObj.transform.position = spawnPosition;
                spawnableObj.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;
        }

        /// <summary>
        /// To spawn objects in the scene linked to a parent object
        /// </summary>
        /// <returns></returns>
        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform)
        {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LoopupString == objectToSpawn.name);

            // If the pool doesn't exist, create it
            if (pool == null)
            {
                pool = new PooledObjectInfo() { LoopupString = objectToSpawn.name };
                ObjectPools.Add(pool);
            }

            // Check if there are any inactive objects in the pool

            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (spawnableObj == null)
            {
                // If there are no inactivate objects, create a new one
                spawnableObj = Instantiate(objectToSpawn, parentTransform);
            }
            else
            {
                // If there is an inactive object, reactive it
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;
        }

        public static void ReturnObjectToPool(GameObject obj)
        {
            string goName = obj.name.Replace("(Clone)", string.Empty);

            PooledObjectInfo pool = ObjectPools.Find(p => p.LoopupString == goName);

            if (pool == null)
            {
                Debug.LogWarning("Trying to release an object that is not pooled: " + obj.name);
            }
            else
            {
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }
    }

    public class PooledObjectInfo
    {
        public string LoopupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();
    }
