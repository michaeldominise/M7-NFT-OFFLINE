using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
using Newtonsoft.Json;
using M7.ServerTestScripts;

namespace M7
{
    public class ActivationCodeSceneManager : SceneManagerBase
    {
        public static ActivationCodeSceneManager Instance { get; private set; }

        [SerializeField] TMP_InputField activationCodeInput;
        [SerializeField] Button activateButton;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start() => CheckActivationButton();
        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "ActivateButton":
                    Activate();
                    break;
            }
        }

        protected override void ExecuteInputEvent(TMP_InputField input)
        {
            switch (input.name)
            {
                case "ActivationCodeInput":
                    CheckActivationButton();
                    break;
            }
        }

        void CheckActivationButton() => activateButton.interactable = !string.IsNullOrWhiteSpace(activationCodeInput.text);

        void Activate()
        {
            var playFabIfAndActivationCode = new Dictionary<string, string>
            {
                { "playFabId", PlayerDatabase.AccountProfile.PlayFabId },
                { "code", activationCodeInput.text }
            };

            ActivationDone();

            // MessageBox.Create("Activating...", MessageBox.ButtonType.Loading).Show();
            // AzureFunction.ActivateAccount(playFabIfAndActivationCode, OkResult, ErrorResult );
        }

        private void OkResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            
            MessageBox.HideCurrent();
            
            if (response.ResponseResult == ResponseResult.OK)
            {
                ActivationDone();
                return;
            }
            
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Account Activation").Show();
        }

        private void ErrorResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            MessageBox.HideCurrent();
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Account Activation").Show();
        }
        
        private static void ActivationDone()
        {
            PlayerDatabase.AccountProfile.SetPlayerStatus(PlayerStatus.Activated);
            PlayfabDataManager.Instance.Init();
            UnloadAtSceneLayer(Instance.sceneLayer);
        }
    }
}
