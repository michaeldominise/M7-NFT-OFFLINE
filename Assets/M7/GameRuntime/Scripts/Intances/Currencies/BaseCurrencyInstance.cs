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
    public abstract class BaseCurrencyInstance : BaseInstance_ClickableUI<SaveableCurrencyData>
    {
        public CurrencyObject CurrencyObject => AssetReference?.Asset as CurrencyObject;

        public override void RefreshNonAssetReferenceDisplay() { }
    }
}