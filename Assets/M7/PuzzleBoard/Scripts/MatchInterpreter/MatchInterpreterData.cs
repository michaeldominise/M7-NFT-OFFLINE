using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using M7.Match;
using Sirenix.OdinInspector;
using M7.Skill;
using M7.GameData;
using M7.GameRuntime;

[CreateAssetMenu(fileName = "MatchInterpreterData", menuName = "Assets/Data/MatchInterpreter/MatchInterpreterData")]
public class MatchInterpreterData : ScriptableObject
{
    [SerializeField] List<TileTypeMatchData> matchData;

    public TileTypeMatchData GetTileTypeMatchData(SkillEnums.ElementFilter elementType) => matchData.FirstOrDefault(x => x.element.ElementType == elementType);
    public CellType GetTileTypeOfElement(SkillEnums.ElementFilter elementType) => GetTileTypeMatchData(elementType)?.tileType;
    public RPGElement GetCharacterElement(SkillEnums.ElementFilter elementType) => GetTileTypeMatchData(elementType)?.element;

    [System.Serializable]
    public class TileTypeMatchData
    {
        public CellType tileType;
        public RPGElement element;
        public SkillEnums.ElementFilter effectiveElementType;
        public SkillEnums.ElementFilter weaknessElementType;
        public float effectiveMultiplier = 2f;
        public float weakMultiplier = 0.5f;

        public UIStatusValueManager.DamageType GetDamageType(SkillEnums.ElementFilter defenderElementType)
        {
            if (effectiveElementType == defenderElementType)
                return UIStatusValueManager.DamageType.Effective;
            else if (weaknessElementType == defenderElementType)
                return UIStatusValueManager.DamageType.Weak;
            else
                return UIStatusValueManager.DamageType.Normal;
        }

        public float GetDamageMultiplier(SkillEnums.ElementFilter defenderElementType)
        {
            if (effectiveElementType == defenderElementType)
                return effectiveMultiplier;
            else if (weaknessElementType == defenderElementType)
                return weakMultiplier;
            else
                return 1;
        }

        public float GetDamageMultiplier(UIStatusValueManager.DamageType damageType)
        {
            switch (damageType)
            {
                case UIStatusValueManager.DamageType.Effective:
                    return effectiveMultiplier;
                case UIStatusValueManager.DamageType.Weak:
                    return weakMultiplier;
            }
            return 1;
        }
    }
}
