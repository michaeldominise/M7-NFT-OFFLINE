using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine.Serialization;

namespace M7.Match
{
    [GlobalConfig("BGMatchCore/LevelEditor/Configs")]
    public class CellBagOverview : GlobalConfig<CellBagOverview>
    {
        [ReadOnly]
        [ListDrawerSettings(Expanded = true)]
        [FormerlySerializedAs("AllTileBags")]public CellBag[] AllCellBags;

        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void UpdateOverview()
        {
            // Finds and assigns all scriptable objects of type Character
            this.AllCellBags = AssetDatabase.FindAssets("t:CellBag")
                .Select(guid => AssetDatabase.LoadAssetAtPath<CellBag>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
    }
}

