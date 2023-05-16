using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Controllers.Weapons;

public class ActorInventory : MonoBehaviour
{
    public Weapon[] items = new Weapon[5];
        public int currentSlot;
        public int lastSlot;
}
