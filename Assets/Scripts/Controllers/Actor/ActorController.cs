using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Data.Actors;
using PixelH8.Controllers.Weapons;
using PixelH8.Helpers;

namespace PixelH8.Controllers.Actors
{


    public class ActorController : MonoBehaviour
    {
        public Actor actor;
        private ActorData data;

        private void Start()
        {
            actor = GetComponent<Actor>();
            data = actor.actorData;
        }

        private void Update()
        {
            CheckMouse();
            MoveCamera();
            SetFirstPersonOffset();
        }
        private void LateUpdate()
        {

        }
        private void FixedUpdate()
        {
            MovementFloats();
            DoMove();
        }
        void CheckMouse()
        {
            {
                float mouseX = UnityEngine.Input.GetAxis("Mouse X") * Time.deltaTime;
                float mouseY = UnityEngine.Input.GetAxis("Mouse Y") * Time.deltaTime;

                mouseX = mouseX * 200;//sensitivity
                mouseY = mouseY * 200;

                data.xRot -= mouseY;
                data.xRot = Mathf.Clamp(data.xRot, -85, 85);
                data.xRotationTransform.localRotation = Quaternion.Euler(data.xRot, 0f, 0f);
                transform.Rotate(Vector3.up * mouseX);
            }
        }
        void DoMove()
        {
            if (Grounded() && !data.isJumping)
            {
                if (data.fall)
                {
                    data.landDelay = false;
                    data.fall = false;
                    data.onGround = true;
                }

                if (data.jump && !data.isJumping && data.jumpTimer < Time.time)
                {
                    data.jumpTimer = Time.time + 0.25f;
                    data.isJumping = true;
                    data.velocity = new Vector3(
                        data.velocity.x,
                        data.jumpHeight,
                        data.velocity.z
                    );
                }

                if (!data.isJumping)
                {
                    data.velocity = new Vector3(
                        data.velocity.x,
                        Mathf.Lerp(data.velocity.y, -6, Time.deltaTime * 2),
                        data.velocity.z
                    );
                }

                data.move = transform.right * data.horizontalFloat + transform.forward * data.verticalFloat;
                data.move = Vector3.ClampMagnitude(data.move, 1);

                if (data.isJumping && data.jumpTimer < Time.time)
                    data.isJumping = false;
            }
            else
            {
                data.onGround = false;

                if (!data.isJumping && Grounded(Mathf.Abs(data.velocity.y) * 0.125f) && !data.landDelay)
                    data.landDelay = true;

                if (data.isJumping && data.velocity.y < 0)
                {
                    data.fall = true;
                    data.isJumping = false;
                }
                else if (!data.isJumping && data.onGround)
                {
                    data.onGround = false;
                    data.fall = true;
                    data.velocity = new Vector3(
                        data.velocity.x,
                        0,
                        data.velocity.z
                    );
                }
                else
                    data.onGround = false;

                if (data.fall)
                    data.velocity = new Vector3(
                        data.velocity.x,
                        data.velocity.y + Physics.gravity.y * Time.deltaTime,
                        data.velocity.z
                    );
                data.fall = true;
            }

            actor.characterController.Move((data.move * ((data.speed * data.speedModifier) * Time.fixedDeltaTime)) + (data.velocity * Time.fixedDeltaTime));
        }
        void MoveCamera()
        {
            if (data.xRot > 50)
            {
                var max = 85 - 50;
                var value = data.xRot - 50;
                var percent = value / max;
                data.cameraTransform.localPosition = new Vector3(0, percent, percent * 0.125f);
            }
            else
                data.cameraTransform.localPosition = Vector3.zero;
        }
        void MovementFloats()
        {
            if (data.onGround)
            {
                data.verticalFloat = Mathf.Lerp(data.verticalFloat, (data.forward && !data.backward) ? 1f :
                    (data.backward && !data.forward) ? -1f :
                    0f, 8f * Time.fixedDeltaTime);
                data.horizontalFloat = Mathf.Lerp(data.horizontalFloat, (data.right && !data.left) ? 1f :
                    (data.left && !data.right) ? -1f :
                    0f, 8f * Time.fixedDeltaTime);
                data.speedModifier = Mathf.Lerp(data.speedModifier,
                    (data.sprint &&
                    data.forward &&
                    !data.backward) ? 1.75f :
                    1f, 8f * Time.fixedDeltaTime);


                data.horizontalFloat = data.horizontalFloat > 0.99f ? 1f :
                data.horizontalFloat < -0.99f ? -1 :
                data.horizontalFloat < 0.01f && data.horizontalFloat > -0.01f ? 0f :
                data.horizontalFloat;

                data.verticalFloat = data.verticalFloat > 0.99f ? 1f :
                data.verticalFloat < -0.99f ? -1 :
                data.verticalFloat < 0.01f && data.verticalFloat > -0.01f ? 0f :
                data.verticalFloat;

                data.speedModifier = data.speedModifier > 1.74f ? 1.75f :
                data.speedModifier < 1.01f && data.speedModifier > 1 ? 1f :
                data.speedModifier < 0.51f ? 0.5f :
                data.speedModifier > 0.99f && data.speedModifier < 1 ? 1 :
                data.speedModifier;
            }
        }
        void SetFirstPersonOffset()
        {
            int currentSlot = actor.actorInventory.currentSlot;
            if (actor.actorInventory.items[currentSlot] != null)
            {
                if (!actor.actorData.Aim)
                {
                    //data.offsetTransform.localPosition = actor.actorInventory.items[currentSlot].positionOffset;
                    if (Vector3.Distance(data.offsetTransform.localPosition, actor.actorInventory.items[currentSlot].positionOffset) > 0.001f)
                    {
                        data.offsetTransform.localPosition = Vector3.Slerp(data.offsetTransform.localPosition, actor.actorInventory.items[currentSlot].positionOffset, Time.deltaTime * 8);
                    }
                    else
                        data.offsetTransform.localPosition = actor.actorInventory.items[currentSlot].positionOffset;

                    Physics.Raycast(actor.actorData.cameraTransform.position,
                            actor.actorData.cameraTransform.forward,
                            out RaycastHit hitInfo,
                            2000f,
                            actor.actorInventory.items[currentSlot].Projectile.GetComponent<WeaponProjectileRay>().layerMask);

                    Vector3 AimPoint = Vector3.zero;
                    if (hitInfo.collider != false)
                        AimPoint = hitInfo.point;
                    else
                        AimPoint = actor.actorData.cameraTransform.position + actor.actorData.cameraTransform.forward * 2000;

                    Vector3 lerpRot = Vector3.Slerp(data.offsetTransform.forward, AimPoint - data.offsetTransform.position, Time.deltaTime * 4);

                    data.offsetTransform.forward = lerpRot;
                }
                else
                {
                    if (Vector3.Distance(data.offsetTransform.localPosition, new Vector3(0, 0, actor.actorInventory.items[currentSlot].positionOffset.z)) > 0.001f)
                    {
                        data.offsetTransform.localPosition = Vector3.Slerp(data.offsetTransform.localPosition, new Vector3(0, 0, actor.actorInventory.items[currentSlot].positionOffset.z), Time.deltaTime * 16);
                    }
                    else
                        data.offsetTransform.localPosition = new Vector3(0, 0, actor.actorInventory.items[currentSlot].positionOffset.z);

                    if (Vector3.Distance(data.offsetTransform.localRotation.eulerAngles, Vector3.zero) > 0.01f)
                    {
                        data.offsetTransform.localRotation = Quaternion.Euler(new Vector3(
                            Mathf.LerpAngle(data.offsetTransform.localRotation.eulerAngles.x,0,Time.deltaTime * 8),
                            Mathf.LerpAngle(data.offsetTransform.localRotation.eulerAngles.y,0,Time.deltaTime * 8),
                            Mathf.LerpAngle(data.offsetTransform.localRotation.eulerAngles.z,0,Time.deltaTime * 8)
                        ));
                    }
                        //data.offsetTransform.localRotation = Quaternion.Euler(Vector3.Lerp(data.offsetTransform.localRotation.eulerAngles, Vector3.zero, Time.deltaTime * 4));
                    else
                        data.offsetTransform.localRotation = Quaternion.Euler(Vector3.zero);

                }
            }
            else
            {
                data.offsetTransform.localPosition = Vector3.zero;
                data.offsetTransform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }
        void SetFirstPersonAnimator()
        {
            
        }

        void FireWeapon()
        {
            var currentSlot = actor.actorInventory.currentSlot;
            var weapon = actor.actorInventory.items[currentSlot];
            weapon.Fire();
        }
        
        public bool Grounded(float distanceAdditive = 0)
        {
            var Pos1 = transform.position + new Vector3(0, actor.characterController.height / 2f, 0) + actor.characterController.center + Vector3.up * -actor.characterController.height * 0.5f;
            var Pos2 = Pos1 + Vector3.up * actor.characterController.height;
            if (Physics.CapsuleCast(Pos1, Pos2, actor.characterController.radius, Vector3.down, actor.characterController.height / 2f + distanceAdditive, actor.walkableMasks))
                return true;
            else
                return false;
        }
    }
}
