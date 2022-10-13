using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    [System.Serializable]
    public class RpgObjectDisplayStats
    {
        [SerializeField] private string displayName;
        [SerializeField] private string descriptionText;
        [SerializeField] private AssetReferenceT<Sprite> icon;
        public string DisplayName => displayName;
        public string DescriptionText => descriptionText;
        public AssetReferenceT<Sprite> Icon => icon;
    }
}

