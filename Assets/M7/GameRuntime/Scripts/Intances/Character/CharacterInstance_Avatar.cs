using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    #if UNITY_EDITOR
using DG.Tweening;
using M7.Tools.Scripts;
    #endif

using M7.GameData;
using Sirenix.OdinInspector;
using M7.Skill;
using System.Collections;
using System;

namespace M7.GameRuntime
{
    public class CharacterInstance_Avatar : BaseCharacterInstance
    {
        [SerializeField] Image iconAvatarContainer;

        StatsInstance_Character statsInstance_Character = new StatsInstance_Character();
        public override StatsInstance_Character StatsInstance => statsInstance_Character;

        public override void OnPostLoadAssetReferenceLoaded()
        {
            base.OnPostLoadAssetReferenceLoaded();
            iconAvatarContainer.sprite = AvatarAsset;
            Debug.Log(AvatarAsset.name);

        }

        public override void RefreshNonAssetReferenceDisplay() { }
    }
}
