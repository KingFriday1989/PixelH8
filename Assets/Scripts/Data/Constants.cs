using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelH8.Data
{
    [CreateAssetMenu(fileName = "PixelH8/Data", menuName = "Data Sets/Constants")]
    public class Constants : ScriptableObject
    {
        [SerializeField] public LayerMask SolidObjects;
        [SerializeField] public GameObject waypoint;
    }
}

