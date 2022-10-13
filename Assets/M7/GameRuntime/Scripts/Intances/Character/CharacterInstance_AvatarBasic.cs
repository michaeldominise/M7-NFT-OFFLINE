using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using M7.GameData;
using System;
using M7.CDN.Addressable;
using M7.CDN;
using UnityEngine.AddressableAssets;

namespace M7.GameRuntime
{
    public class CharacterInstance_AvatarBasic : InitializableInstance<AssetReferenceT<RPGObject>>, IDisposableAssetReference
    {
        [SerializeField] Image iconAvatarContainer;
        [SerializeField] Toggle toggle;
        [SerializeField] TabItem tabItem;

        public AssetReference AssetReference => ObjectData;
        public CharacterObject CharacterObject => AssetReference?.Asset as CharacterObject;

        public override void Init(AssetReferenceT<RPGObject> objectData, Action onFinish)
        {
            if(objectData == null)
            {
                onFinish?.Invoke();
                return;
            }
            base.Init(objectData, () =>
            {
                ObjectData.LoadAssetAsync(result =>
                {
                    AddressableAssetDisposeManager.AddDisposableAssetReference(this);
                    CharacterObject.DisplayStats.Avatar.LoadAssetAsync(sprite => iconAvatarContainer.sprite = sprite);
                    toggle.isOn = result.MasterID == PlayerDatabase.AccountProfile.AvatarId;
                });
                onFinish?.Invoke();
            });
        }

        public virtual void DisposeAssetReference() => AddressableAssetDisposeManager.DisposeAssetReference(this);
        public virtual void ReleaseAsset()
        {
            if (AssetReference.Asset == null)
                return;

            AssetReference.ReleaseAsset();
            CharacterObject.DisplayStats.Avatar.ReleaseAsset();
        }

        public void OnChangeValue(bool isOn)
        {
            if (isOn)
                onClickInstance?.Invoke(ObjectData);
        }
    }
}
