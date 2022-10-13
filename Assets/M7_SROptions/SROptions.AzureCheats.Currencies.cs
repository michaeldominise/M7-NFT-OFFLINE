using M7.GameRuntime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using System.Collections;
using M7.GameData;
using M7;

public partial class SROptions
{
    static string AddNftGaianiteUri = $"{AzureFunction.uri}AddNftGaianite";
    static string AddNftM7Uri = $"{AzureFunction.uri}AddNftM7";

    public enum CurrencyType
    {
        Gaianite,
        M7,
    }

    int gaianiteAmount = 1000;
    [Category("AddCurrency_Gaianite")] public void AddCurrency_Gaianite() => GameManager.Instance.StartCoroutine(AddCurrency(CurrencyType.Gaianite, gaianiteAmount));
    [Category("AddCurrency_Gaianite")] public int GaianiteAmount { get => gaianiteAmount; set => gaianiteAmount = value; }

    int m7Amount = 1000;
    [Category("AddCurrency_M7")] public void AddCurrency_M7() => GameManager.Instance.StartCoroutine(AddCurrency(CurrencyType.M7, m7Amount));
    [Category("AddCurrency_M7")] public int M7Amount { get => m7Amount; set => m7Amount = value; }

    static IEnumerator AddCurrency(CurrencyType currencyType, int amount)
    {
        if (string.IsNullOrWhiteSpace(PlayerDatabase.AccountProfile.WalletAddress))
            yield break;

        SRDebug.Instance.HideDebugPanel();
        MessageBox.Create($"AddCurrency: {currencyType}.", MessageBox.ButtonType.Loading).Show();

        var nftData = new Dictionary<string, object>();
        //nftData["walletAddress"] = PlayerDatabase.AccountProfile.WalletAddress;
        nftData["hashID"] = PlayerDatabase.AccountProfile.Email;
        nftData["amount"] = amount;
        //yield return AzureFunction.EndPoint(GetCurrencyUri(currencyType), nftData, null, null);
        var inventoryDataCurrency = currencyType switch
        {
            CurrencyType.Gaianite => PlayerDatabase.Inventories.SystemCurrencies,
            CurrencyType.M7 => PlayerDatabase.Inventories.Currencies,
            _ => PlayerDatabase.Inventories.Currencies
        };

        inventoryDataCurrency.FindItem(GetCurrencyInstanceId(currencyType)).Amount += amount;
        MessageBox.HideCurrent();

        GameManager.RestartGameDialog();
    }

    static string GetCurrencyUri(CurrencyType currencyType) => currencyType switch
    {
        CurrencyType.Gaianite => AddNftGaianiteUri,
        CurrencyType.M7 => AddNftM7Uri,
        _ => throw new NotImplementedException(),
    };
    
    static string GetCurrencyInstanceId(CurrencyType currencyType) => currencyType switch
    {
        CurrencyType.Gaianite => "Gaianite",
        CurrencyType.M7 => "M7",
        _ => throw new NotImplementedException(),
    };
}