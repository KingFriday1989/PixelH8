using UnityEngine;

namespace PixelH8.Data
{
    public class ObjectsAndData : Singleton<ObjectsAndData>
    {
        [SerializeField]
        public AudioContainer AudioContainer = AudioContainer.instance;
        [SerializeField]
        public WeaponContainer weaponContainer = WeaponContainer.instance;

        public Constants constants;

        public GameObject Waypoint;
    }
}