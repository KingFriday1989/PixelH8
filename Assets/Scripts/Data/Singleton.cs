using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelH8.Data
{
    public class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                    _instance = new GameObject().AddComponent<T>();

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null) Destroy(obj: this);
            DontDestroyOnLoad(target: this);
        }
    }
}