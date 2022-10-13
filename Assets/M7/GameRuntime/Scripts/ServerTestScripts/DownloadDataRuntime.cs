using M7.GameData;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using UnityEngine.SceneManagement;
using System.Linq;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;

namespace M7.ServerTestScripts
{
    public class DownloadDataRuntime : MonoBehaviour
    {
        public static DownloadDataRuntime Instance => GameManager.Instance.DownloadDataRuntime;

        [SerializeField] GetPlayerBattleData getPlayerBattleData;
        [SerializeField] GetAllPlayerData getAllPlayerData;
        [SerializeField] SetGaianite setGaianite;
        [SerializeField] SetCharacterTeam setCharacterTeam;
        [SerializeField] HeroInventoryData heroInventoryData;
        [SerializeField] SetMoveData setMoveData;
        [SerializeField] SetEnergyData setEnergyData;
        [SerializeField] SetCustomStage setCustomStageData;

        [SerializeField] SetStatsData setStatsData;
        [SerializeField] SetRecoveryData setRecoveryData;
        [SerializeField] IAPPackData iAPPackData;
        [SerializeField] IAPCharacterData iAPCharacterData;

        public GetPlayerBattleData GetPlayerBattleData => getPlayerBattleData;
        public GetAllPlayerData GetAllPlayerData => getAllPlayerData;
        public SetGaianite SetGaianite => setGaianite;
        public SetCharacterTeam SetCharacterTeam => setCharacterTeam;
        public HeroInventoryData HeroInventoryData => heroInventoryData;
        public SetMoveData SetMoveData => setMoveData;
        public SetEnergyData SetEnergyData => setEnergyData;
        public SetStatsData SetStatsData => setStatsData;
        public SetCustomStage SetCustomStage => setCustomStageData;
        public SetRecoveryData SetRecoveryData => setRecoveryData;
        public IAPPackData IAPPackData => iAPPackData;
        public IAPCharacterData IAPCharacterData => iAPCharacterData;

        [SerializeField] GameObject loadingPanel;

        public bool isLoadScene = false;

        public enum ServerStatus { GetAllPlayerData, GetStageData, SetCollection, SetCharacterTeam, ReadyLevel, ConfirmLevel, BoostLevel, LevelTimeFinish, RetryMove, Energy_StartGame, Energy_EndGame, StatsData, UpdateCustomStage, SetRecovery, PurchasePack, PurchaseChar}
        public void Init(ServerStatus serverStatus, string jsonToPass = null)
        {
            switch (serverStatus)
            {
                case ServerStatus.GetAllPlayerData:
                    GetAllPlayerData.GetAllMainData();
                    break;
                case ServerStatus.GetStageData:
                    GetPlayerBattleData.GetBattleGameData();
                    break;
                case ServerStatus.SetCollection:
                    SetGaianite.SetCollectedCubes(BattleManager.Instance.GaianiteCollectionManager.cubeCountTestOnly);
                    break;
                case ServerStatus.SetCharacterTeam:
                    SetCharacterTeam.SetCharacter();
                    break;
                case ServerStatus.ReadyLevel:
                    HeroInventoryData.HeroInventoryEvent(HeroInventoryData.HeroInventoryState.OnReadyLevel);
                    break;
                case ServerStatus.ConfirmLevel:
                    HeroInventoryData.HeroInventoryEvent(HeroInventoryData.HeroInventoryState.OnConfirmLevel);
                    break;
                case ServerStatus.BoostLevel:
                    HeroInventoryData.HeroInventoryEvent(HeroInventoryData.HeroInventoryState.OnBoostLevel);
                    break;
                case ServerStatus.LevelTimeFinish:
                    HeroInventoryData.HeroInventoryEvent(HeroInventoryData.HeroInventoryState.OnFinishTime);
                    break;
                case ServerStatus.Energy_StartGame:
                    SetEnergyData.PlayCheckEnergy(SetEnergyData.EnergyStateEvent.PlayGame);
                    break;
                case ServerStatus.Energy_EndGame:
                    SetEnergyData.PlayCheckEnergy(SetEnergyData.EnergyStateEvent.TryAgainGame);
                    break;
                case ServerStatus.RetryMove:
                    SetMoveData.MoveCheckData(jsonToPass);
                    break;
                case ServerStatus.StatsData:
                    SetStatsData.CheckStats(jsonToPass);
                    break;
                case ServerStatus.UpdateCustomStage:
                    SetCustomStage.SetChosenStage(jsonToPass);
                    break;
                case ServerStatus.SetRecovery:
                    SetRecoveryData.CheckRecovery(jsonToPass);
                    break;
                case ServerStatus.PurchasePack:
                    IAPPackData.BuyPack(jsonToPass);
                    break;
                case ServerStatus.PurchaseChar:
                    IAPCharacterData.BuyCharacter();
                    break;
            }
        }

        public void GetNfts()
        {
            // call for NFT
            var email = new Dictionary<string, string>
        {
            { "email", PlayerDatabase.AccountProfile.Email }
        };

            AzureFunction.GetNfts(email, OkResult, ErrorResult);
        }

        private void ErrorResult(string errorResult)
        {
            Debug.Log($"Get NFT error {errorResult}");
        }

        private void OkResult(string result)
        {
            Debug.Log($"Get NFT ok {result}");
            var nft = JsonConvert.DeserializeObject<Nfts>(result);

            if (nft == null)
                return;

            PlayerDatabase.Inventories.Characters.OverwriteValues(nft.characters);
            PlayerDatabase.Inventories.Incubators.OverwriteValues(nft.incubators);
            PlayerDatabase.Inventories.Gems.OverwriteValues(nft.gems);
            PlayerDatabase.Inventories.Currencies.OverwriteValues(nft.currencies);

        }
    }
}