using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu( fileName = "", menuName = "Assets/Data/Automatch/Auto match Setting")]
public class AutoMatchSetting : ScriptableObject {

    public enum AutoMatchSettingType
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Level5
    }
    public AutoMatchSettingType settingType;

    [TitleGroup("Match Speed", HorizontalLine = false), LabelText("Min")]
    public float matchSpeedMin = 0.2f;
    [TitleGroup("Match Speed", HorizontalLine = false), LabelText("Max")]
    public float matchSpeedMax = 0.5f;

    [TitleGroup("Match Logic ", HorizontalLine = false), LabelText("Chance For Correct Match")]
    [Range(0,1.0f)] public float chanceForCorrectMatch = 0.5f;

    [TitleGroup("Match Logic ", HorizontalLine = false), LabelText("Priority on high tile count")]
    [Range(0, 1)] public float highestCountPriority;

    [TitleGroup("Match Logic ", HorizontalLine = false), LabelText("Min Matches Per Turn")]
    public int minMatchesPerTurn = 1;
    //public int maxMatchesPerTurn = 1;

    public bool didGetCorrectMatch
    {
        get
        {
            return Random.Range(0.0f, 1.0f) <= chanceForCorrectMatch ? true : false;
        }
    }


}
