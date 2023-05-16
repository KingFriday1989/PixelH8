using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Controllers.Weapons;


namespace PixelH8.Controllers.Actors
{
    public class ActorInput : MonoBehaviour
    {
        private Actor actor;

        private void Start()
        {
            actor = GetComponent<Actor>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            CheckInputKeys();
        }


        void CheckInputKeys()
        {
            //Forward
            if (UnityEngine.Input.GetKey(KeyCode.W) && !UnityEngine.Input.GetKey(KeyCode.S))
                actor.actorData.forward = true;
            else
                actor.actorData.forward = false;
            //Backward
            if (UnityEngine.Input.GetKey(KeyCode.S) && !UnityEngine.Input.GetKey(KeyCode.W))
                actor.actorData.backward = true;
            else
                actor.actorData.backward = false;
            //Left
            if (UnityEngine.Input.GetKey(KeyCode.A) && !UnityEngine.Input.GetKey(KeyCode.D))
                actor.actorData.left = true;
            else
                actor.actorData.left = false;
            //Right
            if (UnityEngine.Input.GetKey(KeyCode.D) && !UnityEngine.Input.GetKey(KeyCode.A))
                actor.actorData.right = true;
            else
                actor.actorData.right = false;

            // Jumping
            if (UnityEngine.Input.GetKey(KeyCode.Space))
                actor.actorData.jump = true;
            else
                actor.actorData.jump = false;

            // Sprint
            if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
            {
                actor.actorData.sprint = true;
            }
            else
                actor.actorData.sprint = false;


            if (Input.GetKey(KeyCode.Mouse0))
            {
                var currentSlot = actor.actorInventory.currentSlot;
                if (actor.actorInventory.items[currentSlot])
                {
                    var currentItem = actor.actorInventory.items[currentSlot];
                    if (!currentItem.fireReset && !currentItem.wait && currentItem.lastShot < Time.time - currentItem.rps)
                    {
                        if(currentItem.SingleShot && !currentItem.fireReset)
                            currentItem.fireReset = true;
                        if (currentItem.MagAmmo > 0)
                        {
                            currentItem.MagAmmo--;
                            Physics.Raycast(actor.actorData.cameraTransform.position,
                            actor.actorData.cameraTransform.forward,
                            out RaycastHit hitInfo,
                            2000f,
                            currentItem.Projectile.GetComponent<WeaponProjectileRay>().layerMask);

                            Vector3 AimPoint = Vector3.zero;
                            if (hitInfo.collider != false)
                                AimPoint = hitInfo.point;
                            else
                                AimPoint = actor.actorData.cameraTransform.position + actor.actorData.cameraTransform.forward * 2000;

                            //Debug.DrawLine(actor.actorData.cameraTransform.position,AimPoint,Color.red,10);
                            currentItem.firePoint = AimPoint;
                            currentItem.Fire();
                        }

                    }
                }
            }
            else
            {
                var currentSlot = actor.actorInventory.currentSlot;
                var currentItem = actor.actorInventory.items[currentSlot];
                if(currentItem != null)
                {
                    if(currentItem.lastShot < Time.time - currentItem.rps)
                        currentItem.fireReset = false;
                }
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                var currentSlot = actor.actorInventory.currentSlot;
                if (actor.actorInventory.items[currentSlot])
                {
                    var currentItem = actor.actorInventory.items[currentSlot];
                    if (!currentItem.wait)
                    {
                        actor.actorData.Aim = true;
                    }
                }
            }
            else
            {
                actor.actorData.Aim = false;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                var currentSlot = actor.actorInventory.currentSlot;
                if (actor.actorInventory.items[currentSlot])
                {
                    var currentItem = actor.actorInventory.items[currentSlot];
                    if (!currentItem.wait && currentItem.MagAmmo < currentItem.MagMax && currentItem.ReserveAmmo > 0)
                    {
                        currentItem.Reload();
                    }
                }
            }
        }
    }
}