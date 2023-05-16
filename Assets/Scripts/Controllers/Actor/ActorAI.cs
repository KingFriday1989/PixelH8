using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PixelH8.Controllers.Weapons;

namespace PixelH8.Controllers.Actors
{
    public class ActorAI : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;
        public Transform Target;
        public Weapon weapon;

        private float FireDelay;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var dis = Vector3.Distance(Target.position, transform.position);
            if (dis < 30)
            {
                navMeshAgent.angularSpeed = 360;
                if (dis < 30 && 
                weapon.lastShot < Time.time - weapon.rps && 
                weapon.lastShot < Time.time - FireDelay && 
                weapon.MagAmmo > 0 && 
                !weapon.wait)
                {
                    if (weapon.MagAmmo > 0)
                    {
                        weapon.MagAmmo--;
                        Physics.Raycast(transform.position + new Vector3(0, 1, 0),
                            transform.forward,
                            out RaycastHit hitInfo,
                            2000,
                            weapon.Projectile.GetComponent<WeaponProjectileRay>().layerMask);

                        Vector3 AimPoint = Vector3.zero;
                        if (hitInfo.collider != false)
                            AimPoint = hitInfo.point;
                        else
                            AimPoint = transform.position + new Vector3(0, 1, 0) + transform.forward * 2000;

                        weapon.firePoint = AimPoint;
                        weapon.Fire();
                        FireDelay = UnityEngine.Random.Range(0.5f,2f);
                    }
                }

                if (dis < 10)
                {
                    var rot = Target.position - transform.position;
                    transform.forward = new Vector3(rot.x,0,rot.z);
                    navMeshAgent.SetDestination(transform.position);
                }
                else
                {
                    
                    navMeshAgent.SetDestination(Target.position);
                }
            }
            else
            {
                navMeshAgent.angularSpeed = 120;
            }
            
            if (weapon.MagAmmo == 0 && weapon.ReserveAmmo > 0 && !weapon.wait)
            {
                weapon.ReserveAmmo += weapon.MagMax;
                weapon.Reload();
            }
        }
    }
}