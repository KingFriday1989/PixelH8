using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelH8.Data
{
    [CreateAssetMenu(fileName = "WeaponContainer", menuName = "PixelH8/WeaponContainer")]
    public class WeaponContainer : ScriptableObject
    {
        public static WeaponContainer instance;
        [System.Serializable]
        public struct WeaponDirectory
        {
            public List<WeaponGroup> weaponGroups;
        }

        [System.Serializable]
        public struct WeaponGroup
        {
            public string ID;
            public List<WeaponList> weaponLists;
        }
        
        [System.Serializable]
        public struct WeaponList
        {
            public string ID;
            public GameObject Prefab;
            public List<WeaponAttachmentList> weaponAttachmentList;
        }  

        [System.Serializable]
        public struct WeaponAttachmentList
        {
            public string ID;
            public GameObject Prefab;
        }  

        public WeaponDirectory WeaponLibrary;

        void Awake()
        {
            instance = this;
        }
    }
}