using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using M7.GameData.Scripts.RPGObjects.Currency;
using M7.GameRuntime.Scripts.Energy;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using UnityEngine;

namespace M7.GameData
{
    [System.Serializable]
    public class SaveableCurrencyData : BaseSaveableData<CurrencyObject>
    {
        [JsonProperty] [SerializeField] float amount;
        [JsonProperty] [SerializeField] bool isVC;
        [JsonIgnore] public float Amount { get => amount; set => amount = value; }
        [JsonIgnore] public bool IsVC { get => isVC; set => isVC = value; }

        public SaveableCurrencyData() : base() { }
        public SaveableCurrencyData(string masterID, string instanceID, float amount) : base(masterID, instanceID)
        {
            this.amount = amount;
        }
    }
}