using M7.Match;
using System;
using UnityEngine;

namespace M7.Skill
{
    public class StatusEffect_HideObject: StatusEffect
    {
        [SerializeField] float delay;

        public override void Execute(StatusEffectInstance statusEffectInstance, Action onFinish)
        {
            var targetTile = statusEffectInstance.Target as MatchGridCell;
            targetTile.SetSpriteRenderer(false, delay);
            base.Execute(statusEffectInstance, onFinish);
        }
    }
}