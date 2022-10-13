using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace M7
{
    public partial class LoginWithPasswordSceneManager : SceneManagerBase
    {
        public static LoginWithPasswordSceneManager Instance { get; private set; }

        [SerializeField] AssetReference loginWithCodeScene;
        [SerializeField] AssetReference activationCodeScene;
        [SerializeField] TMP_InputField emailInput;
        [SerializeField] TMP_InputField passwordInput;
        [SerializeField] Button loginButton;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start() => CheckLoginButton();

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch(gameObject.name)
            {
                case "LoginButton":
                    LoginWithPassword(emailInput.text, passwordInput.text);
                    break;
                case "LoginWithCodeButton":
                    LoadScene(loginWithCodeScene, LoadSceneMode.Additive, overwriteSceneLayer: sceneLayer);
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
                case "PasswordInput":
                    CheckLoginButton();
                    break;
            }
        }

        void CheckLoginButton() => loginButton.interactable = !string.IsNullOrWhiteSpace(emailInput.text) && !string.IsNullOrWhiteSpace(passwordInput.text);
    }
}
