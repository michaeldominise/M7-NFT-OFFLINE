using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    [System.Serializable]
    public class SkinData
    {
        [SerializeField] string skinID;
        [SerializeField] string displayName;
        [SerializeField] string description;

        [SerializeField] LeveledAssetReferenceObject<Sprite> icons;
        [SerializeField] LeveledAssetReferenceObject<Sprite> avatars;
        [SerializeField] LeveledAssetReferenceObject<GameObject> spines;

        public string SkinID => skinID;
        public string DisplayName => displayName;
        public string Description => description;
        public LeveledAssetReferenceObject<Sprite> Icons => icons;
        public LeveledAssetReferenceObject<Sprite> Avatars => avatars;
        public LeveledAssetReferenceObject<GameObject> Spines => spines;
    }
}
