using M7.GameData;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GlobalLevelData
{
    public static int levels;
    public static float time;
    public static float gaiCost;
    public static float m7Cost;
    
    public static void SetValues(SaveableCharacterData sCharacterData)
    {
        int BaseLevel = PlayerDatabase.GlobalDataSetting.PlayerAllLevelChartData.GetPlayerChart(sCharacterData.Level).levels + 1;
        //levels = ;
        time = PlayerDatabase.GlobalDataSetting.PlayerAllLevelChartData.GetPlayerChart(BaseLevel).time;
        gaiCost = PlayerDatabase.GlobalDataSetting.PlayerAllLevelChartData.GetPlayerChart(BaseLevel).gaiCost;
        m7Cost = PlayerDatabase.GlobalDataSetting.PlayerAllLevelChartData.GetPlayerChart(BaseLevel).m7Cost;
    }
}
