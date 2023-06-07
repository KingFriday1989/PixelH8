using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Data.Actors;
using PixelH8.Controllers.Weapons;

namespace PixelH8.Controllers.Actors
{
    public class Actor : MonoBehaviour
    {
        public Animator animator;
        public ActorData actorData;
        public ActorInventory actorInventory;
        public CharacterController characterController;
        public LayerMask walkableMasks;

        protected virtual void Start()
        {
            actorData = GetComponent<ActorData>();
            characterController = GetComponent<CharacterController>();
        }

        public RaycastHit GetWeaponFirePoint(Vector3 StartPos,Vector3 StartForward)
        {
            Physics.Raycast(actorData.cameraTransform.position,
                            actorData.cameraTransform.forward,
                            out RaycastHit hitInfo,
                            2000f,
                            actorInventory.items[actorInventory.currentSlot].Projectile.GetComponent<WeaponProjectileRay>().layerMask);

            return hitInfo;
        }
    }
}

