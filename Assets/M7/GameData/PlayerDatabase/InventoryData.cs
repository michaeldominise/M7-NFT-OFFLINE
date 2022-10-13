using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using static M7.GameData.SortUtility;

namespace M7.GameData
{
    [System.Serializable]
    public class InventoryData<T> where T : BaseSaveableData, new()
    {
        [JsonProperty] [SerializeField] public List<T> items = new List<T>();

        public List<T> GetItems() => items;
        public virtual T FindItem(string instanceID) => FindItem(item => item.InstanceID == instanceID);
        public virtual T FindItem(System.Predicate<T> condition) => items.Find(condition);
        public virtual List<T> FindItems(string masterID) => FindItems(item => item.MasterID == masterID);
        public virtual List<T> FindItems(System.Predicate<T> condition) => items.FindAll(condition);
        public void RemoveItem(T item) => items.Remove(item);
        public int RemoveItems(System.Predicate<T> condition) => items.RemoveAll(condition);
        public void AddItem(T item) => items.Add(item);
        public void AddItems(params T[] items) => this.items.AddRange(items);

        public List<T> GetSortedItems(params SortData<T>[] sortData) => items.GetSortedItems(sortData);

        public void OverwriteValues(string json)
        {
            // JsonConvert.PopulateObject(json, items, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
            items = JsonConvert.DeserializeObject<List<T>>(json, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
        }
    }
}