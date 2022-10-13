using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Skill;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using M7.GameRuntime;

public class AttackInformationManager : MonoBehaviour
{
    [SerializeField] Image skillAvatar;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] CanvasGroup canvasGroup;

    public IEnumerator SetInfo(SkillObject skillObject, float duration)
    {
        canvasGroup.DOFade(1,1);
        bool isReverse = (TurnManager.Instance.CurrentState != TurnManager.State.EnemyTurn);
        canvasGroup.GetComponent<HorizontalLayoutGroup>().reverseArrangement = isReverse;
        if (isReverse)
        {
            skillAvatar.transform.localScale = new Vector3 (1,1,1);
        }  
        else
        {
            skillAvatar.transform.localScale = new Vector3 (-1,1,1);
        }
        skillAvatar.sprite = skillObject.DisplayStats.skillimage;
        skillName.text = skillObject.DisplayStats.DisplayName;

        yield return new WaitForSeconds(duration);
        canvasGroup.DOFade(0, 1);
    }
}
