using System;
using M7.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class PreloadbattleTeamManager : BaseTeamManager<BaseCharacterInstance>
    {
        void Awake()
        {
            //Init(PlayerDatabase.Teams.CurrentPartySelected.AllSaveableCharacters, null);
        }

        public void Start()
        {
        }
    }
}
