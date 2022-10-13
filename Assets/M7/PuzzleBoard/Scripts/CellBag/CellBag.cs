/*
 * TileType.cs
 * Author: Cristjan Lazar
 * Date: Oct 8, 2018
*/

using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace M7.Match {
    [Serializable]
    public class TileBagItem
    {
        [FormerlySerializedAs("tileType")] public CellType cellType;
        public int qty;
    }

    /// <summary>
    /// Data container for a tile type's attributes.
    /// </summary>
    [CreateAssetMenu(fileName = "TileBag",menuName = "BGGamesCore/Match/TileBag")]
    public class CellBag : SerializedScriptableObject {
        [DisableInPlayMode, ShowInInspector, TableList(AlwaysExpanded = true)]
        public TileBagItem[] cellBagItems;

        [NonSerialized]
        CellType[] cellTypeListCache = null;
        public CellType[] TileTypeList
        { 
            get 
            {
                if (cellBagItems.Length == 0)
                    return cellTypeListCache;
                if (cellTypeListCache == null)
                {
                    var list = new List<CellType>();
                    for (var x = 0; x < cellBagItems.Length; x++)
                    {
                        if (cellBagItems[x].cellType == null)
                            continue;
                        for (var y = 0; y < cellBagItems[x].qty; y++)
                            list.Add(cellBagItems[x].cellType);
                    }
                    cellTypeListCache = list.ToArray();
                }
                return cellTypeListCache;
            } 
        }
    }

}

