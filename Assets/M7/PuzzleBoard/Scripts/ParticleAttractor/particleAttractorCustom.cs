using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class particleAttractorCustom : MonoBehaviour
{
	public ParticleSystem ps;
	public Transform target;
	public AnimationCurve lerpCurve = AnimationCurve.Constant(0, 1, 1);
	public float intensity = 2;
	ParticleSystem.Particle[] m_Particles;
	int numParticlesAlive;
	float step;

	private void Start() => m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];

	public void UpdateLifeTimeDuration(float newLifeTimeDuration)
    {
		var mainParticle = ps.main;
		mainParticle.startLifetime = newLifeTimeDuration;
	}

	protected void Update () {
        numParticlesAlive = ps.GetParticles(m_Particles);
		for (int i = 0; i < numParticlesAlive; i++)
		{
			var id = (float)m_Particles[i].randomSeed / uint.MaxValue;
			var degrees = id * 360;
			var radians = degrees * Mathf.Deg2Rad;
			var x = Mathf.Cos(radians);
			var y = Mathf.Sin(radians);
			var pos = transform.position + new Vector3(x, y, 0) * intensity;

			step = 1 - m_Particles[i].remainingLifetime / m_Particles[i].startLifetime;
			m_Particles[i].position = Vector3.Lerp(Vector3.Lerp(transform.position, pos, step), Vector3.Lerp(transform.position, target.position, Mathf.Max(step, 1)), lerpCurve.Evaluate(step));
		}
		ps.SetParticles(m_Particles, numParticlesAlive);
	}
}
