using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using M7.GameData;

namespace M7.GameRuntime
{
    public class PuzzleOverlayHandler : MonoBehaviour
    {
        static Action<bool> onPuzzleOverlay;

        #region Public Variables
        [HideInInspector]
        bool isForeGround;

        #endregion 

        #region Private Variables
        //private float initialOverlay
        private SpriteRenderer[] spriteRenderers;
        #endregion

        #region Public Functions
        #endregion

        #region Private Functions
        public static void SetOverlay(bool value)
        {
            IsOverlayActive = value;
            onPuzzleOverlay?.Invoke(value);
        }
        public static bool IsOverlayActive { get; set; }

        public void InitForeGround()
        {
            IsOverlayActive = false;
            isForeGround = true;
            onPuzzleOverlay += OnPuzzleOverlay;
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            setOriginalSpriteRendererOrderInLayerTo1000();
        }

        private void setOriginalSpriteRendererOrderInLayerTo1000()
        {
            foreach (var item in spriteRenderers)
                item.sortingOrder = 1000;
        }

        private void OnPuzzleOverlay(bool overlay)
        {
            foreach (var item in spriteRenderers)
                item.DOFade(overlay ? 1 : 0, BattleSceneSettings.Instance.PuzzleBoardOverlayDuration);
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            if (isForeGround)
                onPuzzleOverlay -= OnPuzzleOverlay;
        }
        #endregion 
    }

}