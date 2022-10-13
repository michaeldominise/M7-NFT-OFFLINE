using System;
using M7.GameData.Scripts.RPGObjects.Boosters;
using Newtonsoft.Json;
using UnityEngine;

namespace M7.GameData
{
    [Serializable]
    public class SaveableBoosterData : BaseSaveableData<BoosterObject>
    {
        [JsonProperty] [SerializeField] float amount;
        [JsonIgnore] public float Amount { get => amount; private set => amount = value; }
        public void Decrement() => amount--;
        public SaveableBoosterData() : base() { }
        public SaveableBoosterData(string masterID, string instanceID, float amount) : base(masterID, instanceID) => this.amount = amount;
    }
}