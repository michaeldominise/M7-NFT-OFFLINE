using System;
using M7.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class TeamManager_Default : BaseTeamManager<BaseCharacterInstance>
    {
        [SerializeField] bool autoLoadCurrentSelectedParty;

        void Awake()
        {
            if(autoLoadCurrentSelectedParty)
                Init(PlayerDatabase.Teams.CurrentPartySelected.Waves[0], null);
        }
    }
}
