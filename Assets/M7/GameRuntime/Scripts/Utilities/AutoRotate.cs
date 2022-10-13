using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {

    [SerializeField] Vector3 _rotationVec;
	[SerializeField] float rotationInterval = 0.1f;

    private void OnEnable()
    {
		StartCoroutine(Rotate());
    }

    IEnumerator Rotate () {
		while (gameObject.activeInHierarchy)
		{
			yield return new WaitForSeconds(rotationInterval);
			transform.Rotate(_rotationVec);
		}
	}
}
