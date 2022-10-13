using M7.CDN.Addressable;
using M7.GameData;
using M7.Skill;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameRuntime
{
    [System.Serializable]
    public abstract class BaseCharacterInstance : BaseRPGObjectInstance<SaveableCharacterData>
    {
        [Serializable] class LoadedAsset { public AssetType value = AssetType.None; }

        [Flags] public enum AssetType
        {
            None = 0,
            Spine = 1 << 0,
            Icon = 1 << 1,
            EquipmentItems = 1 << 2,
            EquipmentSkillObjects = 1 << 3,
            BassicSkillObjects = 1 << 4,
            Avatar = 1 << 5,
            All = (1 << 6) - 1
        }

        [SerializeField] AssetType preLoadAsset = AssetType.Spine;
        public SaveableCharacterData SaveableCharacterData => SaveableData;
        public CharacterObject CharacterObject => AssetReference?.Asset as CharacterObject;
        protected AssetReferenceT<GameObject> MainSpineAssetReference => CharacterObject?.DisplayStats.MainSpine;
        protected AssetReferenceT<GameObject> SubSpineAssetReference => CharacterObject?.DisplayStats.SubSpine;
        protected AssetReferenceT<Sprite> IconAssetReference => CharacterObject?.DisplayStats.Icon;
        protected AssetReferenceT<Sprite> AvatarAssetReference => CharacterObject?.DisplayStats.Avatar;

        public CharacterSpine MainSpineAsset => (MainSpineAssetReference?.Asset as GameObject)?.GetComponent<CharacterSpine>();
        public CharacterSpine SubSpineAsset => (SubSpineAssetReference?.Asset as GameObject)?.GetComponent<CharacterSpine>();
        public Sprite IconAsset => IconAssetReference?.Asset as Sprite;
        public Sprite AvatarAsset => AvatarAssetReference?.Asset as Sprite;


        protected AssetReferenceT<GameData.EquipmentItem>[] equipmentItemsReferences;
        protected AssetReferenceT<GameData.EquipmentItem>[] EquipmentItemsReferences
        {
            get
            {
                equipmentItemsReferences ??= CharacterObject?.Equipments.GetEquipements(SaveableCharacterData.EquipmentIDs).Select(x => x.GetAssetReference<GameData.EquipmentItem>()).Where(x => x != null)?.ToArray();
                return equipmentItemsReferences;
            }
        }
        public GameData.EquipmentItem[] EquipmentItems => EquipmentItemsReferences?.Where(x => !string.IsNullOrWhiteSpace(x.AssetGUID))?.Select(x => x.Asset as GameData.EquipmentItem).ToArray();
        public AssetReferenceT<GameObject>[] SkillAssetReferences => EquipmentItems?.Select(x => x?.SkillObject)?.Where(x => x != null)?.ToArray();
        public SkillObject[] SkillAssets => SkillAssetReferences?.Where(x => !string.IsNullOrWhiteSpace(x.AssetGUID))?.Select(x => (x.Asset as GameObject).GetComponent<SkillObject>()).ToArray();
        public AssetReferenceT<GameObject> BasicAttackSkillAssetReference => CharacterObject?.BasicAttackSkillObject;
        public AssetReferenceT<GameObject> CharacterSkillObjectAssetReference => CharacterObject?.CharacterSkillObject;
        public SkillObject BasicAttackSkillAsset => (CharacterObject.BasicAttackSkillObject.Asset as GameObject).GetComponent<SkillObject>();
        public SkillObject CharacterSkillAsset => (CharacterObject.CharacterSkillObject.Asset as GameObject).GetComponent<SkillObject>();
        public abstract StatsInstance_Character StatsInstance { get; }

        public override void OnBaseRPGObjectReferenceLoaded(Action onFinish)
        {
            var loadedAssets = new LoadedAsset();
            Action onFinishTrigger = () =>
            {
                if ((loadedAssets.value ^ preLoadAsset) == 0)
                {
                    OnPostLoadAssetReferenceLoaded();
                    onFinish?.Invoke();
                }
            };

            LoadAssetReference(AssetType.Spine, preLoadAsset, loadedAssets, onFinishTrigger, MainSpineAssetReference, SubSpineAssetReference);
            LoadAssetReference(AssetType.Icon, preLoadAsset, loadedAssets, onFinishTrigger, IconAssetReference);
            LoadAssetReference(AssetType.Avatar, preLoadAsset, loadedAssets, onFinishTrigger, AvatarAssetReference);
            LoadAssetReference(AssetType.EquipmentItems, preLoadAsset, loadedAssets, 
                () => LoadAssetReference(AssetType.EquipmentSkillObjects, preLoadAsset, loadedAssets, onFinishTrigger, SkillAssetReferences), 
                EquipmentItemsReferences);
            LoadAssetReference(AssetType.BassicSkillObjects, preLoadAsset, loadedAssets, onFinishTrigger, BasicAttackSkillAssetReference, CharacterSkillObjectAssetReference);
            if(preLoadAsset == AssetType.None)
                onFinishTrigger();
        }

        void LoadAssetReference<T>(AssetType assetType, AssetType preLoadAsset, LoadedAsset loadedAssets, Action onFinish, params AssetReferenceT<T>[] assetReferences) where T : UnityEngine.Object
        {
            if ((assetType | preLoadAsset) != preLoadAsset)
            {
                onFinish?.Invoke();
                return;
            }

            if (assetReferences == null || assetReferences.Length == 0)
            {
                loadedAssets.value |= assetType;
                onFinish?.Invoke();
                return;
            }

            var loadCount = 0;
            var loadedAssetNew = loadedAssets;
            foreach (var assetReference in assetReferences)
            {
                if (assetReference.AssetGUID == "d61a1c937b0f4324aaf7bf3bcf0a244a")
                    print("hit");
                assetReference.LoadAssetAsync(result =>
                {
                    loadCount++;
                    if (loadCount == assetReferences.Length)
                    {
                        loadedAssets.value |= assetType;
                        onFinish?.Invoke();
                    }
                });
            }
        }


        public virtual void OnPostLoadAssetReferenceLoaded()
        {
            StatsInstance.Init(this);
            StatsInstance.InitStatValues();
        }

        public override void ReleaseAsset()
        {
            base.ReleaseAsset();
            MainSpineAssetReference?.ReleaseAsset();
            IconAssetReference?.ReleaseAsset();
            if (SkillAssetReferences != null)
                foreach (var skillAssetReference in SkillAssetReferences)
                    skillAssetReference?.ReleaseAsset();
            if (equipmentItemsReferences != null)
            {
                foreach (var equipmentItemsReference in EquipmentItemsReferences)
                    equipmentItemsReference?.ReleaseAsset();
            }
            equipmentItemsReferences = null;
        }
    }
}