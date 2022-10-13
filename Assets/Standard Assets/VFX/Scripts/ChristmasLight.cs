using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasLight : MonoBehaviour
{
    [SerializeField] ParticleSystem[] _lights;
    [SerializeField] Color _colorOdd = new Color(1, 1, 1, 1), _colorEven = new Color(1, 1, 1, 1);
    GameObject mirrorLight;

    // Start is called before the first frame update
    void Awake()
    {
        if (_lights != null)
            ApplyColor();
    }

    void ApplyColor()
    {
        for (int i = 0; i < _lights.Length; i++)
        {
            var litefx = _lights[i].main;
            if (i % 2 == 0)
            {
                litefx.startColor = _colorEven;
                litefx.startDelay = litefx.startLifetime;
            }
            else
                litefx.startColor = _colorOdd;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
