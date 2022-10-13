using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

    /// <summary>
    /// Component wrangler for MatchGrid tiles.
    /// </summary>
    public class hoverfx : MonoBehaviour
    {
        [SerializeField] float speed = 0.1f;
		[SerializeField] float intensity = 1;
        [ShowInInspector, ReadOnly] public float randomSeedY { get; private set; }
        [ShowInInspector, ReadOnly] public float randomSeedX { get; private set; }
        float originalSpeed = 0.1f;
        float originalintensity = 0.1f;
        Vector3 localPos;

		[Button]
        public void RandomizeSeed()
        {
            randomSeedY = Random.value;
            randomSeedX = Random.value;
        }

        void Awake()
        {
            RandomizeSeed();
			originalSpeed = speed;
            localPos = transform.localPosition;
		}

        public void SetValues(float speed, float intensity)
		{
			this.speed = speed;
			this.intensity = intensity;
        }

		public void ResetValues()
		{
			speed = originalSpeed;
            intensity = originalintensity;
        }

		public void Update()
        {
            localPos.x += (Mathf.PerlinNoise(Time.time * randomSeedX * speed, Time.time * speed) - 0.5f) * intensity;
            localPos.y += (Mathf.PerlinNoise(Time.time * randomSeedY * speed, Time.time * speed) - 0.5f) * intensity;
            transform.localPosition = localPos;
        }
    }
