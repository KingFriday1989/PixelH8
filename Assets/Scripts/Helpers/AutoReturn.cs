using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelH8.Helpers
{
    public class AutoReturn : MonoBehaviour
    {
        public float returnDelay;
        private float startTime;

        void Start()
        {
            startTime = Time.time;
        }
        void Update()
        {
            if (startTime < Time.time - returnDelay * 4)
                ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
    }
}

