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
                    if ((!currentItem.fireReset || currentItem.burst && currentItem.BurstCurrent < currentItem.BurstCount) && !currentItem.wait && currentItem.lastShot < Time.time - currentItem.rps)
                    {
                        if(currentItem.GetWeaponMode() == Weapon.WeaponMode.Semi && !currentItem.fireReset)
                            currentItem.fireReset = true;
                        else if(currentItem.GetWeaponMode() == Weapon.WeaponMode.Burst)
                        {
                            currentItem.burst = true;
                            currentItem.BurstCurrent = 0;
                        }

                        if (currentItem.MagAmmo > 0)
                        {
                            var hitInfo = actor.GetWeaponFirePoint(actor.actorData.cameraTransform.position,actor.actorData.cameraTransform.forward);
                            Vector3 firePointNew = Vector3.zero;
                            if (hitInfo.collider != false)
                                firePointNew = hitInfo.point;
                            else
                                firePointNew = actor.actorData.cameraTransform.position + actor.actorData.cameraTransform.forward * 1000;
                                
                            currentItem.firePoint = firePointNew;
                            currentItem.Fire();

                            if(currentItem.burst)
                                if(currentItem.BurstCurrent < currentItem.BurstCount)
                                    currentItem.BurstCurrent++;
                                else
                                {
                                    currentItem.burst = false;
                                    currentItem.fireReset = false;
                                }
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
                        currentItem.burst = false;
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