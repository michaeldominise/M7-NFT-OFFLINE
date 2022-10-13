using M7;
using M7.GameData;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime
{
    public class UIBattle_CharacterCard : MonoBehaviour
    {
        [SerializeField] Image avatar;
        [SerializeField] Image background;
        [SerializeField] private TextMeshProUGUI QueueText;
        public CharacterInstance_Battle _characterInstanceBattle;
        private Color heroColor = new Color(0.2470588f, 0.6627451f, 0.9607844f);
        private Color EnemyColor = new Color(1,0.1137255f,0.145098f);
        public void Attach(CharacterInstance_Battle targetReference)
        {
            avatar.sprite = targetReference.IconAsset;
            background.color = targetReference.IsPlayerObject ? heroColor : EnemyColor;
            transform.localScale = targetReference.IsPlayerObject ? transform.localScale : new Vector3(0.9f,0.9f,0.9f);
            _characterInstanceBattle = targetReference;
        }

        public void InitilalizeQueueNo(float value)
        {
            QueueText.text=value.ToString();

        }
    }
}