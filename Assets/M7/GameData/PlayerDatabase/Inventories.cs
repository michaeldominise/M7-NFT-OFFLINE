using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using M7.GameData.Scripts.Saveables;
using Newtonsoft.Json;

namespace M7.GameData
{
    [System.Serializable]
    public class Inventories
    {
        [JsonProperty] [SerializeField] public InventoryData<SaveableCharacterData> characters = new InventoryData<SaveableCharacterData>();
        [JsonProperty] [SerializeField] public InventoryData<SaveableIncubatorData> incubators = new InventoryData<SaveableIncubatorData>();
        [JsonProperty] [SerializeField] public InventoryData<SaveableGemData> gems = new InventoryData<SaveableGemData>();
        [JsonProperty] [SerializeField] public InvetoryDataCurrencies currencies = new InvetoryDataCurrencies();
        [JsonProperty] [SerializeField] public InvetoryDataCurrencies systemCurrencies = new InvetoryDataCurrencies();
        [JsonProperty] [SerializeField] public InvetoryDataBoosters boosters = new InvetoryDataBoosters();
        [JsonProperty] [SerializeField] public SaveableEnergyData energy = new SaveableEnergyData();

        [JsonIgnore] public InventoryData<SaveableCharacterData> Characters => characters;
        [JsonIgnore] public InventoryData<SaveableIncubatorData> Incubators => incubators;
        [JsonIgnore] public InventoryData<SaveableGemData> Gems => gems;
        [JsonIgnore] public InvetoryDataCurrencies Currencies => currencies;
        [JsonIgnore] public InvetoryDataCurrencies SystemCurrencies => systemCurrencies;
        [JsonIgnore] public InvetoryDataBoosters Boosters => boosters;
        [JsonIgnore] public SaveableEnergyData Energy => energy;
    }

    [System.Serializable]
    public class InvetoryDataUpsert<T> : InventoryData<T> where T : BaseSaveableData, new()
    {
        public override T FindItem(string instanceID)
        {
            var item = base.FindItem(instanceID);
            if(item == null)
            {
                item = new T();

                var data = new Dictionary<string, object>();
                data["masterId"] = data["instanceID"] = instanceID;
                item.OverwriteValues(JsonConvert.SerializeObject(data));
                AddItem(item);
            }

            return item;
        }
    }

    [System.Serializable]
    public class InvetoryDataCurrencies : InvetoryDataUpsert<SaveableCurrencyData>
    {
        public float GetAmount(string instanceId, float defaultValue = 0) => FindItem(instanceId)?.Amount ?? defaultValue;
    }

    [System.Serializable]
    public class InvetoryDataBoosters : InvetoryDataUpsert<SaveableBoosterData>
    {
        public float GetAmount(string instanceId, float defaultValue = 0) => FindItem(instanceId)?.Amount ?? defaultValue;
    }
}
