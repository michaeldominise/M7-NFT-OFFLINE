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
    static string AddNftBoosterUri = $"{AzureFunction.uri}AddNftBooster";

    public enum BoosterType
    {
        Null,
        BoosterObject_Hammer,
        BoosterObject_Blaster,
        BoosterObject_Laser,
        BoosterObject_Shuffle,
    }

    int hammerAmount = 1000;
    [Category("AddBooster_Hammer")] public void AddBooster_Hammer() => GameManager.Instance.StartCoroutine(AddBooster(BoosterType.BoosterObject_Hammer, hammerAmount));
    [Category("AddBooster_Hammer")] public int HammerAmount { get => hammerAmount; set => hammerAmount = value; }

    int blasterAmount = 1000;
    [Category("AddBooster_Blaster")] public void AddBooster_Blaster() => GameManager.Instance.StartCoroutine(AddBooster(BoosterType.BoosterObject_Blaster, blasterAmount));
    [Category("AddBooster_Blaster")] public int BlasterAmount { get => blasterAmount; set => blasterAmount = value; }

    int laserAmount = 1000;
    [Category("AddBooster_Laser")] public void AddBooster_Laser() => GameManager.Instance.StartCoroutine(AddBooster(BoosterType.BoosterObject_Laser, laserAmount));
    [Category("AddBooster_Laser")] public int LaserAmount { get => laserAmount; set => laserAmount = value; }

    int shuffleAmount = 1000;
    [Category("AddBooster_Shuffle")] public void AddBooster_Shuffle() => GameManager.Instance.StartCoroutine(AddBooster(BoosterType.BoosterObject_Shuffle, shuffleAmount));
    [Category("AddBooster_Shuffle")] public int ShuffleAmount { get => shuffleAmount; set => shuffleAmount = value; }

    static IEnumerator AddBooster(BoosterType boosterType, int amount)
    {
        if (string.IsNullOrWhiteSpace(PlayerDatabase.AccountProfile.WalletAddress))
            yield break;

        SRDebug.Instance.HideDebugPanel();
        MessageBox.Create($"AddBooster: {boosterType}.", MessageBox.ButtonType.Loading).Show();

        var nftData = new Dictionary<string, object>();
        //nftData["walletAddress"] = PlayerDatabase.AccountProfile.WalletAddress;
        nftData["hashID"] = PlayerDatabase.AccountProfile.Email;
        nftData["tokenId"] = (int)boosterType;
        nftData["amount"] = amount;

        PlayerDatabase.Inventories.Currencies.FindItem(GetBoosterInstanceId(boosterType)).Amount += amount;
        //yield return AzureFunction.EndPoint(AddNftBoosterUri, nftData, null, null);
        MessageBox.HideCurrent();

        GameManager.RestartGameDialog();
    }

    static string GetBoosterInstanceId(BoosterType boosterType) => boosterType switch
    {
        BoosterType.Null => throw new NotImplementedException(),
        BoosterType.BoosterObject_Hammer => "BoosterObject_Hammer",
        BoosterType.BoosterObject_Blaster => "BoosterObject_Blaster",
        BoosterType.BoosterObject_Laser => "BoosterObject_Hammer",
        BoosterType.BoosterObject_Shuffle => "BoosterObject_Shuffle",
        _ => throw new NotImplementedException(),
    };
}