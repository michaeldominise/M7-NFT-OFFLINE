using M7.CDN.Addressable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "ChapterData", menuName = "Assets/M7/GameData/ChapterData")]
    public class ChapterData : ScriptableObject
    {
        [SerializeField] int index;
        [SerializeField] string displayName;
        [SerializeField] AssetReferenceT<Sprite> bannerAssetReference;
        [SerializeField] List<LevelData> levelDataList;

        public int Index => index;
        public string DisplayName => displayName;
        public AssetReferenceT<Sprite> BannerAssetReference => bannerAssetReference;
        public List<LevelData> LevelDataList => levelDataList;
        public LevelData FirstLevelData => levelDataList.FirstOrDefault();
        public LevelData LastLevelData => levelDataList.LastOrDefault();
    }
}
