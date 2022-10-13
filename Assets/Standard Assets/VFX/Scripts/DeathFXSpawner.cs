using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor;

public class DeathFXSpawner : MonoBehaviour
{
    List<Transform> _FXRoot;
    ParticleSystem _FX;

    string _fXPath = "vfx_death_generic_fx";
    PositionConstraint[] obj;

    // Start is called before the first frame update
    void Start()
    {
        if (_FX == null)
            _FX = Resources.Load<ParticleSystem>(_fXPath);

        obj = transform.GetComponentsInChildren<PositionConstraint>(true);

        if (obj != null)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                obj[i].translationOffset = Vector3.zero;
                if (obj[i].transform.childCount > 0)
                    Destroy(obj[i].transform.GetChild(0).gameObject);
            }
        }
    }

    public void SpawnDeathFX()
    {
        foreach (var root in obj)
        {
            if (_FX != null)
            {
                var fx = Instantiate(_FX.gameObject);
                fx.transform.SetParent(root.transform, false);
                fx.transform.SetPositionAndRotation(root.transform.position, root.transform.rotation);
            }
        }
    }
}
