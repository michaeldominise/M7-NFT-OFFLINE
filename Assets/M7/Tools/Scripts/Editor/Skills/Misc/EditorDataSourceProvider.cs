using M7.Tools.Utility;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Source Provider for editor
/// This is lazily loaded so that when the editor needs the objects again,
/// it doesn't need to load it from the asset database again
/// </summary>
public static class EditorDataSourceProvider 
{
    private static List<GameObject> _skillObjectPrefabs;
    private static List<GameObject> _targetManagerPrefabs;
    private static List<GameObject> _customTargetFilters;
    private static List<GameObject> _sorters;
    private static List<GameObject> _statusEffectsPrefabs;

    public static List<GameObject> SkillObjectPrefabs
    {
        get
        {
            if(_skillObjectPrefabs == null)
            {
                _skillObjectPrefabs = GetSkillObjects();
            }

            return _skillObjectPrefabs;
        }
    }

    public static List<GameObject> TargetManagerPrefabs
    {
        get
        {
            if (_targetManagerPrefabs == null)
            {
                _targetManagerPrefabs = GetTargetManagers();
            }

            return _targetManagerPrefabs;
        }
    }

    public static List<GameObject> CustomTargetFilters
    {
        get
        {
            if(_customTargetFilters == null)
            {
                _customTargetFilters = GetPrefabs(EditorPath.CustomFiltersLocation);
            }

            return _customTargetFilters;
        }
    }

    public static List<GameObject> Sorters
    {
        get
        {
            if (_sorters == null)
            {
                _sorters = GetPrefabs(EditorPath.SortersLocation);
            }

            return _sorters;
        }
    }

    public static List<GameObject> GetVFXs()
    {
        return AssetUtility.GetAssets<GameObject>("t:prefab", new string[] { EditorPath.VFXLocation });
    }

    public static List<GameObject> GetSkillObjects()
    {
        return AssetUtility.GetAssets<GameObject>("t:prefab", new string[] { EditorPath.SkillObjectsLocation });
    }

    public static List<GameObject> GetTargetManagers()
    {
        return AssetUtility.GetAssets<GameObject>("t:prefab", new string[] { EditorPath.TargetManagersLocation });
    }

    public static List<GameObject> GetStatusEffects()
    {
        return AssetUtility.GetAssets<GameObject>("t:prefab", new string[] { EditorPath.StatusEffectsLocation });
    }

    private static List<GameObject> GetPrefabs(string path)
    {
        return AssetUtility.GetAssets<GameObject>("t:prefab", new string[] { path });
    }
}

/// <summary>
/// Default directory paths of each objects for saving or loading
/// </summary>
public static class EditorPath
{
    public static string SkillObjectsLocation = "Assets/M7/Skills/Prefab/Skills/Skills/Heroes";//"Assets/M7/Skills/Prefab/Skills/Skills";
    public static string TargetManagersLocation = "Assets/M7/Skills/Prefab/Templates/TargetManagers";
    public static string VFXLocation = "Assets/M7/FX/VFX/Prefabs";
    public static string StatusEffectsLocation = "Assets/M7/Skills/Prefab/Templates/StatusEffects";
    public static string CustomFiltersLocation = "Assets/M7/Skills/Prefab/Templates/TargetFilterItemCustom";
    public static string SortersLocation = "Assets/M7/Skills/Prefab/Templates/Sorters";

    public static string CustomLayoutsLocation = "Assets/Editor/Tools/Skills/CustomLayouts";
}