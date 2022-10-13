using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime.Scripts.OnBoarding.Game
{
    [Serializable]
    public class OnBoardingModel
    {
        [SerializeField] private Image displayImage;
        [SerializeField] private Button nextButton;
        [SerializeField] private TMP_Text description;
        [SerializeField] private RectTransform box;
        [SerializeField] private RectTransform tail;

        public Image DisplayImage => displayImage;
        public Button NextButton => nextButton;
        public TMP_Text Description => description;
        public RectTransform Box => box;
        public RectTransform Tail => tail;
    }
}
