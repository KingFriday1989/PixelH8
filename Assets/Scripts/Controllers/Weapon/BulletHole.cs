using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System;

namespace PixelH8.Controllers.Weapons
{
    public class BulletHole : MonoBehaviour
    {
        [Serializable]
        public struct TextureList
        {
            public string ID;
            public List<Material> material;
        }
        [SerializeField] private DecalProjector projector;
        [SerializeField] public List<TextureList> materials;
        [SerializeField] private int decalTime = 30;
        [HideInInspector] public string ID;
        private float timer;

        private void OnEnable()
        {
            Material material = ReturnMaterial(ID);
            projector.material = material;
            timer = Time.time + decalTime;
        }

        private void FixedUpdate()
        {
            if (timer < Time.time)
            {
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }
        }

        private Material ReturnMaterial(string id)
        {
            if (ID != "")
            {
                var matList = materials.Find(x => x.ID == ID).material;
                if (matList.Count > 0)
                    return matList[UnityEngine.Random.Range(0, matList.Count)];
                else
                    return null;
            }
            else
            {
                Debug.Log("No Physics Material!");
                var matList = materials.Find(x => x.ID == "Default").material;
                if (matList.Count > 0)
                    return matList[UnityEngine.Random.Range(0, matList.Count)];
                else
                    return null;
            }
        }
    }
}