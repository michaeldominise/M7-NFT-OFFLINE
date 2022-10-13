using System;
using System.Collections.Generic;
using M7.GameData;
using M7.GameRuntime.Scripts.UI;
using M7.GameRuntime.Scripts.UI.OverDrive;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Managers.Avatars
{
    public class DisplayAvatarManager : MonoBehaviour
    {
        [SerializeField] private List<CharacterSkillUI> displayAvatarItems;

        public void Init(TeamData_Player teamsCurrentPartySelected, Action onFinishTrigger)
        {
            // for (var index = 0; index < teamsCurrentPartySelected.AllSaveableCharacters.Count; index++)
            // {
            //       displayAvatarItems[index].Init(teamsCurrentPartySelected.AllSaveableCharacters[index]);
            // }

            onFinishTrigger?.Invoke();
        }
    }
}
