using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Helpers;

namespace PixelH8.Controllers.Weapons
{
    public class Weapon : MonoBehaviour
    {
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
        public bool SingleShot = true;
        public int MagAmmo = 5;
        public int MagMax = 5;
        public int ReserveAmmo = 30;
        public int ReserveMax = 30;
        public int DamageMin = 40;
        public int DamageMax = 60;
        [SerializeField]
        private GameObject AudioObject;
        [SerializeField]
        private AudioSource[] NonPrioritySources;
        public AudioClip Shoot;
        public AudioClip MagOut;
        public AudioClip MagIn;
        public AudioClip BoltBack;
        public AudioClip BoltForward;
        private int currentAudSrc;
        public float currentCylRot;
        public float desiredCylRot;
        private float reloadWait;
        public bool wait { get; private set; }
        public bool fireReset;

        void Awake()
        {
            rps = 60f / fireRPM;
            NonPrioritySources = new AudioSource[3];
            currentAudSrc = 2;
        }
   
        void LateUpdate()
        {
            if(HasCylender)
            {
                UpdateCylenderRotation();
            }
        }

        public void Fire()
        {
            SpawProjectile();

            animator.SetTrigger("Fire");
            lastShot = Time.time;
            PlayFire();
            var newMuzzleFlash = ObjectPoolManager.SpawnObject(MuzzleFlash,MuzzlePoint.position,Quaternion.Euler(Vector3.zero));
            //newMuzzleFlash.transform.parent = null;
            newMuzzleFlash.transform.forward = GetFireDirection();
        }

        public void Reload()
        {
            animator.SetTrigger("Reload");
            wait = true;
        }

        public void ResetWait()
        {
            wait = false;
        }

        public void UpdateCylenderRotation()
        {
            var rotAngle = 360/MagMax;
            desiredCylRot = rotAngle * MagAmmo;

            if(currentCylRot != desiredCylRot)
            {
                currentCylRot = Mathf.Lerp(currentCylRot,desiredCylRot,0.1f);
                Cylender.localRotation = Quaternion.Euler(0,0,currentCylRot);
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

        private void InstantiateShell(GameObject shell, bool Cluster = false)
        {
            var ejectShell = ObjectPoolManager.SpawnObject(shell, EjectPoint.position, Quaternion.identity);
            ejectShell.transform.parent = null;
            ejectShell.transform.rotation = Quaternion.Euler(ejectShell.transform.rotation.eulerAngles + new Vector3(0, 0, Random.Range(-30f, 30f)));

            if (Cluster)
                ejectShell.transform.localPosition += new Vector3(UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f));
        }

        public void PlayFire()
        {
            SpawnAudio(Shoot);
        }
        public void PlayMagOut()
        {
            SpawnAudio(MagOut, 20);
        }
        public void PlayMagIn()
        {
            SpawnAudio(MagIn, 20);
        }
        public void PlayBoltBack()
        {
            SpawnAudio(BoltBack, 20);
        }
        public void PlayBoltForward()
        {
            SpawnAudio(BoltForward, 20);
        }

        private void SpawProjectile()
        {
            //var newProjectile = Instantiate(Projectile, MuzzlePoint);
            var newProjectile = ObjectPoolManager.SpawnObject(Projectile, MuzzlePoint.position, Quaternion.identity);
            newProjectile.GetComponent<WeaponProjectileRay>().damageMin = DamageMin;
            newProjectile.GetComponent<WeaponProjectileRay>().damageMax = DamageMax;
            //newProjectile.transform.parent = null;
            newProjectile.transform.forward = GetFireDirection();
        }
        private void SpawnAudio(AudioClip clip, int maxDistance = 100/*,bool nonPriority = false*/)
        {
            var audPoint = ObjectPoolManager.SpawnObject(AudioObject, transform.position, Quaternion.Euler(Vector3.zero));
            var audSrc = audPoint.GetComponent<AudioSource>();
            //var autoReturn = audPoint.GetComponent<AutoReturn>();
            audPoint.GetComponent<AutoReturn>().returnDelay = clip.length;

            audSrc.clip = clip;
            audSrc.volume = 1;//change to volume setting
            audSrc.rolloffMode = AudioRolloffMode.Logarithmic;
            audSrc.minDistance = 5;
            audSrc.maxDistance = maxDistance;
            audSrc.pitch = UnityEngine.Random.Range(0.8f,1.2f);
            //if (nonPriority)
            //{
            //    if (currentAudSrc >= 2)
            //        currentAudSrc = 0;
            //    else
            //        currentAudSrc++;
            //
            //    if (NonPrioritySources[currentAudSrc] != null)
            //    {
            //        Destroy(NonPrioritySources[currentAudSrc].gameObject);
            //        NonPrioritySources[currentAudSrc] = audSrc;
            //    }
            //    else
            //        NonPrioritySources[currentAudSrc] = audSrc;
            //}
            audSrc.enabled = false;
            audSrc.enabled = true;
            audSrc.Play();
            //autoReturn.returnDelay = clip.length;
            //StartCoroutine(AudioReturnDelay(autoReturn,audSrc));
        }

        //IEnumerator AudioReturnDelay(AutoReturn autoReturn, AudioSource audioSource)
        //{
        //    yield return new WaitForSeconds(0.01f);
        //    autoReturn.returnDelay = audioSource.clip.length;
        //}
        Vector3 GetFireDirection()
        {
            return firePoint - MuzzlePoint.position;
        }
    }
}