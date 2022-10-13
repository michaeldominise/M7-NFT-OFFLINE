using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace M7
{
    public partial class Settings_PlayerProfile_EmailSceneManager : SceneManagerBase
    {
        public static Settings_PlayerProfile_EmailSceneManager Instance { get; private set; }

        [SerializeField] VerificationCode verificationCode;
        [SerializeField] TMP_InputField emailInput;
        [SerializeField] TMP_InputField emailVerificationCodeInput;
        [SerializeField] TMP_InputField passwordInput;
        [SerializeField] Button saveButton;

        protected override void Awake() 
        { 
            Instance = this;
            base.Awake();
        }

        private void Start() => CheckSaveButton();

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;
                case "SendCodeButton":
                    if (verificationCode.IsReady && !string.IsNullOrWhiteSpace(emailInput.text))
                    {
                        verificationCode.OnClick(emailInput.text, VerificationCode.VerificationType.Login);
                    }
                    break;
                case "SaveButton":
                    UpdateEmail();
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
                case "EmailInput":
                case "EmailVerificationCodeInput":
                case "PasswordInput":
                    CheckSaveButton();
                    break;
            }
        }

        void CheckSaveButton() => saveButton.interactable = !string.IsNullOrWhiteSpace(emailInput.text) && !string.IsNullOrWhiteSpace(emailVerificationCodeInput.text) && !string.IsNullOrWhiteSpace(passwordInput.text);
    }
}