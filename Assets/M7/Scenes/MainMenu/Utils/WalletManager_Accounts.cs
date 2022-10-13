using M7.GameData;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using M7.GameRuntime;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using UnityEngine.Events;

//WalletConnect
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

namespace M7
{
    public class WalletManager_Accounts : MonoBehaviour
    {
        [SerializeField] WalletManager_AccountItem pref;
        [SerializeField] Transform container;
        [SerializeField] List<WalletManager_AccountItem> accountItems;

        public void Show(string[] walletAccounts, Action<string> onClick)
        {
            gameObject.SetActive(true);
            for(var i = 0; i < walletAccounts.Length || i < accountItems.Count; i++)
            {
                if (i < walletAccounts.Length)
                {
                    var walletAccount = walletAccounts[i];
                    if (i == accountItems.Count)
                        accountItems.Add(Instantiate(pref, container));
                    accountItems[i].Init($"Account{i + 1}: {AccountProfile.GetShortWalletAddress(walletAccount)}", () => onClick?.Invoke(walletAccount), i < walletAccounts.Length);
                }
                else
                    accountItems[i].gameObject.SetActive(false);
            }
        }
    }
}
