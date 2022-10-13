using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaiSpriteActivator : MonoBehaviour
{
    [SerializeField] GameObject gaiSpriteObj;

    private void OnEnable()
    {
        gaiSpriteObj.SetActive(true);
    }
}
