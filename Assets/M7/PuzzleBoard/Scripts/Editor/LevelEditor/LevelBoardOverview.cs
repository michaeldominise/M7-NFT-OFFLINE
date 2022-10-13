using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Linq;
using UnityEditor;

namespace M7.Match
{
    [GlobalConfig("BGMatchCore/LevelEditor/Configs")]
    public class LevelBoardOverview : GlobalConfig<LevelBoardOverview>
    {
        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
        public MatchGridEditorSavedLevelData[] AllLevels;

        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void UpdateOverview()
        {
            // Finds and assigns all scriptable objects of type Character
            this.AllLevels = AssetDatabase.FindAssets("t:MatchGridEditorSavedLevelData")
                .Select(guid => AssetDatabase.LoadAssetAtPath<MatchGridEditorSavedLevelData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
    }
}

