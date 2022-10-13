using M7.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace M7.GameData
{
    [System.Serializable]
    public class AncestorData
    {
        [System.Serializable]
        public class ParentData
        {
             [JsonProperty] public string masterID { get; set; }
             [JsonProperty] public string instanceID { get; set; }
        }

        [JsonProperty] [SerializeField] ParentData [] parentIDs;
        [JsonProperty] [SerializeField] ParentData [] grandParentIDs;

        [JsonIgnore] public ParentData [] ParentIDs => parentIDs;
        [JsonIgnore] public ParentData [] GrandParentIDs => grandParentIDs;
    }
}