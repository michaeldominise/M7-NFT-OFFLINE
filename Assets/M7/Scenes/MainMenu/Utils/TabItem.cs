using M7.GameData;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using M7.GameRuntime;
using UnityEngine.UI;
using DG.Tweening;

namespace M7
{
    [ExecuteInEditMode]
    public class TabItem : MonoBehaviour
    {
        public enum Transition { None, Fade, ColorChange, SpriteSwap }

        [SerializeField] Transition transitionType = Transition.ColorChange;
        [SerializeField] Toggle targetToggle;
        [SerializeField, ShowIf("@transitionType != Transition.None")] Graphic toggleGraphic;
        [SerializeField, ShowIf("@transitionType == Transition.ColorChange")] Color selectedColor = Color.white;
        [SerializeField, ShowIf("@transitionType == Transition.ColorChange")] Color unselectColor = Color.white;
        [SerializeField, ShowIf("@transitionType != Transition.SpriteSwap && transitionType != Transition.None")] float transitionDuration = 0.2f;
        [SerializeField, ShowIf("@transitionType == Transition.SpriteSwap")] Sprite onSprite;
        [SerializeField, ShowIf("@transitionType == Transition.SpriteSwap")] Sprite offSprite;

        private void OnEnable() => OnValueChanged(targetToggle?.isOn ?? false, 0);

        public void OnValueChanged(bool isOn) => OnValueChanged(isOn, transitionDuration);
        void OnValueChanged(bool isOn, float transitionDuration)
        {
            switch (transitionType)
            {
                case Transition.Fade:
                    toggleGraphic?.CrossFadeAlpha(isOn ? 1 : 0, transitionDuration, true);
                    break;
                case Transition.ColorChange:
                    toggleGraphic?.CrossFadeColor(isOn ? selectedColor : unselectColor, transitionDuration, true, true);
                    break;
                case Transition.SpriteSwap:
                    var toggleImage = toggleGraphic as Image;
                    if (!toggleImage)
                        break;
                    toggleImage.sprite = isOn ? onSprite : offSprite;
                    break;
            }
        }
    }
}
