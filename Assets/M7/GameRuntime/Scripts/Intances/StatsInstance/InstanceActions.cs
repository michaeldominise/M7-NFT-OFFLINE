using M7.GameData;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    [Serializable]
    public abstract class InstanceActions { }

    public class InstanceActions_CharacterStats : InstanceActions
    {
        public Action<float> onCurrentHpUpdate;
        // public Action<float> onOverdrivePercentageUpdate;
        public Action<float> onHpPercentageUpdate;
        public Action<float> onAttackUpdate;
        public Action<float> onDefenseUpdate;
        public Action<float> onMiningUpdate;
        public Action<float> onLuckUpdate;
        public Action<float> onDurabilityUpdate;
        public Action<float> onQueueUpdate;
        public Action<float> onMatchBoardDamageUpdate;
        public Action<float> onAttackTurnUpdate;
        public Action<float> onDamageReductionUpdate;
    }

    public class InstanceActions_TeamStats : InstanceActions
    {
        public Action<float> onSkillPointsUpdate;
    }

}
