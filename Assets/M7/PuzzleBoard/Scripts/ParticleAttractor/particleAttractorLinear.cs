using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class particleAttractorLinear : MonoBehaviour {
	protected ParticleSystem ps;
	protected ParticleSystem.Particle[] m_Particles;
	public Transform target;
	public float speed = 5f;
	public bool useSpeed = true;
	protected int numParticlesAlive;


	protected virtual void Start () {
		ps = GetComponent<ParticleSystem>();
		if (!GetComponent<Transform>()){
			GetComponent<Transform>();
		}
        m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }


    protected virtual void Update () {
	    numParticlesAlive = ps.GetParticles(m_Particles);
		var step = speed * Time.deltaTime;
		for (var i = 0; i < numParticlesAlive; i++) {
			
			if (useSpeed)
			{
				m_Particles[i].position = Vector3.LerpUnclamped(m_Particles[i].position, target.position, step);
				continue;
			}

			m_Particles[i].position = target.position;
		}
		ps.SetParticles(m_Particles, numParticlesAlive);
	}
}
