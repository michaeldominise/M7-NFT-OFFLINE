using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MatchDamageFormula", menuName = "Assets/Data/MatchDamageFormula")]
public class MatchDamageFormula : ScriptableObject {

    const int MIN_MATCH_COUNT = 0;

    [System.Serializable]
    public class MatchValueGroup
    {
        public float baseValue;
        public float bonusValue;
    }

    public MatchValueGroup firstMatchValue;
    public MatchValueGroup suceedingMatchValue;

    public float GetDamage_FirstMatch(int firstMatchCount)
    {
        float toReturn = firstMatchValue.baseValue;

        if(firstMatchCount > MIN_MATCH_COUNT) 
            toReturn += ((firstMatchCount - MIN_MATCH_COUNT) * firstMatchValue.bonusValue);

        return toReturn;
    }

    public float ComputeDamage(float damage, int cellCount) => GetBaseDamage(damage) + GetBonusDamage(damage, cellCount);
    public float GetBaseDamage(float damage) => damage * firstMatchValue.baseValue;
    public float GetBonusDamage(float damage, int cellCount) => damage * (cellCount - MIN_MATCH_COUNT) * firstMatchValue.bonusValue;

    public float GetDamage_SucceedingMatches(int otherMatchCount)
    {
        float toReturn = suceedingMatchValue.baseValue;

        if(otherMatchCount > MIN_MATCH_COUNT) 
            toReturn += ((otherMatchCount - MIN_MATCH_COUNT) * suceedingMatchValue.bonusValue);
        
        return toReturn;
    }
}
