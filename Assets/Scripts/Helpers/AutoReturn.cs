using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelH8.Helpers
{
    public class AutoReturn : MonoBehaviour
    {
        public bool Timed = false;
        public float returnDelay;
        private float startTime;
        private bool returned;

        void OnEnable()
        {
            startTime = Time.time;
        }
        void OnDisable()
        {
            if(!returned)
                ObjectPoolManager.ReturnObjectToPool(gameObject);
        }
        
        void FixedUpdate()
        {
            if (Timed && Time.time >= startTime + returnDelay)
            {
                returned = true;
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
                
        }
    }
}

