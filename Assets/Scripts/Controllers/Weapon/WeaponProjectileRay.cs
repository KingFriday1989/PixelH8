using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PixelH8.Controllers.Weapons
{
    public class WeaponProjectileRay : MonoBehaviour
    {
        [Serializable]
        public struct ImpactList
        {
            public string ID;
            public List<AudioClip> impactSounds;
        }
        [Serializable]
        public struct SmokePuffList
        {
            public string ID;
            public GameObject smokePuff;
        }

        [SerializeField] public List<SmokePuffList> smokePuffs;
        public List<ImpactList> impactList;
        public ParticleSystem specialParticleEffect;
        public GameObject bulletHole;

        public Vector3 startPos;
        [SerializeField] private Vector3 lastPos;
        [SerializeField] private Vector3 hitPos;
        [SerializeField] private Vector3 gravityEffect;

        public int gravityDelay = 20;
        public int maxDistance = 2000;

        public float distance;
        public float originalVelocity;
        public float bulletVelocity = 250;
        public int damageMin;
        public int damageMax;
        [SerializeField] private float bulletVelocityFrame;

        public LayerMask layerMask;

        public bool canBounce = true;
        [SerializeField] private bool bounced;
        [SerializeField] private bool gravity = false;

        //private bool inWater;

        private void Start()
        {
            startPos = transform.position;
            lastPos = startPos;
            originalVelocity = bulletVelocity;
        }

        void Update()
        {
            //Check if gravity needs to be set
            if (!gravity && Vector3.Distance(startPos, transform.position) > gravityDelay)
                gravity = true;

            //get curent velocity for this frame
            bulletVelocityFrame = bulletVelocity * Time.deltaTime;

            //start a raycast using the velocity for this frame as distance
            var ray = new Ray(lastPos, transform.forward);
            Physics.Raycast(ray, out RaycastHit hitInfo, bulletVelocityFrame, layerMask);

            var debugPos = transform.position;
            if (hitInfo.collider != null)
            {

                var newBulletHole = Instantiate(bulletHole);
                var decalProjector = newBulletHole.GetComponentInChildren<DecalProjector>();
                var bulletHoleManager = newBulletHole.GetComponent<BulletHole>();
                var ID = hitInfo.collider.sharedMaterial == null ? "Default" :
                    hitInfo.collider.sharedMaterial.name.ToLower() == "dirt" ? "Dirt" :
                    hitInfo.collider.sharedMaterial.name.ToLower() == "field" ? "Dirt" ://Update with more effects
                    hitInfo.collider.sharedMaterial.name.ToLower() == "grass" ? "Dirt" ://Update with more effects
                    hitInfo.collider.sharedMaterial.name.ToLower() == "mud" ? "Dirt" ://Update with more effects
                    hitInfo.collider.sharedMaterial.name.ToLower() == "wood planks" ? "Wood" :
                    hitInfo.collider.sharedMaterial.name.ToLower() == "metal grate" ? "Metal" :
                    "Default";
                var newSmoke = Instantiate(smokePuffs.Find(x => x.ID == ID).smokePuff);
                var newSpecialEffect = specialParticleEffect != null ? Instantiate(specialParticleEffect) : null;
                var rb = hitInfo.rigidbody;

                bulletHoleManager.ID = ID;
                newBulletHole.transform.up = hitInfo.normal;
                decalProjector.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, UnityEngine.Random.Range(-180, 180)));
                newBulletHole.transform.position = hitInfo.point;
                newBulletHole.transform.parent = hitInfo.collider.transform;

                newSmoke.transform.parent = null;
                newSmoke.transform.position = hitInfo.point;
                newSmoke.transform.up = hitInfo.normal;

                if(LayerMask.LayerToName(hitInfo.collider.gameObject.layer) == "Player" || LayerMask.LayerToName(hitInfo.collider.gameObject.layer) == "Enemy")
                {
                    var playerHealth = hitInfo.collider.gameObject.GetComponent<Health>();
                    playerHealth.Damage(UnityEngine.Random.Range(damageMin,damageMax +1));
                }

                if (newSpecialEffect != null)
                {
                    newSpecialEffect.transform.parent = null;
                    newSpecialEffect.transform.position = hitInfo.point;
                }

                var hitList = impactList.Find(x => x.ID == ID).impactSounds;
                var hitsound = hitList[UnityEngine.Random.Range(0,hitList.Count)];
                if (rb != null)
                {
                    rb.velocity += transform.forward + -hitInfo.normal * (bulletVelocity / 4 * Time.deltaTime);
                    //rb.AddRelativeForce(transform.forward + -hitInfo.normal * (bulletVelocity / 2 * Time.deltaTime), ForceMode.Impulse);
                }

                if (canBounce & !bounced)//expand to include physics material
                {
                    var hitInfoAngle = Vector3.Angle(transform.forward, hitInfo.normal);
                    bulletVelocity /= 2;
                    //transform.forward += hitInfo.normal * Vector3.Magnitude(transform.forward * (bulletVelocity * Time.deltaTime));
                    transform.forward = transform.forward + hitInfo.normal * 0.5f;

                    //Log(hitInfoAngle);
                    bool randChance = UnityEngine.Random.Range(0f, 1f) > 0.75f;
                    if (hitInfoAngle < 160 && randChance)
                    {
                        var ricList = impactList.Find(x => x.ID == "Ricochet").impactSounds;
                        var ricSnd = ricList[UnityEngine.Random.Range(0,ricList.Count)];
                        bounced = true;
                        AudioSource.PlayClipAtPoint(ricSnd, transform.position, 1); // Change these to a self destruct audio source that can adjust disance.
                    }
                    else
                    {
                        AudioSource.PlayClipAtPoint(hitsound, transform.position, 1);
                        ObjectPoolManager.ReturnObjectToPool(gameObject);
                        //Destroy(gameObject);
                    }
                }
                else
                {
                    AudioSource.PlayClipAtPoint(hitsound, transform.position, 1);
                    ObjectPoolManager.ReturnObjectToPool(gameObject);
                    //Destroy(gameObject);
                }
            }
            else
            {
                //move forward, add gravity or fly straight
                if (gravity)
                {
                    //lerp to desired speed then stop calculations
                    //used to simulate drag over time
                    if (bulletVelocity > originalVelocity * 0.5f)
                    {
                        bulletVelocity = Mathf.Lerp(bulletVelocity, originalVelocity * 0.5f, Time.deltaTime);
                        bulletVelocityFrame = bulletVelocity * Time.deltaTime;

                        if (bulletVelocity < originalVelocity * 0.5f + 0.01f)
                            bulletVelocity = originalVelocity * 0.5f;
                    }

                    //lerp to gravity then stop calculation
                    if (gravityEffect.y > Physics.gravity.y)
                    {
                        gravityEffect = Vector3.Lerp(gravityEffect, Physics.gravity, Time.deltaTime);
                        if (gravityEffect.y < Physics.gravity.y + 0.01f)
                            gravityEffect = Physics.gravity;
                    }

                    //add to position forward based on velocity plus gravity
                    transform.position = transform.position + ((transform.forward * bulletVelocityFrame) + gravityEffect * Time.deltaTime);
                }
                else
                    //add to position forward based on velocity
                    transform.position = transform.position + (transform.forward * bulletVelocityFrame);
            }

            
            lastPos = transform.position;
            distance = Vector3.Distance(startPos, transform.position);
            Debug.DrawLine(debugPos, lastPos,Color.green,5);

            if (distance > maxDistance)
                ObjectPoolManager.ReturnObjectToPool(gameObject);
                //Destroy(gameObject);
        }
    }
}