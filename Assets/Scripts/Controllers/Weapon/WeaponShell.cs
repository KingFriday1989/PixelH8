using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelH8.Controllers.Weapons
{
    public class WeaponShell : MonoBehaviour
    {
        public AudioClip[] sounds;
        private float timer;
        private bool playSound = true;
        private int bounce;
        public Vector2 velocity = new Vector2(2f, 4f);
        public Vector2 angularVelocity = new Vector2(1000, 2000);
        public Vector2 RandomRangeX = new Vector2(0, 0);
        public Vector2 RandomRangeY = new Vector2(-10, 10);
        public Vector2 RandomRangeZ = new Vector2(0, 0);

        private void Start()
        {
            transform.localRotation =
            Quaternion.Euler(
            transform.localRotation.eulerAngles +
            new Vector3(Random.Range(RandomRangeX.x, RandomRangeX.y),
            Random.Range(RandomRangeY.x, RandomRangeY.y),
            Random.Range(RandomRangeZ.x, RandomRangeZ.y)));

            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = rb.transform.right * Random.Range(velocity.x, velocity.y);
                rb.angularVelocity = new Vector3(0, Random.Range(angularVelocity.x, angularVelocity.y), 0);
            }
            timer = Time.time + 10;
        }

        private void Update()
        {
            if (timer < Time.time)
            {
                ObjectPoolManager.ReturnObjectToPool(gameObject);
            }

            var rb = GetComponent<Rigidbody>() != null ? GetComponent<Rigidbody>() : null;
            if (rb != null && Vector3.Distance(rb.velocity, Vector3.zero) < 0.01f)
                Destroy(rb);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (playSound)
            {
                var audioclip = sounds[UnityEngine.Random.Range(0, sounds.Length)];
                if (audioclip != null)
                    AudioSource.PlayClipAtPoint(audioclip, transform.position, 0.5f);

                if (bounce <= 4)
                    bounce++;
                    else
                    playSound = false;
            }
        }
    }
}