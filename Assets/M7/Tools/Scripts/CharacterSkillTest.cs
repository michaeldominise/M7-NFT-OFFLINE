#if UNITY_EDITOR

using System.Security.Principal;
using M7.GameRuntime;
using M7.Skill;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.Tools.Scripts
{
    public class CharacterSkillTest : MonoBehaviour
    {
        [SerializeField] CharacterInstance_Battle _characterInstanceBattle;

        [Button]
        public void Execute()
        {
            SkillManager.TestSkill(_characterInstanceBattle, _characterInstanceBattle.CharacterSkillAsset);
        }    
    }
}

#endif
