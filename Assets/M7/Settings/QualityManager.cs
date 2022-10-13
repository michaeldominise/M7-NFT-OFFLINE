using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using M7.Settings;

public static class QualityManager
{
    public enum QualityLevel : uint
    {
        LOW = 0,
        NORMAL,
        HIGH,
        VERY_HIGH
    }

    public static QualityLevel QLevel = QualityLevel.HIGH;
    public static QualityLevel FrameRateQualityLevel = QualityLevel.NORMAL;

    public static readonly Dictionary<QualityLevel, int> FrameRateMap = new Dictionary<QualityLevel, int>()
    {
        { QualityLevel.LOW, 15 },
        { QualityLevel.NORMAL, 30 },
        { QualityLevel.HIGH, 60 },
        { QualityLevel.VERY_HIGH, 120 }
    };

    private static readonly Dictionary<QualityLevel, IQuality> QualityMap = new Dictionary<QualityLevel, IQuality>()
    {
        { QualityLevel.LOW, new LowQuality() },
        { QualityLevel.NORMAL, new NormalQuality() },
        { QualityLevel.HIGH, new HighQuality() },
        { QualityLevel.VERY_HIGH, new VeryHighQuality() }
    };

    static QualityManager() { }

    public static void Initialize() 
    {
        if(!PlayerPrefs.HasKey(SettingsAPI.GRAPHICS_QUALITY_KEY))
        {
            //if(SystemInfo.graphicsMemorySize < 2048 && SystemInfo.systemMemorySize < 2048)
            //{
            //    SettingsAPI.GraphicsQuality = QualityLevel.LOW;
            //    SettingsAPI.FogEnabled = false;
            //    SettingsAPI.BloomEnabled = false;
            //    SettingsAPI.ShadowEnabled = false;
            //}
        }

        SetQuality(SettingsAPI.GraphicsQuality);
        SetFrameRate(SettingsAPI.FrameRate);
    }

    public static void SetQuality(QualityLevel level)
    {
        if(QualityMap.ContainsKey(level))
        {
            QLevel = level;
            QualityMap[level].ApplyChanges();
        }
    }

    public static void SetFrameRate(QualityLevel level)
    {
        if(FrameRateMap.ContainsKey(level))
        {
            FrameRateQualityLevel = level;
            Application.targetFrameRate = FrameRateMap[level];
        }
    }
}

public interface IQuality
{
    string QualityName { get; }
    int QualityIndex { get; }
    bool ApplyExpensiveChanges { get; }

    void ApplyChanges();
}

public class LowQuality : IQuality
{
    public string QualityName { get { return "M7 Low"; } }
    public int QualityIndex 
    { 
        get 
        { 
            for(int i = 0; i < QualitySettings.names.Length; i++)
            {
                if(QualitySettings.names[i] == QualityName)
                {
                    return i;
                }
            }

            return 0;
        }
    }
    
    public bool ApplyExpensiveChanges { get { return true; } }

    public void ApplyChanges()
    {
        QualitySettings.SetQualityLevel(QualityIndex, ApplyExpensiveChanges);
    }
}

public class NormalQuality : IQuality
{
    public string QualityName { get { return "Medium"; } }
    public int QualityIndex 
    { 
        get 
        { 
            for(int i = 0; i < QualitySettings.names.Length; i++)
            {
                if(QualitySettings.names[i] == QualityName)
                {
                    return i;
                }
            }

            return 0;
        }
    }
    public bool ApplyExpensiveChanges { get { return true; } }

    public void ApplyChanges()
    {
        QualitySettings.SetQualityLevel(QualityIndex, ApplyExpensiveChanges);
    }
}

public class HighQuality : IQuality
{
    public string QualityName { get { return "M7 High"; } }
    public int QualityIndex 
    { 
        get 
        { 
            for(int i = 0; i < QualitySettings.names.Length; i++)
            {
                if(QualitySettings.names[i] == QualityName)
                {
                    return i;
                }
            }

            return 0;
        }
    }
    public bool ApplyExpensiveChanges { get { return true; } }

    public void ApplyChanges()
    {
        QualitySettings.SetQualityLevel(QualityIndex, ApplyExpensiveChanges);
    }
}

public class VeryHighQuality : IQuality
{
    public string QualityName { get { return "M7 High"; } }
    public int QualityIndex 
    { 
        get 
        { 
            for(int i = 0; i < QualitySettings.names.Length; i++)
            {
                if(QualitySettings.names[i] == QualityName)
                {
                    return i;
                }
            }

            return 0;
        }
    }
    public bool ApplyExpensiveChanges { get { return true; } }

    public void ApplyChanges()
    {
        QualitySettings.SetQualityLevel(QualityIndex, ApplyExpensiveChanges);
    }
}