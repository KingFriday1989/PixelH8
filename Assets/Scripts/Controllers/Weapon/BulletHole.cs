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

        private void Start()
        {
            Material material = ReturnMaterial(ID);
            projector.material = material;
            timer = Time.time + decalTime;
        }

        private void Update()
        {
            if (timer < Time.time)
            {
                    Destroy(gameObject);
            }
        }

        private Material ReturnMaterial(string id) 
        {
            if (ID != null)
            {
                var matList = materials.Find(x => x.ID == ID).material;
                var mat = matList[UnityEngine.Random.Range(0,matList.Count)];
                return mat;
            }
            return materials.Find(x => x.ID == "Default").material[0];
        }
    }
} 