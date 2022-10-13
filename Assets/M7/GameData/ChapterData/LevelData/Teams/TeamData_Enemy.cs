using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

namespace M7.GameData
{
    [System.Serializable]
    public class TeamData_Enemy : TeamData
    {
        [JsonProperty] [SerializeField] List<WaveData_Enemy> waves = new List<WaveData_Enemy>();
        [JsonIgnore] public override List<WaveData> Waves => waves.Select(x => x as WaveData).ToList();
    }
}