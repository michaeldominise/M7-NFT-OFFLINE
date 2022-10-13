using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData.Scripts.RPGObjects.Boosters
{
    [Serializable]
    public class BoosterObjectDisplayStats
    {
        [SerializeField] private string displayName;
        [SerializeField] private string descriptionText;
        [PreviewField(50, ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite icon;
        [PreviewField(50, ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite guideImage;
        public string DisplayName => displayName;
        public string DescriptionText => descriptionText;
        public Sprite Icon => icon;
        public Sprite GuideImage => guideImage;
    }
}