using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using DG.Tweening;

namespace M7
{
    public partial class SetPasswordSceneManager : SceneManagerBase
    {
        public static SetPasswordSceneManager Instance { get; private set; }

        [SerializeField] AssetReference activationCodeScene;
        [SerializeField] TMP_InputField passwordInput;
        [SerializeField] TMP_InputField confirmPasswordInput;
        [SerializeField] Button nextButton;
        [SerializeField] CanvasGroup passwordNotTheSamePanel;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start() => CheckNextButton();

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "NextButton":
                    checkSamePassword();
                    break;
            }
        }

        protected override void ExecuteToggleEvent(Toggle toggle)
        {
            switch (toggle.name)
            {
                case "ShowPassToggle1":
                    passwordInput.inputType =
                        toggle.isOn ? TMP_InputField.InputType.Standard : TMP_InputField.InputType.Password;
                    passwordInput.ForceLabelUpdate();
                    break;
                case "ShowPassToggle2":
                    confirmPasswordInput.inputType =
                        toggle.isOn ? TMP_InputField.InputType.Standard : TMP_InputField.InputType.Password;
                    confirmPasswordInput.ForceLabelUpdate();
                    break;
            }
        }

        protected override void ExecuteInputEvent(TMP_InputField input)
        {
            switch (input.name)
            {
                case "PasswordInput":
                case "ConfirmPasswordInput":
                    CheckNextButton();
                    break;
            }
        }

        void CheckNextButton() => nextButton.interactable = !string.IsNullOrWhiteSpace(passwordInput.text) &&
                                                            !string.IsNullOrWhiteSpace(confirmPasswordInput.text);
        
        private void checkSamePassword()
        {
            if(passwordInput.text == confirmPasswordInput.text)
            {
                CreateAccount();
            }
            else
            {
                //MessageBox.Create("Passwords are not the same", MessageBox.ButtonType.Ok).Show();

                passwordNotTheSamePanel.DOFade(1, 0.5f).onComplete += () => passwordNotTheSamePanel.DOFade(0, 0.5f).SetDelay(2f);
            }
        }
    }
}