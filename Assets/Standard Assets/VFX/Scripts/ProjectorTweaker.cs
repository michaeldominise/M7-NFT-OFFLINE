using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class ProjectorTweaker : MonoBehaviour
{
    [Range(0,2)] public float _tweakAlpha;
    public float _speed = 1;
    Projector _projector;
    Material _mat;
    // Use this for initialization
    void Awake()
    {
        _mat = GetComponent<Projector>().material;
        _mat.SetFloat("_Glow", _tweakAlpha);
    }
            
    void Update()
    {
        _mat.SetFloat("_Glow", _tweakAlpha);
    }
}
