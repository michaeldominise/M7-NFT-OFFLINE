using M7;
using M7.ServerTestScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class GetIAPButton: MonoBehaviour
{
    [SerializeField] IAPProductIDManager iAPProductIDManager;
    [SerializeField] IAPButton iAPButton;

    public IAPProductIDManager IAPProductIDManager => iAPProductIDManager;
    public void OnPurchaseInit(int inAppTypeIdx)
    {
        IAPProductIDManager.InAppType inAppType = IAPProductIDManager.InAppType.Pack;

        switch (inAppTypeIdx)
        {
            case 0:
                inAppType = IAPProductIDManager.InAppType.Pack;
                break;
            case 1:
                inAppType = IAPProductIDManager.InAppType.Character;
                break;
            case 2:
                inAppType = IAPProductIDManager.InAppType.Ticket;
                break;
        }

        IAPProductIDManager.PurchaseInApp(inAppType, iAPButton);
    }
    public void OnPurchaseFailed()
    {
        MessageBox.Create("Purchase Failed", MessageBox.ButtonType.Ok);
    }
}
