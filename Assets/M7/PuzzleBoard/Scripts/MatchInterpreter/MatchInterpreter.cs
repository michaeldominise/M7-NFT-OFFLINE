using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayMaker;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using M7.Match;
using M7.Match.PlaymakerActions;
using M7.Skill;
using M7.GameData;
using M7.GameRuntime;
using DarkTonic.MasterAudio;

public class MatchInterpreter : MonoBehaviour {

    [System.Serializable]
    public class MatchData
    {
        [SerializeField] SkillEnums.ElementFilter _elementType;
        [SerializeField] int _tileCount;

        public SkillEnums.ElementFilter ElementType { get { return _elementType; } }
        public int tileCount { get { return _tileCount; } }

        public MatchData(SkillEnums.ElementFilter ce, int tc)
        {
            _elementType = ce;
            _tileCount = tc;
        }
    }

    public static MatchInterpreter Instance;

    [SerializeField] MatchGrid _matchGrid => PuzzleBoardManager.Instance.ActiveGrid;
    [SerializeField] MatchInterpreterData _data;
    [SerializeField] MatchDamageFormula _matchDamageFormula;

    public List<MatchData> totalMatchList;
    public List<float> debug_MultipliersThisGame;
    public float debug_AverageMultiplier;

    public float damageMultiplier;
    public float additionalDamageMultiplier = 1;

    public MatchDamageFormula MatchDamageFormula => _matchDamageFormula;
    public List<SkillEnums.ElementFilter> OmniAffectedElements => PuzzleBoardManager.Instance.OmniAffectedElements;

    private void Awake()
	{
        Instance = this;
	}

    public void Try_InterpretMatch(List<PointList<RectPoint>> chains)
    {
        foreach (PointList<RectPoint> match in chains)
        {

            SkillEnums.ElementFilter elementType = SkillEnums.ElementFilter.None;
            foreach(var rectPoint in match)
                elementType |= _matchGrid.Grid[rectPoint].CellTypeContainer.CellType.ElementType;
            MatchData matchData = new MatchData(elementType, match.Count);
            totalMatchList.Add(matchData);

            if (!BattleManager.Instance.IsGameDone)
                Match_OfType(matchData, match.Count);
        }
    }

    public void ComputeDamage(float additionalMultiplier)
    {
        var matchMultiplier = 0f;
        for (int i = 0; i < totalMatchList.Count; i++)
        {
            if (i == 0)
                matchMultiplier = _matchDamageFormula.GetDamage_FirstMatch(totalMatchList[i].tileCount);
            else
                matchMultiplier += _matchDamageFormula.GetDamage_SucceedingMatches(totalMatchList[i].tileCount);
        }
        matchMultiplier = Mathf.Max(1, matchMultiplier);
        damageMultiplier = matchMultiplier * additionalMultiplier * additionalDamageMultiplier;
    }

    public float ComputeDamage(float damage, int cellCount) => _matchDamageFormula.ComputeDamage(damage, cellCount);

    public void ClearDamage()
    {
        totalMatchList.Clear();

        foreach (var charBattle in BattleManager.Instance.ActiveTeam.ActiveCharacters)
        {
            if (charBattle == null)
                return;

            charBattle.StatsInstance.SetValue(SkillEnums.TargetCharacterStats.MatchBoardDamage, 0);
            charBattle.StatsInstance.InstanceActions.onMatchBoardDamageUpdate?.Invoke(0);
        }
    }

    public void RegisterDebugMultiplier()
    {
        debug_MultipliersThisGame.Add(damageMultiplier);


        debug_AverageMultiplier = 0;

        foreach (float val in debug_MultipliersThisGame)
            debug_AverageMultiplier += val;

        debug_AverageMultiplier /= debug_MultipliersThisGame.Count;
    }

    public CellType Get_TileType_OfElement(SkillEnums.ElementFilter elementType)
    {
        return _data.GetTileTypeOfElement(elementType);
    }

    public RPGElement Get_CharacterElement(SkillEnums.ElementFilter elementType)
    {
        return _data.GetCharacterElement(elementType);
    }

    public void Match_OfType(MatchData matchData, int totalMatches)
    {
        //var omniMultiplierBonus = PuzzleBoardManager.Instance.OmniAffectedElements.Contains(matchData.ElementType) ? PuzzleBoardSettings.Instance.omniSphereAttackMultiplier : 1;
        //var additionalMultiplier = omniMultiplierBonus * PuzzleBoardSettings.Instance.GetComboAttackMultiplier(PuzzleBoardManager.Instance.LastComboCount);

        foreach (var charBattle in BattleManager.Instance.ActiveTeam.AliveCharacters)
        {
            if (charBattle == null || !charBattle.StatsInstanceBattle.CanAttack() || matchData.ElementType == SkillEnums.ElementFilter.None || (charBattle.Element.ElementType | matchData.ElementType) != matchData.ElementType)
                continue;

            float predictedDamage = ComputeDamage(charBattle.StatsInstance.Attack, totalMatches) - charBattle.StatsInstance.Attack;
            charBattle.StatsInstance.SetValue(SkillEnums.TargetCharacterStats.MatchBoardDamage, predictedDamage);
        }
    }

    public MatchInterpreterData.TileTypeMatchData GetTileTypeMatchData(SkillEnums.ElementFilter attackerElementType) => _data.GetTileTypeMatchData(attackerElementType);
}
