using M7.CDN.Addressable;
using M7.GameRuntime;
using M7.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace M7.GameData
{
    [System.Serializable]
    public class CharacterDisplayStats : RpgObjectDisplayStats
    {
        [SerializeField] AssetReferenceT<Sprite> avatar;
        [SerializeField] SkillEnums.RarityFilter rarity = SkillEnums.RarityFilter.Common;
        [SerializeField, FormerlySerializedAs("spine")] AssetReferenceT<GameObject> mainSpine;
        [SerializeField] AssetReferenceT<GameObject> subSpine;
        public AssetReferenceT<Sprite> Avatar => avatar;
        public AssetReferenceT<GameObject> MainSpine => mainSpine;
        public AssetReferenceT<GameObject> SubSpine => subSpine;
        public SkillEnums.RarityFilter Rarity => rarity;
    }
}
