using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_AutoDestruct : MonoBehaviour
{
    [SerializeField] float delay = 1;
    [SerializeField] bool disableOnly = true;

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(delay);
        if (disableOnly)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
}
