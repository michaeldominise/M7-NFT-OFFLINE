using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchGridOffset : MonoBehaviour {

    [SerializeField] Vector3 offset;

    void Start()
    {
        transform.position += offset;
    }
}
