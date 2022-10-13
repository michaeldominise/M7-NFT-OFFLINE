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
    public class WalletManager : MonoBehaviour
    {
        public enum ChainId
        {
            BscMainNet = 56,
            BscTestNet = 97
        }

        [SerializeField] GameObject inGameCurrenciesPanel;
        [SerializeField] GameObject loginWalletPanel;
        [SerializeField] GameObject qrCodePanel;
        [SerializeField] GameObject walletOptions;
        [SerializeField] GameObject connectOptions;
        [SerializeField] TextMeshProUGUI walletIDLabel;
        [SerializeField] ChainId chainId = ChainId.BscTestNet;
        [SerializeField] WalletConnect walletConnect;
        [SerializeField] WalletManager_Accounts walletManager_Accounts;
        [ShowInInspector, ReadOnly] static bool IsReady { get => GameManager.IsInteractable; set => GameManager.IsInteractable = value; }
        bool requestDisconnectWallet;
        bool requestAddWallet;

        private Coroutine timerRoutine;
        
        private void Awake()
        {
            qrCodePanel.SetActive(false);
            walletIDLabel.gameObject.SetActive(false);
            walletConnect.chainId = (int)chainId;
            if (!string.IsNullOrWhiteSpace(PlayerDatabase.AccountProfile.WalletAddress))
                ShowWalletDetails();
        }

        async void Start()
        {
            if(walletConnect.gameObject.activeInHierarchy)
                await walletConnect.Connect();
        }

        public void ExecuteButton(GameObject gameObject)
        {
            if (!IsReady)
                return;

            EnableTeamSelector(true);
            switch (gameObject.name)
            {
                case "WalletButton":
                    EnableTeamSelector(false);
                    walletOptions.SetActive(true);
                    break;
                case "CopyAddressButton":
                    CopyAddress();
                    break;
                case "ChangeWalletButton":
                    break;
                case "DisconnectButton":
                    requestDisconnectWallet = true;
                    DisconnectSession(); 
                    break;
                case "ConnectWalletButton":
                    EnableTeamSelector(false);
                    connectOptions.SetActive(true);
                    break;
                case "MetamaskButton":
                    HandleAuthButtonClick(WalletConnectSharp.Unity.Models.Wallets.MetaMask);
                    break;
                case "WalletConnectButton":
                    HandleAuthButtonClick(WalletConnectSharp.Unity.Models.Wallets.MetaMask);
                    break;
            }
        }
        public void EnableTeamSelector(bool enable)
        {
            MenuTeamSelector.Interactable = enable;
        }

        public void HandleAuthButtonClick(WalletConnectSharp.Unity.Models.Wallets wallet)
        {
            Debug.Log($"[HandleAuthButtonClick]{Time.time}");
            walletConnect.DefaultWallet = wallet;
            if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                qrCodePanel.SetActive(true);
            else
                walletConnect.OpenDeepLink();

            // start timer here
#if !UNITY_EDITOR
            StartWalletTimer();
#endif
        }

        public void WalletConnectSessionEstablished(WalletConnectUnitySession session) 
        {
            Debug.Log($"[WalletConnectSessionEstablished]{Time.time}");
        }


        private WaitForSeconds _walletWaitingTime = new WaitForSeconds(30f);
        IEnumerator WalletTimer()
        {
#if UNITY_ANDROID || UNITY_IOS
            MessageBox.Create("Connecting Wallet", MessageBox.ButtonType.CancelableLoading, title: "Wallet Connect").Show();
#endif
            Debug.Log("Wallet Timer");
            yield return _walletWaitingTime;
#if UNITY_ANDROID || UNITY_IOS
            MessageBox.HideCurrent();
#endif
            MessageBox.Create("Make sure you have the proper wallet installed!", MessageBox.ButtonType.Ok,"Wallet Connect").Show();
        }

        private void StartWalletTimer()
        {
            timerRoutine = StartCoroutine(WalletTimer());
        }

        private void StopWalletTimer()
        {
            StopCoroutine(timerRoutine);
            MessageBox.HideCurrent();
        }
        
        public void WalletConnectHandler(WCSessionData data)
        {
            Debug.Log($"[WalletConnectHandler]{Time.time}");
            if (requestAddWallet)
                return;

            if (!string.IsNullOrEmpty(PlayerDatabase.AccountProfile.WalletAddress))
            {
                ShowWalletDetails();
                return;
            }

            requestAddWallet = true;
            qrCodePanel.SetActive(false);
            if (data.accounts.Length == 0)
                return;
            if (data.accounts.Length > 1)
                walletManager_Accounts.Show(data.accounts, OnAccountWalletClicked);
            else
                OnAccountWalletClicked(data.accounts[0]);
        }

        async void OnAccountWalletClicked(string address)
        {
            Debug.Log($"[OnAccountWalletClicked]{Time.time}");
            //var serverTime = new PlayFab.ClientModels.GetTimeResult().Time.Ticks;
            var messageBox = MessageBox.Create($"Sending sign request for {AccountProfile.GetShortWalletAddress(address)}.", MessageBox.ButtonType.Loading).Show();
            try
            {
                var signMessage = $"{Application.productName} Authentication\n\n{PlayerDatabase.AccountProfile.PlayFabId}:{DateTime.UtcNow.Ticks}";
                var response = await walletConnect.Session.EthPersonalSign(address, signMessage);

                // stop wallet timer coroutine
#if !UNITY_EDITOR
                StopWalletTimer();
#endif
                AddWallet(address);
            }
            catch (Exception e)
            {
                messageBox.InitValues(e.Message, MessageBox.ButtonType.Ok).Show();
            }
            DisconnectSession();
        }

        private async void DisconnectSession()
        {
            await walletConnect.Session.Disconnect();
            walletConnect.ClearSession();
            requestAddWallet = false;
        }

        public void AddWallet(string walletAddress = null)
        {
            if (string.IsNullOrWhiteSpace(walletAddress)) return;

            var wallet = new Dictionary<string, string>
            {
                { "email", PlayerDatabase.AccountProfile.Email },
                { "walletAddress", walletAddress },
                { "sessionTicket", PlayerDatabase.AccountProfile.SessionTicket }
            };
            MessageBox.HideCurrent();
            //MessageBox.Create("Saving wallet details.", MessageBox.ButtonType.Loading).Show();
            PlayerDatabase.AccountProfile.SetWalletAddress(walletAddress);
            GameManager.RestartGameDialog(false);
            //AzureFunction.AddWallet(wallet, OkResult, ErrorResult);
        }

        private void OkResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);

            MessageBox.HideCurrent();
            if (response.ResponseResult == ResponseResult.OK)
            {
                PlayerDatabase.AccountProfile.SetWalletAddress(response.Data.ToString());
                GameManager.RestartGameDialog(false);
            }
            else
                MessageBox.Create(response.Message, MessageBox.ButtonType.Ok).Show();
        }

        public void ShowWalletDetails()
        {
            walletIDLabel.text = PlayerDatabase.AccountProfile.ShortWalletAddress;
            qrCodePanel.SetActive(false);
            inGameCurrenciesPanel.SetActive(true);
            loginWalletPanel.SetActive(false);  
            walletIDLabel.gameObject.SetActive(true);
        }

        public void WalletConnectionFailedEvent(WalletConnectUnitySession session) => ErrorResult(null);

        private void ErrorResult(string obj)
        {
            walletConnect.ClearSession();
            MessageBox.HideCurrent();
            MessageBox.Create("Error connecting wallet.", MessageBox.ButtonType.Ok, "Wallet Connect").Show();
        }

        public void HandleWalletDisconnected()
        {
            if (!requestDisconnectWallet)
                return;

            requestDisconnectWallet = true;
            MessageBox.Create("Disconnecting wallet.", MessageBox.ButtonType.Loading).Show();
            var wallet = new Dictionary<string, string>
            {
                { "email", PlayerDatabase.AccountProfile.Email },
                { "sessionTicket", PlayerDatabase.AccountProfile.SessionTicket }
            };
            AzureFunction.RemoveWallet(wallet, WalletRemoveOkResult, WalletRemoveErrorResult);
        }
        
        private void WalletRemoveOkResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);

            MessageBox.HideCurrent();
            if (response.ResponseResult == ResponseResult.OK)
            {
                PlayerDatabase.AccountProfile.SetWalletAddress("");
                GameManager.RestartGameDialog(false);
            }
            else
                MessageBox.Create(response.Message, MessageBox.ButtonType.Ok).Show();
        }
        
        private void WalletRemoveErrorResult(string obj)
        {
            MessageBox.HideCurrent();
            MessageBox.Create("Error removing wallet", MessageBox.ButtonType.Ok, "Wallet Connect");
        }

        private void CopyAddress() {
            GUIUtility.systemCopyBuffer = PlayerDatabase.AccountProfile.WalletAddress;
        }
    }
}
