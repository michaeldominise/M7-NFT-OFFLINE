using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VFX_RewardsAttractor : MonoBehaviour
{
	protected ParticleSystem ps { get { return GetComponent<ParticleSystem>(); } }
	protected Material mainMaterial { get { return ps.GetComponent<ParticleSystemRenderer>().material; } }
	protected ParticleSystemRenderer childfx(Transform childParticle) { return childParticle.GetComponent<ParticleSystemRenderer>(); }
	protected ParticleSystem.Particle[] m_Particles;
	protected Sequence seq;

	public Transform startPoint, endTarget, subparticle;
	public ParticleSystem rewardparticle, endVFX;
	public bool standAlone = true;
	public int MaxParticles = 10;
	public float travelSpeed = 1, attractDelay = 1;
	[SerializeField] float blendCurve = 100;
	[Range(0, 1)]
	[SerializeField] float blendPosition = .5F;

	public bool isSetToZeroPos;
	public int numParticlesAlive { get; set; }
	public Vector3 endpos { get; set; }
	public bool isDone { get; set; }

	private void OnEnable()
	{
		isDone = false;

		if (standAlone)
		{
			if (isSetToZeroPos)
			{
				gameObject.transform.position = new Vector3(0, 0, 10);
			}
			SpawnReward(MaxParticles);
		}
	}

	public void SpawnReward(int rewardValue)
	{
		gameObject.SetActive(true);
		DOTween.Kill(this);
		StopAllCoroutines();

		rewardValue = Mathf.Clamp(rewardValue, 0, 10);
		ps.maxParticles = rewardValue;
		//attractDelay = ps.startLifetime;

		Spawn();
	}

	protected virtual void Spawn()
	{
		for (int i = 0; i < ps.maxParticles; i++)
		{
			if (subparticle.childCount < ps.maxParticles)
			{
				GameObject child = Instantiate(rewardparticle.gameObject, subparticle, false);
				child.SetActive(false);
			}
		}

		if (endVFX != null)
		{
			endVFX.transform.SetParent(endTarget, false);
			endVFX.transform.SetPositionAndRotation(endTarget.position, endTarget.rotation);
			endVFX.gameObject.SetActive(true);
		}

		m_Particles = new ParticleSystem.Particle[ps.maxParticles];
		StartCoroutine(AttractParticle());
	}

	protected virtual IEnumerator AttractParticle()
	{
		yield return new WaitForSeconds(attractDelay);
		numParticlesAlive = ps.GetParticles(m_Particles);
		endpos = endTarget.position;
		seq = DOTween.Sequence();

		for (int i = 0; i < numParticlesAlive; i++)
		{
			Transform childParticle = subparticle.GetChild(i);
			childfx(childParticle).material = mainMaterial;
			childParticle.localScale = Vector3.one;
			childParticle.position = m_Particles[i].position;
			m_Particles[i].size = 0;
			childParticle.gameObject.SetActive(true);

			seq.PrependInterval(.025F);
			seq.Join(childParticle.DOMove(endpos, travelSpeed)
				.SetEase(Ease.OutSine)
				.OnComplete(delegate ()
				{
                    childParticle.gameObject.SetActive(false);
                    PunchScaleTarget();
				}));
			seq.Join(childParticle.DOScale(Vector3.one * .5F, travelSpeed).SetEase(Ease.InCubic));
			seq.Insert(.25F, childParticle.DOBlendableMoveBy(childParticle.position + (Random.insideUnitSphere * blendCurve), travelSpeed));

		}

		ps.SetParticles(m_Particles, numParticlesAlive);
		yield return new WaitForSeconds(seq.Duration());

		yield return new WaitForSeconds(1);
		gameObject.SetActive(false);
		StopAllCoroutines();
	}

	protected virtual void PunchScaleTarget()
	{
		DOTween.Kill(this);

		if (endVFX != null)
			endVFX.Play();

		endTarget.localScale = Vector3.one * 1.3F;
		endTarget.DOScale(Vector3.one, .1F);

	}

    private void OnDisable()
    {
		isDone = true;
    }
}
