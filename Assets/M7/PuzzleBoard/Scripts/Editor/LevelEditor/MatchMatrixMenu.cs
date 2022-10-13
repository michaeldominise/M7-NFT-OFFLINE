using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.Match
{
    public class MatchMatrixMenu
    {
        [ShowInInspector, MatchMatrix]
        private CellType[] AllTileTypes
        { 
            get 
            { 
                TileTypeOverview.Instance.UpdateOverview();
                return TileTypeOverview.Instance.AllTileTypes;
            }
        }
    }
}
