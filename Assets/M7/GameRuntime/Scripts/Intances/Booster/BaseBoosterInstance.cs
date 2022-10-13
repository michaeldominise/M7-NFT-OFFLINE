using M7.CDN.Addressable;
using M7.GameData;
using M7.Skill;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using M7.GameData.Scripts.RPGObjects.Boosters;

namespace M7.GameRuntime
{
    [System.Serializable]
    public abstract class BaseBoosterInstance : BaseInstance_ClickableUI<SaveableBoosterData>
    {
        public BoosterObject BoosterObject => AssetReference?.Asset as BoosterObject;

        public override void RefreshNonAssetReferenceDisplay() { }
    }
}