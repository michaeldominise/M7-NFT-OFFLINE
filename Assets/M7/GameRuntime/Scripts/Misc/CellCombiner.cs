using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Match;
using DG.Tweening;
using Gamelogic.Grids;
public class CellCombiner : MonoBehaviour
{
    public static CellCombiner Instance => PuzzleBoardManager.Instance.TileCombiner;

    [SerializeField] AnimationCurve initialAnimationCurve;
    [SerializeField] AnimationCurve finalAnimationCurve;
    [SerializeField] float initialIntensity = 0.15f;

    MatchGrid ActiveGrid => PuzzleBoardManager.Instance.ActiveGrid;
    public static bool IsDone => combinerCount == 0;
    static int combinerCount = 0;

    public void TweenCell(PointList<RectPoint> rectPointList, RectPoint toucPoint, Action onFinish)
    {
        combinerCount = 0;
        var touchCell = ActiveGrid.Grid[toucPoint];
        combinerCount++;
        for (int i = 0; i < rectPointList.Count; i++)
        {
            var cell = ActiveGrid.Grid[rectPointList[i]];
            cell.PlayDeathAnim = false;
            cell.transform.SetAsLastSibling();
            cell.transform.localPosition += Vector3.back * 1;
            var isBasicElements = M7.Skill.SkillEnums.ElementFilter.BasicElements.HasFlag(cell.CellTypeContainer.CellType.ElementType);
            if (isBasicElements)
                cell.ShowGlow(true);
            var initialPosition = new Vector3(touchCell.transform.position.x, touchCell.transform.position.y, cell.transform.position.z) + (Vector3)((Vector2)cell.transform.position - (Vector2)touchCell.transform.position) * initialIntensity;
            cell.transform.DOMove(initialPosition, 0.25f).SetEase(initialAnimationCurve).onComplete += 
                () => cell.transform.DOMove(touchCell.transform.position, 0.1f).SetEase(finalAnimationCurve).onComplete += 
                    () =>
                    {
                        cell.ShowGlow(false);
                        if (cell != touchCell || !isBasicElements)
                            cell.ShowDisplay(false);
                    };
        }

        StartCoroutine(DelayOnFinish(0.25f, onFinish));
    }
    
    IEnumerator DelayOnFinish(float delay, Action onFinish)
    {
        yield return new WaitForSeconds(delay);
        combinerCount--;
        onFinish?.Invoke();
    }

    public void TweenCell(Action onFinish)
    {
        // for(int i = 0; i < connectedTile.Count; i++)
        // {
        //     connectedTile[i].gameObject.transform.DOLocalMove(new Vector3(0,0,0) , 1);
        // }

        transform.DOScale(2, 2).OnComplete(() => onFinish?.Invoke()).SetEase(Ease.Linear);
    }
}
