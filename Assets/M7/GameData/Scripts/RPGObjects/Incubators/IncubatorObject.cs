using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using M7.CDN.Addressable;
using M7.Skill;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "IncubatorObject", menuName = "Assets/M7/GameData/RpgObject/IncubatorObject")]
    public class IncubatorObject : RPGObject
    {

#if UNITY_EDITOR
        public static IncubatorObject ActiveIncubatorObject { get { return (IncubatorObject)Selection.activeObject; } }
#endif
        [SerializeField] private string displayName;
        [SerializeField] private string descriptionText;
        [SerializeField] private Sprite displayImage;
        [SerializeField] SkillEnums.RarityFilter rarity = SkillEnums.RarityFilter.Common;

        public string DisplayName => displayName;
        public string DescriptionText => descriptionText;
        public SkillEnums.RarityFilter Rarity => rarity;
        public Sprite DisplayImage => displayImage;
    }
}
