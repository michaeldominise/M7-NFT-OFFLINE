using M7.CDN.Addressable;
using M7.GameData;
using M7.GameRuntime;
using M7.ServerTestScripts;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace M7
{
    public class TeamEditSceneManager : SceneManagerBase
    {
        public static TeamEditSceneManager Instance { get; private set; }
        public enum SortType { Recent, Element, Rarity, Level }

        [SerializeField] PopulateInventoryManager<CharacterInstance_InventoryCard, SaveableCharacterData> populateInventoryManager;
        [SerializeField] MenuTeamSelector_EditTeam menuTeamSelector_EditTeam;
        [SerializeField] private AssetReference heroProfile;
        
        List<AssetReferenceT<CharacterObject>> characterObjectReferenceList = new List<AssetReferenceT<CharacterObject>>();
        List<CharacterObject> characterObjectList = new List<CharacterObject>();


        private void Start()
        {
            Instance = this;
            var characters = PlayerDatabase.Inventories.Characters.GetSortedItems();
            var loadedCount = 0;
            foreach(var character in characters)
            {
                var assetReference = MasterIDManager.RPGObjectReference.FindAssetReference(character.MasterID).GetAssetReference<CharacterObject>();
                if (characterObjectReferenceList.Contains(assetReference))
                    continue;
                characterObjectReferenceList.Add(assetReference);
                assetReference.LoadAssetAsync(result =>
                {
                    loadedCount++;
                    characterObjectList.Add(result);
                    if (loadedCount >= characterObjectReferenceList.Count)
                        populateInventoryManager.Populate(PlayerDatabase.Inventories.Characters.GetSortedItems(new SortUtility.SortData<SaveableCharacterData>(x => PlayerDatabase.Teams.IsHeroInAnyTeam(x.InstanceID), SortUtility.SortData.SortType.Decending)), OnCharacterInstanceClicked);
                });
            }
        }

        public void OnCharacterInstanceClicked(BaseSaveableData saveableData)
        {
            var teamManager = menuTeamSelector_EditTeam.CurrentTeamSelected;
            if (teamManager.SelectedIndex < 0)
            {
                var charData = saveableData as SaveableCharacterData;
                LoadScene(heroProfile, UnityEngine.SceneManagement.LoadSceneMode.Additive, overwriteSceneLayer: 16);
                InventoryHeroes_HeroesSceneManager.CurrentInstanceId = charData.InstanceID;
                return;
            }

            for (var x = 0; x < teamManager.RawCharacters.Length; x++)
            {
                if(x == teamManager.SelectedIndex)
                    teamManager.WaveData.SetSaveableCharacterAtIndex(x, saveableData as SaveableCharacterData);
                else if (teamManager.RawCharacters[x] != null && teamManager.RawCharacters[x].SaveableCharacterData.InstanceID == saveableData.InstanceID)
                    teamManager.WaveData.SetSaveableCharacterAtIndex(x, null);
            }
        
            teamManager.Init(teamManager.WaveData, null);
            teamManager.ResetState();
            Refresh();
        }

        private void Update() => populateInventoryManager.Update();

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.SetCharacterTeam);
                    Exit();
                    break;
                default:
                    base.ExecuteButtonEvent(gameObject);
                    break;
            }
        }

        public void Exit()
        {
            foreach (var characterObjectReference in characterObjectReferenceList)
                characterObjectReference.ReleaseAsset();
            UnloadAtSceneLayer(sceneLayer);
        }

        public void Refresh ()
        {
            var characters = PlayerDatabase.Inventories.Characters.GetItems();
            foreach (var e in characters)
                e.IsDirty = true;
        }

        public void OnDropdownValueChanged(int value)
        {
            switch ((SortType)value)
            {
                case SortType.Recent:
                    populateInventoryManager.ObjectDataList = PlayerDatabase.Inventories.Characters.GetSortedItems(new SortUtility.SortData<SaveableCharacterData>(x => PlayerDatabase.Teams.IsHeroInAnyTeam(x.InstanceID), SortUtility.SortData.SortType.Decending));
                    populateInventoryManager.Sort();
                    break;
                case SortType.Element:
                    populateInventoryManager.Sort(new SortUtility.SortData<SaveableCharacterData>(x => PlayerDatabase.Teams.IsHeroInAnyTeam(x.InstanceID), SortUtility.SortData.SortType.Decending),
                        new SortUtility.SortData<SaveableCharacterData>(x => characterObjectList.FirstOrDefault(y => y.MasterID == x.MasterID).Element.ElementType),
                        new SortUtility.SortData<SaveableCharacterData>(x => x.MasterID));
                    break;
                case SortType.Rarity:
                    populateInventoryManager.Sort(new SortUtility.SortData<SaveableCharacterData>(x => PlayerDatabase.Teams.IsHeroInAnyTeam(x.InstanceID), SortUtility.SortData.SortType.Decending),
                        new SortUtility.SortData<SaveableCharacterData>(x => characterObjectList.FirstOrDefault(y => y.MasterID == x.MasterID).DisplayStats.Rarity, SortUtility.SortData.SortType.Decending),
                        new SortUtility.SortData<SaveableCharacterData>(x => x.MasterID));
                    break;
                case SortType.Level:
                    populateInventoryManager.Sort(new SortUtility.SortData<SaveableCharacterData>(x => PlayerDatabase.Teams.IsHeroInAnyTeam(x.InstanceID), SortUtility.SortData.SortType.Decending),
                        new SortUtility.SortData<SaveableCharacterData>(x => x.Level, SortUtility.SortData.SortType.Decending),
                        new SortUtility.SortData<SaveableCharacterData>(x => x.MasterID));
                    break;
            }
        }
    }
}