using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using PixelH8.Helpers;
using PixelH8.Data;

namespace PixelH8.Controllers.Weapons
{
    public class Weapon : MonoBehaviour
    {
        #region Enum
        public enum WeaponType
        {
            Revolver,
            SMG,
            Rifle,
            Shotgun,
            Grenade,
            Knife,
        }
        [Flags]
        public enum WeaponAttachments
        {
            None = 0,
            Silencer = 1,
            Sight = 2,
            Ammo = 4,
        }

        [Flags]
        public enum WeaponMode
        {
            //None = 0,
            Semi = 0 | 1,
            Burst = 2,
            Auto = 4,
            Special = 8,
        }

        public WeaponType weaponType;
        [EnumFlags]
        public WeaponAttachments weaponAttachments;
        [EnumFlags]
        public WeaponMode weaponModes;

        private WeaponMode weaponMode;


        #endregion
        #region Variables
        public float fireRPM;
        [HideInInspector] public float rps;
        public float lastShot;
        public Animator animator;
        public Vector3 rotationOffset;
        public Vector3 positionOffset;
        public Vector3 firePoint;
        public Transform MuzzlePoint;
        public Transform EjectPoint;
        public Transform Cylender;
        public Transform AimPoint;
        public GameObject Projectile;
        public GameObject Shell;
        public GameObject MuzzleFlash;
        public int ShellEjectForce;
        public bool EjectOnReload;
        public bool HasCylender;
        public bool wait { get; private set; }
        public bool fireReset;
        public bool burst;
        public int BurstCount = 3;
        public int BurstCurrent;
        public int MagAmmo = 5;
        public int MagMax = 5;
        public int ReserveAmmo = 30;
        public int ReserveMax = 30;
        public int DamageMin = 40;
        public int DamageMax = 60;

        [SerializeField]
        private AudioSource[] NonPrioritySources;
        private List<AudioClip> Shoot;
        private List<AudioClip> MagOut;
        private List<AudioClip> MagIn;
        private List<AudioClip> BoltBack;
        private List<AudioClip> BoltForward;
        private int currentAudSrc;
        private float currentCylRot;
        private float desiredCylRot;
        private float reloadWait;
        #endregion 
        #region Start
        void Awake()
        {
            weaponMode = weaponModes.HasFlag(WeaponMode.Auto) ? WeaponMode.Auto :
            weaponModes.HasFlag(WeaponMode.Semi) ? WeaponMode.Semi :
            WeaponMode.Burst;

            rps = 60f / fireRPM;
            NonPrioritySources = new AudioSource[3];
            currentAudSrc = 2;
        }
        void Start()
        {
            var audGrp = ObjectsAndData.Instance.AudioContainer.AudioLibrary.audioGroups;
            Shoot = audGrp.Find(x => x.ID == "Revolver").audioLists.Find(x => x.ID == "Fire").audioClips;
            MagOut = audGrp.Find(x => x.ID == "Revolver").audioLists.Find(x => x.ID == "MagOut").audioClips;
            MagIn = audGrp.Find(x => x.ID == "Revolver").audioLists.Find(x => x.ID == "MagIn").audioClips;
            BoltBack = audGrp.Find(x => x.ID == "Revolver").audioLists.Find(x => x.ID == "BoltBack").audioClips;
            BoltForward = audGrp.Find(x => x.ID == "Revolver").audioLists.Find(x => x.ID == "BoltForward").audioClips;
        }
        void LateUpdate()
        {
            if (HasCylender)
            {
                UpdateCylenderRotation();
            }
        }
        #endregion
        #region Calls
        public WeaponMode GetWeaponMode()
        {
            return weaponMode;
        }
        public void Fire()
        {
            SpawProjectile();
            MagAmmo--;
            animator.SetTrigger("Fire");
            lastShot = Time.time;
            PlayFire();
            var newMuzzleFlash = ObjectPoolManager.SpawnObject(MuzzleFlash, MuzzlePoint.position, Quaternion.Euler(Vector3.zero));
            newMuzzleFlash.transform.forward = GetFireDirection();
        }
        public void Reload()
        {
            animator.SetTrigger("Reload");
            wait = true;
        }
        #endregion
        #region Animation Events
        public void ResetWait()
        {
            wait = false;
        }
        public void UpdateCylenderRotation()
        {
            var rotAngle = 360 / MagMax;
            desiredCylRot = rotAngle * MagAmmo;

            if (currentCylRot != desiredCylRot)
            {
                if (currentCylRot <= desiredCylRot - 0.05f)
                {
                    currentCylRot = desiredCylRot;
                    Cylender.localRotation = Quaternion.Euler(0, 0, currentCylRot);
                }
                else
                {
                    currentCylRot = Mathf.Lerp(currentCylRot, desiredCylRot, 0.05f);
                    Cylender.localRotation = Quaternion.Euler(0, 0, currentCylRot);
                }

            }
        }
        public void AddAmmo()
        {
            if (MagAmmo == 0)
            {
                if (ReserveAmmo < MagMax)
                {
                    MagAmmo = ReserveAmmo;
                    ReserveAmmo = 0;
                }
                else
                {
                    ReserveAmmo -= MagMax;
                    MagAmmo = MagMax;
                }
            }
            else if (MagAmmo > 0)
            {
                if (ReserveAmmo < MagMax)
                {
                    MagAmmo += ReserveAmmo;
                    ReserveAmmo = 0;
                }
                else
                {
                    var needed = MagMax - MagAmmo;
                    ReserveAmmo -= needed;
                    MagAmmo = MagMax;
                }
            }
        }
        public async void SpawnShell()
        {
            if (Shell != null)
            {
                if (EjectOnReload)
                {
                    var cycles = MagMax - MagAmmo;
                    for (int i = 0; i < cycles; i++)
                    {
                        InstantiateShell(Shell, true);
                        await System.Threading.Tasks.Task.Delay(100);
                    }
                }
                else
                {
                    InstantiateShell(Shell);
                }
            }
        }
        #endregion
        #region Effects
        private void InstantiateShell(GameObject shell, bool Cluster = false)
        {
            var ejectShell = ObjectPoolManager.SpawnObject(shell, EjectPoint.position, Quaternion.Euler(Vector3.zero));
            ejectShell.transform.forward = EjectPoint.forward;
            ejectShell.transform.rotation = Quaternion.Euler(ejectShell.transform.rotation.eulerAngles + new Vector3(0, 0, UnityEngine.Random.Range(-30f, 30f)));

            if (Cluster)
                ejectShell.transform.localPosition += new Vector3(UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f));
        }

        private void SpawProjectile()
        {
            var newProjectile = ObjectPoolManager.SpawnObject(Projectile, MuzzlePoint.position, Quaternion.Euler(Vector3.zero));
            newProjectile.GetComponent<WeaponProjectileRay>().damageMin = DamageMin;
            newProjectile.GetComponent<WeaponProjectileRay>().damageMax = DamageMax;
            newProjectile.transform.forward = GetFireDirection();
        }

        Vector3 GetFireDirection()
        {
            return firePoint - MuzzlePoint.position;
        }
        #endregion
        #region Sound
        public void PlayFire()
        {
            var clip = AudioTools.ReturnRandomClip(Shoot);
            if (clip != null)
                AudioTools.SpawnAudio(ObjectsAndData.Instance.AudioContainer.AudioObject, clip, transform.position, 0.6f, 500, 0.8f, 1.2f);
        }
        public void PlayMagOut()
        {
            var clip = AudioTools.ReturnRandomClip(MagOut);
            if (clip != null)
                AudioTools.SpawnAudio(ObjectsAndData.Instance.AudioContainer.AudioObject, clip, transform.position, 0.5f, 20, 0.9f, 1.1f);
        }
        public void PlayMagIn()
        {
            var clip = AudioTools.ReturnRandomClip(MagIn);
            if (clip != null)
                AudioTools.SpawnAudio(ObjectsAndData.Instance.AudioContainer.AudioObject, clip, transform.position, 0.5f, 20, 0.9f, 1.1f);
        }
        public void PlayBoltBack()
        {
            var clip = AudioTools.ReturnRandomClip(BoltBack);
            if (clip != null)
                AudioTools.SpawnAudio(ObjectsAndData.Instance.AudioContainer.AudioObject, clip, transform.position, 0.5f, 20, 0.9f, 1.1f);
        }
        public void PlayBoltForward()
        {
            var clip = AudioTools.ReturnRandomClip(BoltForward);
            if (clip != null)
                AudioTools.SpawnAudio(ObjectsAndData.Instance.AudioContainer.AudioObject, clip, transform.position, 0.5f, 20, 0.9f, 1.1f);
        }
        #endregion
    }
}