using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Data.Actors;

namespace PixelH8.Controllers.Actors
{
    public class Actor : MonoBehaviour
    {
        public ActorData actorData;
        public ActorInventory actorInventory;
        public CharacterController characterController;
        public LayerMask walkableMasks;

        protected virtual void Start()
        {
            actorData = GetComponent<ActorData>();
            characterController = GetComponent<CharacterController>();
        }
    }
}

