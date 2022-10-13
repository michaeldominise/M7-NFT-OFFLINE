using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Linq;
using UnityEditor;

namespace M7.Match
{
    [GlobalConfig("BGMatchCore/LevelEditor/Configs")]
    public class TileTypeOverview : GlobalConfig<TileTypeOverview>
    {
        [ReadOnly]
        public int nextTileID = 64;

        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
        public CellType[] AllTileTypes;

        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void UpdateOverview()
        {
            // Finds and assigns all scriptable objects of type
            this.AllTileTypes = AssetDatabase.FindAssets("t:TileType")
                .Select(guid => AssetDatabase.LoadAssetAtPath<CellType>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
    }
}

