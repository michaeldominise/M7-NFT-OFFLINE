using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using Sirenix.OdinInspector;
using M7.Match;

public class SpawnedTileViewer : MonoBehaviour {

    [SerializeField] SpawnPool _spawnPool;

    List<Transform> tileInstances { get { return _spawnPool.spawned; } }



    public static SpawnedTileViewer Instance;


    void Awake()
    {
        Instance = this;
    }

    public int GetTilesOfType(CellType tileType)
    {

        int toReturn = 0; 
        for (int i = 0; i < tileInstances.Count; i++)
        {

            MatchGridCell mgt = tileInstances[i].GetComponent<MatchGridCell>();

            if (mgt != null)
            {
                if (mgt.CellTypeContainer.CellType == tileType)
                    toReturn++;
            }
        }

        return toReturn;
    }
}
