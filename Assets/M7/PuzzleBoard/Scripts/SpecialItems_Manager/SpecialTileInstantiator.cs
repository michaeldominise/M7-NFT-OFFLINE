using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using M7.Match;
using M7.Skill;
using Sirenix.OdinInspector;
using HutongGames.PlayMaker;
using Gamelogic.Grids;
using System.Linq;

[ActionCategory("M7/SpecialTiles")]
public class SpecialTileInstantiator : FsmStateAction
{

    [Button]
    public override void OnEnter()
    {
        if(InitialMenuManager.Instance.SpecialTileSelectorManager != null)
            Execute(InitialMenuManager.Instance.SpecialTileSelectorManager.specialTiles);
        Finish();
    }

    public static void Execute(List<SkillObject> skillObjects)
    {
        HashSet<int> savedIdx = new HashSet<int>();
        var pointList = PuzzleBoardManager.Instance.ActiveGrid.Grid.ToPointList();
        var count = skillObjects.Count;

        for (int i = 0; i < count; i++)
        {
            int randomChildIdx = Random.Range(0, pointList.Count);
            while (savedIdx.Contains(randomChildIdx))
                randomChildIdx = Random.Range(0, pointList.Count);

            savedIdx.Add(randomChildIdx);
            MatchGridCell randomTile = PuzzleBoardManager.Instance.ActiveGrid.Grid[pointList[randomChildIdx]];

            SetToRandomTile(randomTile, skillObjects[0]);
        }
    }

    static void SetToRandomTile(MatchGridCell RandomTile, SkillObject SkillObject)
    {
        SkillQueueManager.Instance.AddSkillToQueue(RandomTile, SkillObject, false, null, true);
        InitialMenuManager.Instance.SpecialTileSelectorManager.specialTiles.Remove(SkillObject);
        //ExecuteAddedTile(RandomTile, RandomTile.GetComponent<MatchGridTile>().TouchSkillObject);ß
    }
}
