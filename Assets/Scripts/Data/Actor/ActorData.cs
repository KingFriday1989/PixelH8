using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelH8.Data.Actors
{
    public class ActorData : MonoBehaviour
    {
        public string ActorName {get;private set;}

        public float xRot;
        public float verticalFloat;
        public float horizontalFloat;
        public float speedModifier;
        public float speed;
        public float jumpTimer;
        public float jumpHeight = 3;

        public Transform xRotationTransform;
        public Transform cameraTransform;
        public Transform leanTransform;
        public Transform offsetTransform;

        public Renderer[] bodyMeshes;

        public Vector3 velocity;
        public Vector3 move;

        public bool onGround;
        public bool moving;
        public bool rotating;
        public bool forward;
        public bool backward;
        public bool left;
        public bool right;
        public bool sprint;
        public bool jump;
        public bool isJumping;
        public bool landDelay;
        public bool fall;
        public bool zoomIn;
        public bool zoomOut;

        public bool itemEquipt;
        public bool itemHolstered;
        public bool showItem;
        public bool hideItem;
        public bool Aim;
        public bool fire;
        public bool fireHeld;
        public bool draw;
        public bool holster;
        public bool reload;
    }
}