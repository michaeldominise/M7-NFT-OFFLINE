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
    public abstract class BaseGemInstance : BaseInstance_ClickableUI<SaveableGemData>
    {
        public GemObject GemObject => AssetReference?.Asset as GemObject;

        public override void RefreshNonAssetReferenceDisplay() { }
    }
}