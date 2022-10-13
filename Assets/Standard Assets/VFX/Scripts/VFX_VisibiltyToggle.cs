using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VFX_VisibiltyToggle : MonoBehaviour
{
    [SerializeField] bool reverseToggle;
    [SerializeField] GameObject[] objToToggle;

    bool active;

    private void OnEnable()
    {
        active = true;
        if (objToToggle != null)
            foreach (var obj in objToToggle)
                obj.SetActive(reverseToggle ? !active : active);
    }

    private void OnDisable()
    {
        active = false;
        if (objToToggle != null)
            foreach (var obj in objToToggle)
                obj.SetActive(reverseToggle ? !active : active);
    }
}
