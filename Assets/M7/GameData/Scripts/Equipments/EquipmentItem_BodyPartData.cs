using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using M7.Skill;
using Spine.Unity;

namespace M7.GameData
{
    [System.Serializable]
    public class EquipmentItem_BodyPartData
    {
        [SerializeField] AssetReferenceT<Sprite> sprite;
        [SerializeField] string slot;
        [SerializeField] string slotKey;

        public AssetReferenceT<Sprite> Sprite => sprite;
        public string Slot => slot;
        public string SlotKey => slotKey;
    }
}
