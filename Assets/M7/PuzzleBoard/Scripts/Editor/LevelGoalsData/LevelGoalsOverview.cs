using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Linq;
using UnityEditor;

namespace M7.Match
{
    [GlobalConfig("BGMatchCore/LevelEditor/Configs")]
    public class LevelGoalsOverview : GlobalConfig<LevelGoalsOverview>
    {
        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
        public LevelGoalsData[] AllLevelGoals;

        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void UpdateOverview()
        {
            // Finds and assigns all scriptable objects of type
            this.AllLevelGoals = AssetDatabase.FindAssets("t:LevelGoalsData")
                .Select(guid => AssetDatabase.LoadAssetAtPath<LevelGoalsData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
    }
}

