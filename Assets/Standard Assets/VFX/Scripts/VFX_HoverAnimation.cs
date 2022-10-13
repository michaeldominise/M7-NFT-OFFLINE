using System.Collections;
using UnityEngine;

public class VFX_HoverAnimation : MonoBehaviour
{
    enum HoverDirection
    {
        Vertical,
        Horizontal
    }

    [SerializeField] Vector3 rotateDirection;
    [SerializeField] HoverDirection hoverDirection;
    [SerializeField] float _hoverSpeed = 1;
    [Range(0.1F, 1)]
    [SerializeField] float _damping = 1;

    Vector3 postPosition;
    Vector3 postDirection()
    {
        Vector3 dir = Vector3.zero;
        if (hoverDirection == HoverDirection.Horizontal)
            dir = Vector3.right;
        else if (hoverDirection == HoverDirection.Vertical)
            dir = Vector3.up;

        return dir;
    }

    private void OnEnable()
    {
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        postPosition = transform.parent.position;
        while (true)
        {
            transform.eulerAngles += rotateDirection * Time.deltaTime;
            transform.position = (Mathf.Sin(Time.time * _hoverSpeed) * postDirection() * _damping) + postPosition;
            yield return null;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
