using System.Collections.Generic;
using Gamelogic.Grids;
using M7.Match;
using UnityEngine;

namespace M7.GameRuntime.Scripts.OnBoarding.Game.PuzzleBoardSegments
{
    public class PuzzleOnBoardingBehaviour_HighlightPuzzleBoard : OnBoardingSegmentBase
    {
        private bool _isActive;
        
        public override void Execute()
        {
            if(_isActive) return;

            _isActive = true;
            
            base.Execute();
            
            // panel
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.gameObject.SetActive(true);
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetBoxPosition(new Vector2(0, 197));
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetBoxSize(new Vector2(700, 205.84f));
            
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.ShowTail();
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetTailPosition(new Vector2(0, -124));
            
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.ShowDialog("Tap cubes beside drones to blast them");

            // show cube to click
            PuzzleBoardOnBoardingManager.Instance.possibleMove = GetPossibleMoves();
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.ShowPanel(ShowDialog);
            
            Debug.Log("Execute Stage 1 wave 1 highlight board.");
            Debug.Log($"<color=green>{LevelManager.LevelData.DisplayName}</color>");
        }

        public override void Exit() 
        {
            base.Exit();
            
            UnsetRenderingLayer();
            
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.gameObject.SetActive(false);
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.HidePanel();
            
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.DeactivatePuzzleCamera();
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.gameObject.SetActive(false);

            // if(LevelManager.LevelData.DisplayName == "Stage 1")
            //     PuzzleBoardOnBoardingManager.Instance.PopGameOnBoardingSegment("Stage 1_ShowCube");
        }

        private void ShowDialog()
        {
            SetRenderingLayer();

            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.ActivatePuzzleCamera();
        }

        private static void SetRenderingLayer()
        {
            foreach (var cell in PuzzleBoardOnBoardingManager.Instance.possibleMove)
            {
                var layerMask = LayerMask.NameToLayer("OnBoarding");
                cell.SpriteRenderer.maskInteraction = SpriteMaskInteraction.None;
                cell.SpriteRenderer.sortingLayerName = "OnBoarding";
                cell.SpriteRenderer.gameObject.layer = layerMask;

                cell.SubSpriteRenderer.maskInteraction = SpriteMaskInteraction.None;
                cell.SubSpriteRenderer.sortingLayerName = "OnBoarding";
                cell.SubSpriteRenderer.gameObject.layer = layerMask;

                cell.IconSpriteRenderer.maskInteraction = SpriteMaskInteraction.None;
                cell.IconSpriteRenderer.sortingLayerName = "OnBoarding";
                cell.IconSpriteRenderer.gameObject.layer = layerMask;
            }
        }
        
        private static void UnsetRenderingLayer()
        {
            foreach (var cell in PuzzleBoardOnBoardingManager.Instance.possibleMove)
            {
                // PuzzleBoardManager.Instance.ActiveGrid.Grid.Values

                var layerMask = LayerMask.NameToLayer("PuzzleBoard");
                cell.SpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                cell.SpriteRenderer.sortingLayerName = "Default";
                cell.SpriteRenderer.gameObject.layer = layerMask;

                cell.SubSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                cell.SubSpriteRenderer.sortingLayerName = "Default";
                cell.SubSpriteRenderer.gameObject.layer = layerMask;

                cell.IconSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                cell.IconSpriteRenderer.sortingLayerName = "Default";
                cell.IconSpriteRenderer.gameObject.layer = layerMask;
            }
        }

        public List<MatchGridCell> GetPossibleMoves()
        {
            var point1 = new RectPoint(2, 7);
            var point2 = new RectPoint(3, 7);
            var point3 = new RectPoint(4, 7);
            var point4 = new RectPoint(5, 7);
            
            var moves = new List<MatchGridCell>();
            
            moves.Add(PuzzleBoardManager.Instance.ActiveGrid.Grid.GetCell(point1));
            moves.Add(PuzzleBoardManager.Instance.ActiveGrid.Grid.GetCell(point2));
            moves.Add(PuzzleBoardManager.Instance.ActiveGrid.Grid.GetCell(point3));
            moves.Add(PuzzleBoardManager.Instance.ActiveGrid.Grid.GetCell(point4));

            return moves;
        }
    }
}