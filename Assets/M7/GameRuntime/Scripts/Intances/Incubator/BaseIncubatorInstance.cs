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
    public abstract class BaseIncubatorInstance : BaseRPGObjectInstance<SaveableIncubatorData>
    {
        public IncubatorObject IncubatorObject => AssetReference?.Asset as IncubatorObject;

        public override void RefreshNonAssetReferenceDisplay() { }
    }
}