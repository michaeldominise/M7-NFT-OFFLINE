using M7;
using M7.ServerTestScripts;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPProductIDManager : MonoBehaviour
{
    public enum InAppType { Pack, Character, Ticket}
    public void PurchaseInApp(InAppType inAppType, IAPButton iAPButton)
    {
        switch(inAppType)
        {
            case InAppType.Pack:
                OnPurchasePackSuccess(iAPButton);
                break;
            case InAppType.Character:
                OnPurchaseCharacterSuccess(iAPButton);
                break;
            case InAppType.Ticket:
                break;
        }
    }
    public void OnPurchasePackSuccess(IAPButton iAPButton)
    {
        string productIds = iAPButton.productId;
        DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.PurchasePack, productIds);
    }

    public void OnPurchaseCharacterSuccess(IAPButton iAPButton)
    {
        //string productIds = iAPButton.productId;
        DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.PurchaseChar);
    }

    public void OnPurchaseFailed()
    {
        MessageBox.Create("Purchase Failed", MessageBox.ButtonType.Ok);
    }
}
