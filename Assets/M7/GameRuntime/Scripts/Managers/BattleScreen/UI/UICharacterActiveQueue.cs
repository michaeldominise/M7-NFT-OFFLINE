using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace M7.GameRuntime
{
    public class UICharacterActiveQueue : MonoBehaviour
    {
        //public static UICharacterActiveQueue Instance => BattleManager.Instance?.UICharacterActiveQueue;

        [SerializeField] Image activePlayerIcon;
        [SerializeField] TextMeshProUGUI activePlayerName;

        public Sprite nullImage;

        public void SetImage(Sprite icon, string name)
        {
            activePlayerIcon.sprite = icon;
            activePlayerName.text = name;
        }
    }
}