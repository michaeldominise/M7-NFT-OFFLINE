using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

namespace M7.GameData
{
    [System.Serializable]
    public abstract class TeamData
    {
        [JsonProperty] [SerializeField] public string teamName = "Default";

        [JsonIgnore] public string TeamName => teamName;
        [JsonIgnore] public abstract List<WaveData> Waves { get; }
        [JsonIgnore]
        public List<SaveableCharacterData> AllSaveableCharacters 
        {
            get
            {
                var saveableCharacters = new List<SaveableCharacterData>();
                Waves.ForEach(x => saveableCharacters.AddRange(x.SaveableCharacters));
                return saveableCharacters;
            }
        }
    }
}