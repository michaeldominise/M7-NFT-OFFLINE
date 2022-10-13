using M7.GameData;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7
{
    public partial class Settings_PlayerProfile_SelectAvatarSceneManager : SceneManagerBase
    {
        public static Settings_PlayerProfile_SelectAvatarSceneManager Instance { get; private set; }

        [SerializeField] PopulateInventoryManager<CharacterInstance_AvatarBasic, AssetReferenceT<RPGObject>> populateInventoryManager;
        [SerializeField] List<AssetReferenceT<RPGObject>> heroList;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start() => populateInventoryManager.Populate(heroList, OnItemClick);
        private void Update() => populateInventoryManager.Update();

        [ShowInInspector, ReadOnly] public string SelectedAvatarId { get; private set; }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;
                case "ApplyButton":
                    UpdateAvatar();
                    break;
            }
        }

        private void OnItemClick(AssetReferenceT<RPGObject> data)
        {
            var charObject = data.Asset as CharacterObject;
            SelectedAvatarId = charObject.MasterID;
        }
    }
}