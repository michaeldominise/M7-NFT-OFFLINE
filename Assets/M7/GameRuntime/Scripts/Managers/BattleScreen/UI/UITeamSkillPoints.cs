using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime
{
    public class UITeamSkillPoints : MonoBehaviour
    {
        [SerializeField] UIDelayValue currentSkillPoints;
        [SerializeField] UIDelayValue maxSkillPoints;
        [SerializeField] ParticleSystem manaFullParticle;

        InstanceActions_TeamStats InstanceActions { get; set; }

        public void Init(float maxSkillPoints, InstanceActions_TeamStats instanceActions)
        {
            ResetEvents();
            this.maxSkillPoints.SetValue(maxSkillPoints);
            InstanceActions = instanceActions;
            InstanceActions.onSkillPointsUpdate += currentSkillPoints.SetValue;
            InstanceActions.onSkillPointsUpdate += ShowManaFullParticle;
        }

        void ShowManaFullParticle(float value) => manaFullParticle.gameObject.SetActive(maxSkillPoints.currentValue <= value);

        protected void ResetEvents()
        {
            if (InstanceActions != null)
            {
                InstanceActions.onSkillPointsUpdate -= currentSkillPoints.SetValue;
                InstanceActions.onSkillPointsUpdate -= ShowManaFullParticle;
            }
        }
    }
}