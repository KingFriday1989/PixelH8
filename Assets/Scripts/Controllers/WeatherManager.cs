using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelH8.Helpers;
using System.Threading.Tasks;

namespace PixelH8.Controllers
{
    public class WeatherManager : MonoBehaviour
    {
        public enum Weather
        {
            Clear,
            PartlyCloudy,
            Cloudy,
            Rain,
        }

        public Weather weather;
        public Weather currentWeather { get; private set; }
        public GameObject CloudObject;
        public List<GameObject> CloudObjects;
        private bool settingSystem;
        [Range(0, 359)] public int WindDirection = 0;
        [Range(0, 100)] public int WindSpeed = 10;

        private float deadParticleRefresh;
        void Start()
        {

        }
        void FixedUpdate()
        {
            if (CloudObjects.Count > 0 & deadParticleRefresh < Time.time)
            {
                deadParticleRefresh = Time.time + 5;
                List<GameObject> objects = new List<GameObject>();
                foreach (var item in CloudObjects)
                {
                    if (!item.activeInHierarchy)
                    {
                        objects.Add(item);
                    }
                }

                if (objects.Count > 0)
                {
                    foreach (var item in objects)
                    {
                        CloudObjects.Remove(item);
                    }
                }
            }

            bool particlesMax = weather == Weather.PartlyCloudy && CloudObjects.Count >= 50 ? true :
                weather == Weather.Cloudy && CloudObjects.Count >= 200 ? true :
                false;
            if (!settingSystem && weather != currentWeather || !settingSystem && currentWeather == weather && !particlesMax)
                    StartCoroutine(CreateClouds());

        }

        [ContextMenu("CreateClouds")]
        IEnumerator CreateClouds()
        {
            settingSystem = true;
            switch (weather)
            {
                case Weather.PartlyCloudy:
                    for (int i = CloudObjects.Count; i < 50; i++)
                    {
                        yield return new WaitForSeconds(0.1f);
                        GenerateCloud();
                    }
                    break;
                case Weather.Cloudy:
                    for (int i = CloudObjects.Count; i < 200; i++)
                    {
                        yield return new WaitForSeconds(0.1f);
                        GenerateCloud();
                    }
                    break;
                default:
                    break;
            }
            currentWeather = weather;
            settingSystem = false;
        }

        void GenerateCloud()
        {
            var pos = FindNextCloudPosition();
            var rot = pos - Vector3.zero;
            var cloud = ObjectPoolManager.SpawnObject(CloudObject, pos, Quaternion.Euler(Vector3.zero));
            var particleForceModual = cloud.GetComponent<ParticleSystem>().velocityOverLifetime;
            var particleWindDirection = DegreeToVector2(WindDirection);
            particleForceModual.enabled = true;
            particleForceModual.space = ParticleSystemSimulationSpace.World;
            particleForceModual.x = particleWindDirection.x + WindSpeed * 0.05f;
            particleForceModual.z = particleWindDirection.y + WindSpeed * 0.05f;
            cloud.transform.forward = rot;
            CloudObjects.Add(cloud);
        }

        Vector3 FindNextCloudPosition(float avoidDistance = 200)
        {
            Vector3 cloudPos = Vector3.zero;
            cloudPos.y = UnityEngine.Random.Range(200, 1000);

            var randXY = RandomPointInAnnulus(new Vector2(1,1), 200, 3000);
            cloudPos.x = randXY.x;
            cloudPos.z = randXY.y;

            while (Physics.CheckSphere(cloudPos, avoidDistance, LayerMask.GetMask("Weather")))
            {
                randXY = RandomPointInAnnulus(new Vector2(1,1), 200, 3000);
                cloudPos.x = randXY.x;
                cloudPos.z = randXY.y;
            }
            return cloudPos;
        }

        public Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
        {

            var randomDirection = (Random.insideUnitCircle * origin).normalized;

            var randomDistance = Random.Range(minRadius, maxRadius);

            var point = origin + randomDirection * randomDistance;

            return point;
        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
    }
}


