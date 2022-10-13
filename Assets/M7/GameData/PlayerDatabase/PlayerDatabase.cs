using M7.GameRuntime;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "PlayerDatabase", menuName = "Assets/M7/GameData/PlayerDatabase")]
    public class PlayerDatabase : ScriptableObject
    {
        private static PlayerDatabase Instance  => GameManager.Instance.PlayerDatabase;
        public const string PlayerDatabaseAccountKey = "PlayerDatabaseAccountProfile";
        public const string PlayerDatabaseInventoriesCharactersKey = "PlayerDatabaseInventoriesCharacters";
        public const string PlayerDatabaseInventoriesIncubatorsKey = "PlayerDatabaseInventoriesIncubators";
        public const string PlayerDatabaseInventoriesGemsKey = "PlayerDatabaseInventoriesGems";
        public const string PlayerDatabaseInventoriesCurrenciesKey = "PlayerDatabaseInventoriesCurrencies";
        public const string PlayerDatabaseInventoriesSystemCurrenciesKey = "PlayerDatabaseInventoriesSystemCurrencies";
        public const string PlayerDatabaseInventoriesBoostersKey = "PlayerDatabaseInventoriesBoosters";
        public const string PlayerDatabaseInventoriesEnergyKey = "PlayerDatabaseInventoriesEnergy";
        public const string PlayerDatabasePlayerTeamsKey = "PlayerDatabasePlayerTeams";
        public const string PlayerDatabaseCampaignDataKey = "PlayerDatabaseCampaignData"; 


        [SerializeField] public AccountProfile accountProfile;
        [SerializeField] public Inventories inventories;
        [SerializeField] public PlayerTeams teams;
        [SerializeField] public CampaignData campaignData;
        [JsonIgnore] public bool dontLoadServerData;

        public static AccountProfile AccountProfile => Instance.accountProfile;
        public static Inventories Inventories => Instance.inventories;
        public static PlayerTeams Teams => Instance.teams;
        public static CampaignData CampaignData => Instance.campaignData;
        [JsonIgnore] public static GlobalDataSetting GlobalDataSetting => GlobalDatabase.GlobalDataSetting;
        [JsonIgnore] public static DurabilityCostSetting DurabilityCostSetting => GlobalDatabase.DurabilityCostSetting;
        public static bool DontLoadServerData => Instance.dontLoadServerData;
        

        [TextArea]
        [JsonIgnore] public string json;

        [Button]
        private void chars()
        {
            json = JsonConvert.SerializeObject(this);//, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() }
            Debug.Log(json);
        }

        [Button]
        private void CharacterJson()
        {
            json = JsonConvert.SerializeObject(inventories.Characters.GetSortedItems());//, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() }
            Debug.Log(json);
        }

        [Button]
        private void IncubatorJson()
        {
            json = JsonConvert.SerializeObject(inventories.Incubators.GetSortedItems());//, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() }
            Debug.Log(json);
        }
        
        [Button]
        private void GemsJson()
        {
            json = JsonConvert.SerializeObject(inventories.Gems.GetSortedItems());//, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() }
            Debug.Log(json);
        }

        [Button]
        private void StageBonus()
        {
            json = JsonConvert.SerializeObject(campaignData);//, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() }
            Debug.Log(json);
        }

        [Button]
        private void TeamDataList()
        {
            json = JsonConvert.SerializeObject(teams.teamDataList);//, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() }
            Debug.Log(json);
        }

        [Button]
        private void LevelChart()
        {
            //json = JsonConvert.SerializeObject(GlobalDatabase.globalSettings.PlayerAllLevelChartData);//, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() }
            Debug.Log(json);
        }

        [Button]
        private void GetCharacterFromPlayFab()
        {
            PlayFabFunctions.PlayFabCallFunction("GetUserData", false, "HeroCollection", "", SetCharacterInventory);
        }

        //[Button]
        //private void GetPlayerdataFromPlayFab()
        //{
        //    PlayFabFunctions.PlayFabCallFunction("GetAllUserData", "", "", PlayerDataCallback);
        //}

        //private void PlayerDataCallback(ExecuteResult result)
        //{
        //    var playerdata = JsonConvert.DeserializeObject<PlayerData>(result.Result.FunctionResult.ToString());

        //    Debug.Log(result.Result.FunctionResult);

        //    if (result.Status != ResultStatus.Ok) return;

        //    if (playerdata == null) return;


        //    inventories.Characters.OverwriteValues(playerdata.characters);
        //    inventories.Incubators.OverwriteValues(playerdata.incubators);
        //    inventories.Gems.OverwriteValues(playerdata.gems);
        //    inventories.Currencies.OverwriteValues(playerdata.currencies);
        //    inventories.Boosters.OverwriteValues(playerdata.boosters);
        //    teams.OverwriteValues(playerdata.teams);
        //    campaignData.OverwriteValues(playerdata.campaignData);
        //}

        private void SetCharacterInventory(ExecuteResult executeResult)
        {
            Debug.Log($"SetCharacterInventory, {executeResult.Result.FunctionResult}");
            inventories.Characters.OverwriteValues(executeResult.Result.FunctionResult.ToString());
            json = executeResult.Result.FunctionResult.ToString();
        }

        public static void ResetLocalData()
        {
            if (Instance.dontLoadServerData)
                return;
            
            Instance.accountProfile = new AccountProfile();
            Instance.inventories = new Inventories();
            Instance.teams = new PlayerTeams
            {
                teamDataList = new List<TeamData_Player>
                {
                    new TeamData_Player { teamName = "Team 1" },
                    new TeamData_Player { teamName = "Team 2" },
                    new TeamData_Player { teamName = "Team 3" }
                }
            };
            Instance.campaignData = new CampaignData();
            //Instance.globalSettings = new GlobalDataSetting();
        }

        public static void LoadLocalData()
        {
            if (!PlayerPrefs.HasKey(PlayerDatabaseAccountKey))
                return;

            var jsonAccount = PlayerPrefs.GetString(PlayerDatabaseAccountKey);
            var jsonInventoriesCharacters = PlayerPrefs.GetString(PlayerDatabaseInventoriesCharactersKey);
            var jsonInventoriesIncubators = PlayerPrefs.GetString(PlayerDatabaseInventoriesIncubatorsKey);
            var jsonInventoriesCurrencies = PlayerPrefs.GetString(PlayerDatabaseInventoriesCurrenciesKey);
            var jsonInventoriesSystemCurrencies = PlayerPrefs.GetString(PlayerDatabaseInventoriesSystemCurrenciesKey);
            var jsonInventoriesGems = PlayerPrefs.GetString(PlayerDatabaseInventoriesGemsKey);
            var jsonInventoriesBoosters = PlayerPrefs.GetString(PlayerDatabaseInventoriesBoostersKey);
            var jsonInventoriesEnergy = PlayerPrefs.GetString(PlayerDatabaseInventoriesEnergyKey);
            var jsonCampaignData = PlayerPrefs.GetString(PlayerDatabaseCampaignDataKey);
            var jsonPlayerTeams = PlayerPrefs.GetString(PlayerDatabasePlayerTeamsKey);

            Instance.accountProfile.OverwriteValues(jsonAccount);
            Instance.inventories.characters.OverwriteValues(jsonInventoriesCharacters);
            Instance.inventories.incubators.OverwriteValues(jsonInventoriesIncubators);
            Instance.inventories.currencies.OverwriteValues(jsonInventoriesCurrencies);
            Instance.inventories.systemCurrencies.OverwriteValues(jsonInventoriesSystemCurrencies);
            Instance.inventories.gems.OverwriteValues(jsonInventoriesGems);
            Instance.inventories.boosters.OverwriteValues(jsonInventoriesBoosters);
            Instance.inventories.energy.OverwriteValues(jsonInventoriesEnergy);
            Instance.campaignData.OverwriteValues(jsonCampaignData);
            Instance.teams.OverwriteValues(jsonPlayerTeams);
        }

        public static void SaveToLocal()
        {
            var jsonAccount = JsonConvert.SerializeObject(Instance.accountProfile);
            var jsonInventoriesCharacters = JsonConvert.SerializeObject(Instance.inventories.characters.items);
            var jsonInventoriesIncubators = JsonConvert.SerializeObject(Instance.inventories.incubators.items);
            var jsonInventoriesCurrencies = JsonConvert.SerializeObject(Instance.inventories.currencies.items);
            var jsonInventoriesSystemCurrencies = JsonConvert.SerializeObject(Instance.inventories.systemCurrencies.items);
            var jsonInventoriesGems = JsonConvert.SerializeObject(Instance.inventories.gems.items);
            var jsonInventoriesBoosters = JsonConvert.SerializeObject(Instance.inventories.boosters.items);
            var jsonInventoriesEnergy = JsonConvert.SerializeObject(Instance.inventories.energy);
            var jsonCampaignData = JsonConvert.SerializeObject(Instance.campaignData);
            var jsonPlayerTeams = JsonConvert.SerializeObject(Instance.teams);

            PlayerPrefs.SetString(PlayerDatabaseAccountKey, jsonAccount);
            PlayerPrefs.SetString(PlayerDatabaseInventoriesCharactersKey, jsonInventoriesCharacters);
            PlayerPrefs.SetString(PlayerDatabaseInventoriesIncubatorsKey, jsonInventoriesIncubators);
            PlayerPrefs.SetString(PlayerDatabaseInventoriesCurrenciesKey, jsonInventoriesCurrencies);
            PlayerPrefs.SetString(PlayerDatabaseInventoriesSystemCurrenciesKey, jsonInventoriesSystemCurrencies);
            PlayerPrefs.SetString(PlayerDatabaseInventoriesGemsKey, jsonInventoriesGems);
            PlayerPrefs.SetString(PlayerDatabaseInventoriesBoostersKey, jsonInventoriesBoosters);
            PlayerPrefs.SetString(PlayerDatabaseInventoriesEnergyKey, jsonInventoriesEnergy);
            PlayerPrefs.SetString(PlayerDatabaseCampaignDataKey, jsonCampaignData);
            PlayerPrefs.SetString(PlayerDatabasePlayerTeamsKey, jsonPlayerTeams);
        }
        public static bool CanPlayerBattle()
        {
            if (string.IsNullOrWhiteSpace(AccountProfile.WalletAddress) && false)
            {
                MessageBox.Create("Unable to play. Connect your wallet.", MessageBox.ButtonType.Ok).Show();
                return false;
            }
            else if (!Teams.CurrentPartySelected.IsPlayable())
            {
                MessageBox.Create("Unable to play. Selected team dosen't have any hero.", MessageBox.ButtonType.Ok).Show();
                return false;
            }
          
            else if (!Teams.CurrentPartySelected.IsLeveling())
            {
                MessageBox.Create("Unable to play. Other hero is leveling.", MessageBox.ButtonType.Ok).Show();
                return false;
            }

            return true;
        }
    }
}
