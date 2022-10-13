using M7.GameRuntime.Scripts.UI.OverDrive;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEvents;
using M7.GameData;

namespace M7.GameRuntime
{
    public class UIBattle_StatusValueManager : MonoBehaviour
    {

        [SerializeField] UIDelayValue damageText;
        [SerializeField] TextMeshProUGUI damageLabel;
        [SerializeField] Sprite[] effectivitySprites;
        [SerializeField] Image effectivity;
        [SerializeField] Animator animator;
        [SerializeField] float lifeDuration = 2;

        public void Play(Vector3 worldPosition, float value, UIStatusValueManager.DamageType damageType, RPGElement rPGElement, bool isHeal)
        {
            transform.position = worldPosition;

            damageText.SetValueInstant(0);
            damageText.SetValue(Mathf.Floor(Mathf.Abs(value)));
            damageText.stringFormat = isHeal ? "+0" : "N0";
            damageLabel.color = isHeal ? Color.green : rPGElement?.ElementColor ??  Color.white;
            
            if (effectivity)
            {
                var effectivitySprite = effectivitySprites[isHeal == false ? (int)damageType : 1];
                effectivity.sprite = effectivitySprite;
                effectivity.gameObject.SetActive(effectivitySprite != null);
            }
           
            animator.SetTrigger(UIStatusValueManager.DamageType.Normal.ToString());
            StartCoroutine(Kill());
        }

        IEnumerator Kill()
        {
            yield return new WaitForSeconds(lifeDuration);
            UIStatusValueManager.Instance.Despawn(this);
        }
    }
}