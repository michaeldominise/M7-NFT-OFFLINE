using System;
using System.Collections.Generic;
using M7.GameData;
using M7.GameData.Scripts.Saveables;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Energy.Tests
{
    public class JsonTestGai : MonoBehaviour
    {
        [SerializeField] private List<LevelData> levelList;

        [TextArea]
        public string json;

        [Button]
        private void TeamData()
        {
            json = JsonConvert.SerializeObject(PlayerDatabase.Teams.teamDataList);
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
        private void LevelJson()
        {
            json = JsonConvert.SerializeObject(levelList, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
            Debug.Log(json);
        }
    }
}
