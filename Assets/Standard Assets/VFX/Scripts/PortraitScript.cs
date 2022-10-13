using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitScript : MonoBehaviour {
	private Vector3[] vertices = new Vector3[4];
	private ParticleSystem[] effects;
	private int target = 1;
	private float ratio;
	private float step;
	private bool longerY;

	public float duration = 1f;
	public float startLifetime = 1f;	
	public float speed;
	public float emissionRate;
	public Gradient colorOverLifetime;
	public float sizeMultiplier = 1f;
	public AnimationCurve sizeOverLifetime;
	public Material material;

	// Use this for initialization
	void Start () {
		effects = GetComponentsInChildren<ParticleSystem>();

		vertices[0] = new Vector3 (-0.5f, -0.5f, 0);
		vertices[1] = new Vector3 (-0.5f, 0.5f, 0);
		vertices[2] = new Vector3 (0.5f, 0.5f, 0);
		vertices[3] = new Vector3 (0.5f, -0.5f, 0);
		
		for (int i = 0; i < effects.Length; i++) {
			Initialize(effects[i]);
			effects[i].transform.localPosition = vertices[i];	
		}

		ratio = GetRatio();
		GetSpeed();
	}

	void Initialize(ParticleSystem effect) {
		effect.Stop();
		var main = effect.main;
		main.duration = duration;
		main.startLifetime = startLifetime;
		effect.Play();

		var col = effect.colorOverLifetime;
		col.enabled = true;
		col.color = colorOverLifetime;

		var sz = effect.sizeOverLifetime;
		sz.enabled = true;
		sz.size = new ParticleSystem.MinMaxCurve(sizeMultiplier, sizeOverLifetime);

		var shape = effect.shape;
		shape.enabled = false;

		var emission = effect.emission;
		emission.rateOverTime = emissionRate;

		effect.GetComponent<ParticleSystemRenderer>().maxParticleSize = 1.0f;
		effect.GetComponent<ParticleSystemRenderer>().material = material;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		effects[0].transform.localPosition = Vector3.MoveTowards(effects[0].transform.localPosition, vertices[(target) % 4], step);
		effects[1].transform.localPosition = Vector3.MoveTowards(effects[1].transform.localPosition, vertices[(target + 1) % 4], step);
		effects[2].transform.localPosition = Vector3.MoveTowards(effects[2].transform.localPosition, vertices[(target + 2) % 4], step);
		effects[3].transform.localPosition = Vector3.MoveTowards(effects[3].transform.localPosition, vertices[(target + 3) % 4], step);

		if((effects[0].transform.localPosition-vertices[(target) % 4]).sqrMagnitude < 0.0001) {
			target++;
			target %= 4;

			GetSpeed();	
		}
	}

	float GetRatio() {
		if (transform.localScale.x > transform.localScale.y) {
			longerY = false;
			return transform.localScale.x / transform.localScale.y;
		}
		else {
			longerY = true;
			return transform.localScale.y / transform.localScale.x;
		}
	}

	void GetSpeed() {
		if ((target % 2) == 0) {
			if (longerY) {
				step = speed * ratio * Time.deltaTime;
			}
			else {
				step = speed * Time.deltaTime;
			}
		}
		else {
			if (longerY) {
				step = speed * Time.deltaTime;
			}
			else {
				step = speed * ratio * Time.deltaTime;
			}
		}
	}

#if UNITY_EDITOR
	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(1,1,0));
		Gizmos.DrawLine(Vector3.zero,(-Vector3.forward / 5f));
		Gizmos.DrawLine(Vector3.zero,(Vector3.up / 5f));
		Gizmos.DrawLine(Vector3.zero,(Vector3.right / 5f));
	}
#endif
}
