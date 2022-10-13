using M7.Match;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using M7.Match.PlaymakerActions;
using System.Collections.Generic;
using M7.GameRuntime;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_CellDestroy : StatusEffect
    {
        public enum IntervalMultiplierType { ByIndex, ByCasterToTargetDistance }
        [SerializeField] bool showParticleAttactor = false;
        [SerializeField] SkillEnums.CellDestroyType cellDestroyType = SkillEnums.CellDestroyType.None;
        [SerializeField] float attackMultiplier = 0;
        [SerializeField] IntervalMultiplierType intervalMultiplier;

        public bool ShowParticleAttactor => showParticleAttactor;
        public float AttackMultiplier => attackMultiplier;
        public SkillEnums.CellDestroyType CellDestroyType => cellDestroyType;

        [ReadOnly]
        public override float Value
        {
            get => attackMultiplier;
#if UNITY_EDITOR
            set => attackMultiplier = value;
#endif
        }

        public override float ComputeIntervalPerTarget(int index, Component caster, Component target, float value)
        {
            switch(intervalMultiplier)
            {
                case IntervalMultiplierType.ByCasterToTargetDistance:
                    var casterTile = caster as MatchGridCell;
                    var targetTile = target as MatchGridCell;
                    var test = Vector2.Distance(casterTile.CurrentRectPoint.ToVector2Int(), targetTile.CurrentRectPoint.ToVector2Int());
                    return test * value;
                default:
                    return base.ComputeIntervalPerTarget(index, caster, target, value);
            }
        }

        public override void Execute(StatusEffectInstance statusEffectInstance, Action onFinish)
        {
            var tileDestroy = statusEffectInstance.StatusEffect as StatusEffect_CellDestroy;
            var targetCell = statusEffectInstance.Target as MatchGridCell;
            if (!targetCell.TryDealDamage(new CellType_DamageCondition.DamageData(targetCell, new List<MatchGridCell> { targetCell }, tileDestroy.CellDestroyType), TileChainDamager.DEFAULT_DAMAGE))
            {
                base.Execute(statusEffectInstance, onFinish);
                return;
            }

            targetCell.ShowParticleAttactor = tileDestroy.ShowParticleAttactor;
            if (targetCell.CellHealth.IsDead)
                foreach (var charBattle in BattleManager.Instance.ActiveTeam.ActiveCharacters)
                {
                    if (charBattle == null || targetCell.CellTypeContainer.CellType.ElementType == SkillEnums.ElementFilter.None || (charBattle.Element.ElementType | targetCell.CellTypeContainer.CellType.ElementType) != targetCell.CellTypeContainer.CellType.ElementType)
                        continue;

                    float predictedDamage = charBattle.StatsInstance.Attack * tileDestroy.AttackMultiplier;
                    charBattle.StatsInstance.AddValue(SkillEnums.TargetCharacterStats.MatchBoardDamage, predictedDamage);
                }

            base.Execute(statusEffectInstance, onFinish);
        }
    }
}