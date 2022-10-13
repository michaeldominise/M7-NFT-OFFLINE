using M7.GameData;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace M7.GameData
{
    [System.Serializable]
    public class PlayerTeams : DirtyData
    {
        [JsonProperty] [SerializeField] public string selectedTeamName = "Team 1";

        [JsonProperty] [SerializeField] public List<TeamData_Player> teamDataList;
        [JsonIgnore] public TeamData_Player CurrentPartySelected => teamDataList[CurrentPartySelectedIndex];
        [JsonIgnore] public int CurrentPartySelectedIndex => teamDataList.Count == 0 ? 0 : Mathf.Max(teamDataList.FindIndex(x => x.TeamName == selectedTeamName), 0);

        public TeamData_Player GetPartyAtIndex(int index) => teamDataList[(int)Mathf.Repeat(index, teamDataList.Count)];
        //public void OverwriteValues(string json) => JsonConvert.DeserializeObject(json, this, new JsonSerializerSettings { ContractResolver = new PrivateContractResolver() });
        public bool IsHeroInAnyTeam(string instanceId) => teamDataList.FirstOrDefault(team => team.AllSaveableCharacters.FirstOrDefault(x => x != null && x.InstanceID == instanceId) != null) != null;

        public void OverwriteValues(string json)
        {
            PlayerTeams playerTeams = JsonConvert.DeserializeObject<PlayerTeams>(json);
            selectedTeamName = playerTeams.selectedTeamName;
            teamDataList = playerTeams.teamDataList;
        }
    }
}

public class TeamsResponse
{
    [JsonProperty] public Teams Teams;
}

public class Teams
{
    [JsonProperty] public string selectedTeamName;
    [JsonProperty] public TeamList[] TeamList;
}

public class TeamList
{
    [JsonProperty] public string TeamName;
    [JsonProperty] public int[] Waves;
}