using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

namespace M7.GameRuntime
{
    public class UITurnIndicatorManager : MonoBehaviour
    {
        public BattleManager castedBattleManager { get { return BattleManager.Instance; } }

        [SerializeField] TextMeshProUGUI indicatorText;

        public void SetIndicator(string value)
        {
            indicatorText.text = value;
        }

        
        [Button]
        void InvokeTest()
        {
            SetIndicator("Test Turn");
        }

    }
}
