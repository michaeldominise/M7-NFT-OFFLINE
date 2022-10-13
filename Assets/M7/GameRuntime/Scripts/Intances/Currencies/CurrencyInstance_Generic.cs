using System.Collections.Generic;
using UnityEngine;

    #if UNITY_EDITOR
using DG.Tweening;
using M7.Tools.Scripts;
    #endif

using M7.GameData;
using Sirenix.OdinInspector;
using M7.Skill;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace M7.GameRuntime
{
    public class CurrencyInstance_Generic : BaseCurrencyInstance
    {
        [SerializeField] string currencyID;
        [SerializeField] Image currencyImage;
        [SerializeField] TextMeshProUGUI displayName;
        [SerializeField] TextMeshProUGUI amount;
        [SerializeField] UnityEvent<float> setAmount;

        private void Start()
        {
            InitThis();
        }

        public void InitThis()
        {
            if (string.IsNullOrWhiteSpace(currencyID))
                return;

            Init(PlayerDatabase.Inventories.SystemCurrencies.FindItem(currencyID), null);
        }

        public override void OnBaseRPGObjectReferenceLoaded(Action onFinish)
        {
            if (currencyImage)
                currencyImage.sprite = CurrencyObject.DisplayImage;
            if (displayName)
                displayName.text = CurrencyObject.DisplayName;
            base.OnBaseRPGObjectReferenceLoaded(onFinish);
        }

        public override void RefreshNonAssetReferenceDisplay()
        {
            if (amount)
                amount.text = $"{SaveableData?.Amount.ToString("0.00") ?? "0.00"}";
            setAmount?.Invoke(SaveableData?.Amount ?? 0);
        }

        public override void CleanInstance()
        {
            base.CleanInstance();
            if (displayName)
                displayName.text = "";
            if(amount)
                amount.text = "";
        }
    }
}
