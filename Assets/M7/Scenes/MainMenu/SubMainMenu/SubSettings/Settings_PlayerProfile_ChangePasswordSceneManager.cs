using M7.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace M7
{
    public partial class Settings_PlayerProfile_ChangePasswordSceneManager : SceneManagerBase
    {
        public static Settings_PlayerProfile_ChangePasswordSceneManager Instance { get; private set; }

        [SerializeField] VerificationCode verificationCode;
        [SerializeField] TextMeshProUGUI emailAdd;
        [SerializeField] TMP_InputField emailVerificationCodeInput;
        [SerializeField] TMP_InputField passwordInput;
        [SerializeField] Button saveButton;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start()
        {
            emailAdd.text = PlayerDatabase.AccountProfile.Email;
            CheckSaveButton();
        }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;
                case "SendCodeButton":
                    verificationCode.OnClick(PlayerDatabase.AccountProfile.Email, VerificationCode.VerificationType.Login);
                    break;
                case "SaveButton":
                    UpdatePassword();
                    break;
            }
        }

        protected override void ExecuteToggleEvent(Toggle toggle)
        {
            switch (toggle.name)
            {
                case "ShowPassToggle":
                    passwordInput.inputType = toggle.isOn ? TMP_InputField.InputType.Standard : TMP_InputField.InputType.Password;
                    passwordInput.ForceLabelUpdate();
                    break;
            }
        }

        protected override void ExecuteInputEvent(TMP_InputField input)
        {
            switch (input.name)
            {
                case "EmailVerificationCodeInput":
                case "PasswordInput":
                    CheckSaveButton();
                    break;
            }
        }

        void CheckSaveButton() => saveButton.interactable = !string.IsNullOrWhiteSpace(emailVerificationCodeInput.text) && !string.IsNullOrWhiteSpace(passwordInput.text);
    }
}