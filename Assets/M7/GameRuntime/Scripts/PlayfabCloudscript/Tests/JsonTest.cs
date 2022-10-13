using System;
using System.Collections.Generic;
using M7.GameData;
using M7.GameData.Scripts.Saveables;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.Model;
using Sirenix.OdinInspector;
// using Unity.Plastic.Newtonsoft.Json;
using Newtonsoft.Json;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Energy.Tests
{
    public class JsonTest : MonoBehaviour
    {
        [SerializeField] private List<HeroData> heroList;

        [Button]
        private void HeroListToJson()
        {
            print(JsonConvert.SerializeObject(heroList));
        }


        [SerializeField] private List<TotalEnergyData> totalEnergyList;
        
        [Button]
        private void EnergyCapToJson()
        {
            print(JsonConvert.SerializeObject(totalEnergyList));
        }
        
        [SerializeField] private List<RarityBonusData> rarityBonusList;
        
        [Button]
        private void RarityToJson()
        {
            print(JsonConvert.SerializeObject(rarityBonusList));
        }

        [SerializeField] private SaveableEnergyData energyData;
        
        [Button]
        private void CurrentEnergyToJson()
        {
            energyData.timeEnergyUsed = DateTime.UtcNow;
            energyData.timeToNextEnergy = DateTime.UtcNow + TimeSpan.FromSeconds(120);
            print(Newtonsoft.Json.JsonConvert.SerializeObject(energyData));
        }

        [Button]
        private void PlayerType(PlayerType playerType)
        {
            print(Newtonsoft.Json.JsonConvert.SerializeObject(playerType.ToString()));
        }

        [Button]
        public void CreateUserKeyss()
        {
            PlayFabFunctions.PlayFabCallFunction("CreateUserKey", false, "", "", KeyCallback);
        }
        void KeyCallback(ExecuteResult result)
        {
            Debug.Log($" This is the result: {result}");
        }
    }
}
