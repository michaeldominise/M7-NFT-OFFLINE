using M7.GameData;
using M7.GameRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7
{
    public class Settings_PlayerProfileSceneManager : SceneManagerBase
    {
        public static Settings_PlayerProfileSceneManager Instance { get; private set; }

        [SerializeField] AssetReference selectAvatarScene;
        [SerializeField] AssetReference emailScene;
        [SerializeField] AssetReference usernameScene;
        [SerializeField] AssetReference setPassScene;
        [SerializeField] AssetReference deleteAccountScene;
        [SerializeField] TextMeshProUGUI emailAdd;
        [SerializeField] TextMeshProUGUI username;
        [SerializeField] CharacterInstance_AvatarBasic avatar;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }


        private void Start()
        {
            PlayerDatabase.AccountProfile.onValuesChanged += RefreshAccountValues;
            RefreshAccountValues();
        }

        void RefreshAccountValues()
        {
            username.text = PlayerDatabase.AccountProfile.Username;
            emailAdd.text = PlayerDatabase.AccountProfile.Email;
            avatar.Init(MasterIDManager.RPGObjectReference.FindAssetReference(PlayerDatabase.AccountProfile.AvatarId)?.GetAssetReference<RPGObject>(), null);
        }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;
                case "AvatarButton":
                    LoadScene(selectAvatarScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
                case "EmailButton":
                    LoadScene(emailScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
                case "NameButton":
                    LoadScene(usernameScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
                case "SetPasswordButton":
                    LoadScene(setPassScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
                case "DeleteAccountButton":
                    LoadScene(deleteAccountScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
            }
        }
    }
}