using M7.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace M7
{
    public partial class Settings_PlayerProfile_DeleteAccountSceneManager : SceneManagerBase
    {
        public static Settings_PlayerProfile_DeleteAccountSceneManager Instance { get; private set; }

        [SerializeField] VerificationCode verificationCode;
        [SerializeField] TMP_InputField emailVerificationCodeInput;
        [SerializeField] TextMeshProUGUI deleteMessage;
        [SerializeField] Button confirmButton;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start()
        {
            deleteMessage.text = string.Format(deleteMessage.text, PlayerDatabase.AccountProfile.Email);
            CheckConfirmButton();
        }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;
                case "SendCodeButton":
                    if (verificationCode.IsReady)
                        verificationCode.OnClick(PlayerDatabase.AccountProfile.Email, VerificationCode.VerificationType.Deletion);
                    break;
                case "ConfirmButton":
                    DeleteAccount();
                    break;
            }
        }

        protected override void ExecuteInputEvent(TMP_InputField input)
        {
            switch (input.name)
            {
                case "EmailVerificationCodeInput":
                    CheckConfirmButton();
                    break;
            }
        }

        void CheckConfirmButton() => confirmButton.interactable = !string.IsNullOrWhiteSpace(emailVerificationCodeInput.text);
    }
}