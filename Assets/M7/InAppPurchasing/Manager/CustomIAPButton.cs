using M7;
using M7.ServerTestScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;


public class CustomIAPButton : IAPButton
{
    [SerializeField] string theProductID;
    public void Awake() => productId = theProductID;
}
