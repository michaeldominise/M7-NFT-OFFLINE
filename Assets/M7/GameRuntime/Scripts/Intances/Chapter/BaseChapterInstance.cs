using M7.CDN.Addressable;
using M7.GameData;
using M7.Skill;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using M7.CDN;

namespace M7.GameRuntime
{
    [System.Serializable]
    public abstract class BaseChapterInstance : InitializableInstance<ChapterData>, IDisposableAssetReference
    {
        public ChapterData ChapterData => ObjectData;
        protected AssetReferenceT<Sprite> BannerAssetReference => ChapterData?.BannerAssetReference;
        public Sprite BannerAsset => BannerAssetReference?.Asset as Sprite;

        public AssetReference AssetReference => BannerAssetReference;

        public virtual void RefreshNonAssetReferenceDisplay() { }
        public virtual void OnBaseRPGObjectReferenceLoaded() { }
        public virtual void CleanInstance() => DisposeAssetReference();
        public virtual void DisposeAssetReference() => AddressableAssetDisposeManager.DisposeAssetReference(this);
        public virtual void ReleaseAsset() => AssetReference?.ReleaseAsset();
    }
}