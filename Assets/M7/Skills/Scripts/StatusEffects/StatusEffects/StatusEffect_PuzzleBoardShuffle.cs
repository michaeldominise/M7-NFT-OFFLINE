using M7.Match;
using System;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_PuzzleBoardShuffle : StatusEffect
    {
        public override void Execute(StatusEffectInstance statusEffectInstance, Action onFinish)
        {
            PuzzleBoardManager.Instance.RequestShuffle();
            base.Execute(statusEffectInstance, onFinish);
        }
    }
}