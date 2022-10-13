using M7.CDN.Addressable;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sirenix.OdinInspector;
using System.Reflection;
using UnityEngine;

namespace M7.GameData
{
    [System.Serializable]
    public class BaseSaveableData<T> : BaseSaveableData where T : RPGObject
    {
        public BaseSaveableData() : base() { }
        public BaseSaveableData(string masterID, string instanceID) : base(masterID, instanceID) { }
    }

    [System.Serializable]
    public class BaseSaveableData : DirtyData
    {
        [JsonProperty] [SerializeField] public string masterID;
        [JsonProperty] [SerializeField] public string instanceID;
        
        [JsonIgnore] public string MasterID => masterID;
        [JsonIgnore] public string InstanceID => instanceID;

        [JsonIgnore] public AssetReferenceData<RPGObject> AssetReferenceData => MasterIDManager.RPGObjectReference.FindAssetReference(masterID);

        public void OverwriteValues(string json)
        {
            JsonConvert.PopulateObject(json, this, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
            IsDirty = true;
        }

        public BaseSaveableData() { }

        public BaseSaveableData(string masterID, string instanceID)
        {
            this.masterID = masterID;
            this.instanceID = instanceID;
        }
    }

    public class PrivateContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty result = base.CreateProperty(member, memberSerialization);

            var propInfo = member as PropertyInfo;
            result.Writable |= propInfo != null && propInfo.SetMethod != null;

            return result;
        }
    }
}