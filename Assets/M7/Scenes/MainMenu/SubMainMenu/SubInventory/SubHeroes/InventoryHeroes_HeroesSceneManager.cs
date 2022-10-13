using M7.GameData;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7
{
    public class InventoryHeroes_HeroesSceneManager : SceneManagerBase
    {
        public static InventoryHeroes_HeroesSceneManager Instance { get; private set; }

        [SerializeField] PopulateInventoryManager<CharacterInstance_InventoryCard, SaveableCharacterData> populateInventoryManager;
	    [SerializeField] AssetReference hero_profile;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start() => populateInventoryManager.Populate(
	        PlayerDatabase.Inventories.Characters.GetSortedItems(
		        new SortUtility.SortData<SaveableCharacterData>(x => PlayerDatabase.Teams.IsHeroInAnyTeam(x.InstanceID), 
								SortUtility.SortData.SortType.Decending)), OnItemClick);
	    private void Update() => populateInventoryManager.Update();
	    public static string CurrentInstanceId;

	    private void OnItemClick (BaseSaveableData data)
    	{
    		var charData = data as SaveableCharacterData;
	    	LoadScene(hero_profile, UnityEngine.SceneManagement.LoadSceneMode.Additive,  overwriteSceneLayer: 16);
	        CurrentInstanceId = charData.InstanceID;
    	}

        [Button]
        public void LevelUpTesting()
        {
            populateInventoryManager.Populate(PlayerDatabase.Inventories.Characters.GetSortedItems(new SortUtility.SortData<SaveableCharacterData>(x => PlayerDatabase.Teams.IsHeroInAnyTeam(x.InstanceID), SortUtility.SortData.SortType.Decending)), OnItemClick);
        }
    }
}