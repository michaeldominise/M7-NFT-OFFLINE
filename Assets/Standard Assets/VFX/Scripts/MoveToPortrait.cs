using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPortrait : MonoBehaviour {

	public Transform endPoint;
	public float speed = 1f;
	public AnimationCurve fadeIn;
	public float effectTime = 2f;
	public float pause = 0.75f;

	private Vector3 startPoint;
	private float timer = 0;

	// Use this for initialization
	void Start () {
		startPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer < effectTime + pause)
        {
			if (timer < pause) {
				timer += Time.deltaTime;	
			}
			else {
            	timer += Time.deltaTime;
				transform.position = Vector3.MoveTowards( transform.position, endPoint.position, (fadeIn.Evaluate( timer ) * speed));
			}
        }
        else
        {
            timer = 0;
			transform.position = startPoint;
        }
	}
}
