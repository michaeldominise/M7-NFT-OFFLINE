using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_Revive : StatusEffect
    {
        public override void Execute(StatusEffectInstance statusEffectInstance, Action onFinish)
        {
            var charTarget = statusEffectInstance.Target as CharacterInstance_Battle;
            charTarget.Revive();

            base.Execute(statusEffectInstance, onFinish);
        }
    }
}