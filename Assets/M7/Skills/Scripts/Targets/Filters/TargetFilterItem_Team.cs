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
    public class TargetFilterItem_Team : TargetFilterItem<TeamManager_Battle>
    {
        [SerializeField] protected SkillEnums.TeamFilter teamFilter = SkillEnums.TeamFilter.Opponent;
        public override string DebugText => $"Get TeamManager_Battle that is TeamFilter: {teamFilter}{base.DebugText}.";

        public override bool IsValidTarget(Component caster, TeamManager_Battle target) => base.IsValidTarget(caster, target);

        public override List<TeamManager_Battle> GetTargets<CasterType>(CasterType caster, IEnumerable<TeamManager_Battle> initialTargets)
        {
            var list = initialTargets.Count() > 0 ? initialTargets : GetTeamList(caster, teamFilter);
            return list.ToList();
        }

        #region Team Filters
        protected List<TeamManager_Battle> GetTeamList(Component caster, SkillEnums.TeamFilter filter)
        {
            var list = new List<TeamManager_Battle>();
            if ((filter & SkillEnums.TeamFilter.Ally) != 0)
                list.Add(BattleManager.Instance.GetAllyTeam(caster));
            if ((filter & SkillEnums.TeamFilter.Opponent) != 0)
                list.Add(BattleManager.Instance.GetOpponentTeam(caster));
            return list;
        }
        #endregion
    }
}