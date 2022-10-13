using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using System.Linq;
using M7.GameData;
using System;

namespace M7.GameRuntime
{
    public class CharacterInstance_Spine : BaseCharacterInstance
    {
        [SerializeField, FormerlySerializedAs("spineContainer")] Transform mainSpineContainer;
        [SerializeField] Transform subSpineContainer;

        [ShowInInspector, ReadOnly] public CharacterSpine MainSpineInstance { get; private set; }
        [ShowInInspector, ReadOnly] public CharacterSpine SubSpineInstance { get; private set; }
        [ShowInInspector, ReadOnly] public Vector3 SubSpineOffset { get; private set; }
        public Transform ActiveSpineContainer => SubSpineInstance ? subSpineContainer : mainSpineContainer;
        public Transform MainSpineContainer => mainSpineContainer;

        [SerializeField] StatsInstance_Character statsInstance_Character; 
        public override StatsInstance_Character StatsInstance => statsInstance_Character;

        public override void OnPostLoadAssetReferenceLoaded()
        {
            if (MainSpineAsset)
            {
                if (MainSpineInstance != null)
                    Destroy(MainSpineInstance.gameObject);
                MainSpineInstance = Instantiate(MainSpineAsset, mainSpineContainer);
                MainSpineInstance.SpineSkinGenerator.SetSkin(EquipmentItems.Where(x => !x.IsSubSpine).ToArray());
                SetLayerRecursively(MainSpineInstance.gameObject, gameObject.layer);
            }
            if (SubSpineAsset)
            {
                if (SubSpineInstance != null)
                    Destroy(SubSpineInstance.gameObject);
                SubSpineInstance = Instantiate(SubSpineAsset, subSpineContainer);
                SubSpineInstance.SpineSkinGenerator.SetSkin(EquipmentItems.Where(x => x.IsSubSpine).ToArray());
                SubSpineOffset = subSpineContainer.localPosition;
                SetLayerRecursively(SubSpineInstance.gameObject, gameObject.layer);
            }
            base.OnPostLoadAssetReferenceLoaded();
        }

        public override void RefreshNonAssetReferenceDisplay() { }
    }
}
