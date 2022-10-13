using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager
{
    public IAPManager()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct("100_gold_coins", ProductType.Consumable);
        // Initialize Unity IAP...
    }
}