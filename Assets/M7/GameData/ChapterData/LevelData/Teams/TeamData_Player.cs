using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace M7.GameData
{
    [System.Serializable]
    public class TeamData_Player : TeamData
    {
        [JsonProperty] [SerializeField] public List<WaveData_Player> waves = new List<WaveData_Player> { new WaveData_Player() };
        [JsonIgnore] public override List<WaveData> Waves => waves.Select(x => x as WaveData).ToList();

        public bool IsPlayable() => AllSaveableCharacters.FirstOrDefault(x => x != null) != null;
        public bool IsLeveling () {
            List<SaveableCharacterData> lvlingList = AllSaveableCharacters.FindAll(s => s != null);
            if(lvlingList.FindAll(x => x.LevelUpTimeComplete != DateTime.MinValue).Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }   
        }
    }
}