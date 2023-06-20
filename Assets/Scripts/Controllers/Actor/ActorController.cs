using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Data.Actors;
using PixelH8.Controllers.Weapons;
using PixelH8.Helpers;
using PixelH8.Data;

namespace PixelH8.Controllers.Actors
{


    public class ActorController : MonoBehaviour
    {
        public Actor actor;
        private ActorData data;
        private float lastStep;
        private float offsetX;
        private float offsetY;
        private float offsetZ;

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
            SetCameraAnimator();
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

                data.moving = data.horizontalFloat > 0.125f || data.horizontalFloat < -0.125f || data.verticalFloat > 0.125f || data.verticalFloat < -0.125f;
                data.move = transform.right * data.horizontalFloat + transform.forward * data.verticalFloat;
                data.move = Vector3.ClampMagnitude(data.move, 1);

                if (data.isJumping && data.jumpTimer < Time.time)
                    data.isJumping = false;
            }
            else
            {
                data.onGround = false;
                data.moving = false;

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
                    var currentItem = actor.actorInventory.items[currentSlot];
                    var MX = UnityEngine.Input.GetAxis("Mouse X") * Time.deltaTime;
                    var MY = UnityEngine.Input.GetAxis("Mouse Y") * Time.deltaTime;

                    offsetX += -MX;
                    offsetX = Mathf.Clamp(offsetX, -0.1f, 0.1f);
                    offsetY += -MY;
                    offsetY = Mathf.Clamp(offsetY, -0.1f, 0.1f);

                    var offsetVector = new Vector3
                        (
                            offsetX + data.xRot * 0.0005f,
                            offsetY + data.xRot * 0.0005f,
                            -data.xRot * 0.0005f
                        );

                    offsetX = Mathf.Lerp(offsetX, 0, Time.deltaTime * 16);
                    offsetY = Mathf.Lerp(offsetY, 0, Time.deltaTime * 16);

                    if (Vector3.Distance(data.offsetTransform.localPosition, currentItem.positionOffset + offsetVector) > 0.001f)
                    {
                        data.offsetTransform.localPosition = Vector3.Slerp(data.offsetTransform.localPosition, currentItem.positionOffset + offsetVector, Time.deltaTime * 8);
                    }
                    else
                        data.offsetTransform.localPosition = currentItem.positionOffset + offsetVector;

                    var hitInfo = actor.GetWeaponFirePoint(actor.actorData.cameraTransform.position, actor.actorData.cameraTransform.forward);
                    Vector3 AimPoint = Vector3.zero;
                    if (hitInfo.collider != false)
                        AimPoint = hitInfo.point;
                    else
                        AimPoint = actor.actorData.cameraTransform.position + actor.actorData.cameraTransform.forward * 2000;

                    Vector3 lerpRot = Vector3.Slerp(data.offsetTransform.forward, AimPoint - data.offsetTransform.position, Time.deltaTime * 8);
                    data.offsetTransform.forward = lerpRot;
                    data.offsetTransform.localEulerAngles = new Vector3(data.offsetTransform.localEulerAngles.x, data.offsetTransform.localEulerAngles.y, Mathf.Clamp(data.offsetTransform.localEulerAngles.z, -10, 10));
                    //Vector3 clampedRot = new Vector3(data.offsetTransform.localRotation.x, data.offsetTransform.localRotation.y, data.offsetTransform.localRotation.z);
                    //
                    //data.offsetTransform.localEulerAngles = clampedRot;

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
                        data.offsetTransform.localRotation = Quaternion.Euler
                        (new Vector3
                            (
                                Mathf.LerpAngle(data.offsetTransform.localRotation.eulerAngles.x, 0, Time.deltaTime * 8),
                                Mathf.LerpAngle(data.offsetTransform.localRotation.eulerAngles.y, 0, Time.deltaTime * 8),
                                Mathf.LerpAngle(data.offsetTransform.localRotation.eulerAngles.z, 0, Time.deltaTime * 8)
                            )
                        );
                    }
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
        void ClampFirstPersonWeapon()
        {

        }
        void SetCameraAnimator()
        {
            actor.animator.SetFloat("Vert", data.Aim ? 0 : data.verticalFloat);
            actor.animator.SetFloat("Hor", data.Aim ? 0 : data.horizontalFloat);
            actor.animator.SetFloat("Speed", data.Aim ? 0 : data.speedModifier);
        }

        void FireWeapon()
        {
            var currentSlot = actor.actorInventory.currentSlot;
            var weapon = actor.actorInventory.items[currentSlot];
            weapon.Fire();
        }

        public void PlayStep()
        {
            if (lastStep < Time.time)
            {
                lastStep = Time.time + 0.1f;
                var audioClips = ObjectsAndData.Instance.AudioContainer.AudioLibrary.audioGroups.Find(x => x.ID == "Footsteps").audioLists.Find(x => x.ID == "Default").audioClips;
                var clip = audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
                var audPoint = AudioTools.SpawnAudio(ObjectsAndData.Instance.AudioContainer.AudioObject, clip, transform.position, 0.25f * data.speedModifier, 20, 0.9f, 1.1f);
            }
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
