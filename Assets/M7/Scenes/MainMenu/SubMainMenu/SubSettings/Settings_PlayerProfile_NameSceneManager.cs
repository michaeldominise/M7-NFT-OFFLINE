using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace M7
{
    public partial class Settings_PlayerProfile_NameSceneManager : SceneManagerBase
    {
        public static Settings_PlayerProfile_NameSceneManager Instance { get; private set; }

        [SerializeField] TMP_InputField usernameInput;
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
                case "SaveButton":
                    ChangeDisplayName();
                    break;
            }
        }

        protected override void ExecuteInputEvent(TMP_InputField input)
        {
            switch (input.name)
            {
                case "UsernameInput":
                    CheckSaveButton();
                    break;
            }
        }

        void CheckSaveButton() => saveButton.interactable = !string.IsNullOrWhiteSpace(usernameInput.text);
    }
}