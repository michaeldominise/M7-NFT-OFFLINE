using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using System.Linq;
using Sirenix.OdinInspector;

namespace M7.Skill
{
    [Serializable]
    public class TargetFilterItem_StatsInstanceCanAttack : TargetFilterItem<CharacterInstance_Battle>
    {
        [SerializeField] protected SkillEnums.TeamFilter teamFilter = SkillEnums.TeamFilter.Opponent;
        [SerializeField] protected SkillEnums.SelectionFilter selectionFilter = SkillEnums.SelectionFilter.All;
        [SerializeField] protected SkillEnums.HpStatusFilter hpStatusFilter = SkillEnums.HpStatusFilter.Alive;
        [SerializeField] protected SkillEnums.ElementFilter elementFilter = SkillEnums.ElementFilter.All;
        public override string DebugText => $"Get CharacterInstance_Battle that is TeamFilter: {teamFilter}, HpStatusFilter: {hpStatusFilter}, ElementFilter: {elementFilter}{base.DebugText}.";

        public override bool IsValidTarget(Component caster, CharacterInstance_Battle target) => base.IsValidTarget(caster, target) && FilterSelection(caster, target) && FilterHealthStatus(caster, target) && FilterElement(caster, target);

        public override List<CharacterInstance_Battle> GetTargets<CasterType>(CasterType caster, IEnumerable<CharacterInstance_Battle> initialTargets)
        {
            var list = initialTargets.Count() > 0 ? initialTargets : GetCharacterList(caster, teamFilter);
            return list.Where(x => IsValidTarget(caster, x)).ToList();
        }

        #region CharacterInstance_Battle Filters
        public static List<CharacterInstance_Battle> GetCharacterList(Component caster, SkillEnums.TeamFilter filter)
        {
            var list = new List<CharacterInstance_Battle>();
            if ((filter & SkillEnums.TeamFilter.Ally) != 0)
                list.AddRange(BattleManager.Instance.GetAllyTeam(caster).ActiveCharacters);
            if ((filter & SkillEnums.TeamFilter.Opponent) != 0)
                list.AddRange(BattleManager.Instance.GetOpponentTeam(caster).ActiveCharacters);
            return list;
        }

        protected bool FilterSelection(Component caster, CharacterInstance_Battle target) => (selectionFilter | ((target == caster) ? SkillEnums.SelectionFilter.Caster : SkillEnums.SelectionFilter.Others)) == selectionFilter;
        protected bool FilterHealthStatus(Component caster, CharacterInstance_Battle target) => (hpStatusFilter | (target.IsAlive ? SkillEnums.HpStatusFilter.Alive : SkillEnums.HpStatusFilter.Dead)) == hpStatusFilter;
        protected bool FilterElement(Component caster, CharacterInstance_Battle target) => (elementFilter | target.Element.ElementType) == elementFilter;
        #endregion
    }
}