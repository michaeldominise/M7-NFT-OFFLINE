using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameData.Scripts.Saveables
{
    [Serializable]
    public class SaveableEnergyData
    {
        public  int currentEnergy;
        public int energyCap;
        [ShowInInspector, DisplayAsString(false)]
        public  DateTime timeEnergyUsed;
        [ShowInInspector, DisplayAsString(false)]
        public  DateTime timeToNextEnergy;
        
        [ShowInInspector, DisplayAsString(false)]
        [JsonIgnore] public string time;
        [JsonIgnore] [ReadOnly] public bool isClockTicking;
        
        [Button]
        private void EnergyToJson()
        {
            Debug.Log(JsonConvert.SerializeObject(this));
        }
        
        public string TimeToNextEnergy()
        {
            var timeSpan = timeToNextEnergy - DateTime.UtcNow;
            return string.Format($" {timeSpan.Hours:0}h {timeSpan.Minutes:0}m");
        }

        public void OverwriteValues(string json)
        {
            var result = JsonConvert.DeserializeObject<SaveableEnergyData>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
            currentEnergy = result.currentEnergy;
            energyCap = result.energyCap;
            timeEnergyUsed = result.timeEnergyUsed;
            timeToNextEnergy = result.timeToNextEnergy;
        }
    }
}