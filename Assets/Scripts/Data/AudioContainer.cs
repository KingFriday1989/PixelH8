using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelH8.Data
{
    [CreateAssetMenu(fileName = "AudioContainer", menuName = "PixelH8/AudioContainer")]
    public class AudioContainer : ScriptableObject
    {
        public static AudioContainer instance;
        [System.Serializable]
        public struct AudioDirectory
        {
            public List<AudioGroup> audioGroups;
        }

        [System.Serializable]
        public struct AudioGroup
        {
            public string ID;
            public List<AudioList> audioLists;
        }
        [System.Serializable]
        public struct AudioList
        {
            public string ID;
            public List<AudioClip> audioClips;
        }
        public GameObject AudioObject;
        public AudioDirectory AudioLibrary;

        void Awake()
        {
            instance = this;
        }
    }
}