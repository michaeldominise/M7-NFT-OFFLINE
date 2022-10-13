using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace M7.Tools
{
    [GlobalConfig("M7/ToolWindow/Editor/ScriptableObjects")]
    public class BaseOverview<OverviewType, DataType> : GlobalConfig<OverviewType> where OverviewType : GlobalConfig<OverviewType>, new() where DataType : Object
    {
        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
        public DataType[] AllData;

        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public virtual void UpdateOverview()
        {
            AllData = AssetDatabase.FindAssets($"t:{typeof(DataType).Name}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<DataType>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(x => x.name)
                .ToArray();
            EditorUtility.SetDirty(this);
        }
    }
}

