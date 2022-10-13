using System;
using System.Collections.Generic;
using System.Linq;
using M7.Match;
using M7.Match.PlaymakerActions;
using Chamoji.Social;
using DarkTonic.MasterAudio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using M7.Tools;

[CreateAssetMenu(fileName = "PuzzleBoardSettings", menuName = "Assets/Data/PuzzleBoardSettings")]
public class PuzzleBoardSettings : M7Settings
{
    [Serializable]
    public class GroupColorChanceRate
    {
        public int cellCount = 1;
        public int weightRate = 1;
    }

    public static PuzzleBoardSettings Instance => PuzzleBoardManager.Instance.PuzzleBoardSettings;
    public enum ComboTriggerType { PerConnectedTiles, PerHorizontalAndVertical }

    [Header("Board")]
    public Vector2 cellDimensions = Vector2.one;
    public Vector2 cellDimensionEnemy = new Vector2(0.6f, 0.6f);

    public Vector3 enemyGridDefaultScale = new Vector3(0.6f, 0.6f, 0.6f);
    public Vector3 enemyGridSmallScale = new Vector3(0.5f, 0.5f, 0.5f);

    public float cellTouchRadius = 0.25f;
    public GroupColorChanceRate[] groupColorChanceRates;

    [Header("OmniSphere")]
    public int maxOmniSphereGeneration = 3;
    public int omniSphereComboGenerationTrigger = 8;
    public int omniSphereAttackMultiplier = 2;

    [Header("ManaCore")]
    public int maxManaValue = 15;
    public int manaStartGeneration = 5;
    public int generateToMaxManaTrigger = 10;

    [Header("ComboMultiplier")]
    public int maxComboCount = 10;
    public int startComboCount = 5;
    public float comboInterval = 0.3f;
    public ComboTriggerType comboTriggerType;
    //public OmniSphereData[] omniSphereDataList;

    [SerializeField] float[] comboAttackMultiplier;



    public float GetComboAttackMultiplier(int comboCount)
    {
        comboCount = Mathf.Min(comboCount, maxComboCount);
        if ((comboAttackMultiplier?.Length ?? 0) == 0)
            return 1;

        return comboAttackMultiplier[Mathf.Min(comboCount, comboAttackMultiplier.Length - 1)];
    }
}
