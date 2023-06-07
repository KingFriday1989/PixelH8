using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Helpers;

namespace PixelH8.Helpers
{
    public static class AudioTools
    {
        public static GameObject SpawnAudio(GameObject AudioObject, Transform parent, AudioClip clip, float volume = 0.75f, int maxDistance = 10, float pitchMin=1, float pitchMax=1/*,bool nonPriority = false*/)
        {
            var audPoint = ObjectPoolManager.SpawnObject(AudioObject, parent);
            var audSrc = audPoint.GetComponent<AudioSource>();
            audPoint.GetComponent<AutoReturn>().returnDelay = clip.length;
        
            audSrc.clip = clip;
            audSrc.volume = volume;
            audSrc.maxDistance = maxDistance;
            audSrc.pitch = UnityEngine.Random.Range(pitchMin,pitchMax);
            audSrc.Play();
            return audPoint;
        }
        
        public static GameObject SpawnAudio(GameObject AudioObject, AudioClip clip, Vector3 pos, float volume = 0.75f, int maxDistance = 10, float pitchMin=1, float pitchMax=1/*,bool nonPriority = false*/)
        {
            var audPoint = ObjectPoolManager.SpawnObject(AudioObject, pos, Quaternion.Euler(Vector3.zero));
            var audSrc = audPoint.GetComponent<AudioSource>();
            audPoint.GetComponent<AutoReturn>().returnDelay = clip.length;
        
            audSrc.clip = clip;
            audSrc.volume = volume;
            audSrc.maxDistance = maxDistance;
            audSrc.pitch = UnityEngine.Random.Range(pitchMin,pitchMax);
            audSrc.Play();
            return audPoint;
        }

        public static AudioClip ReturnRandomClip(List<AudioClip> clipList)
        {
            if(clipList[0] == null) return null;
            
            AudioClip clip;
            if(clipList.Count > 1)
                clip = clipList[UnityEngine.Random.Range(0,clipList.Count)];
            else
                clip = clipList[0];

            return clip;
        }
    }
}