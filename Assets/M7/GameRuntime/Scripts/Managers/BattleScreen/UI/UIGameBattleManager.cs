using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Org.BouncyCastle.Bcpg;
using Sirenix.OdinInspector;
using UnityEvents;

namespace M7.GameRuntime
{
    public class UIGameBattleManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI roundText;
        [SerializeField] TextMeshProUGUI chapterText;
        // [SerializeField] TextMeshProUGUI stageText;
        [SerializeField] UIWaveManager waveManager;
        //[SerializeField] Button endTurnBtn;
        [SerializeField] CanvasGroup enemyTurnIndicator;
        [SerializeField] UITeamSkillPoints uiTeamSkillPoints;
        [SerializeField] CanvasGroup uiSkillContainerCanvasGroup;
        [SerializeField] GameObject activityLogPanel;

        public delegate void StageUpdate(string stageName);

        public static event StageUpdate OnStageUpdate;
        
        public void Init()
        {
            chapterText.text = LevelManager.ChapterData.DisplayName;
            // stageText.text = LevelManager.LevelData.DisplayName;
            OnStageUpdate?.Invoke(LevelManager.LevelData.DisplayName);
            //waveManager.Init(LevelManager.LevelData.TeamData.Waves.Count);
        }

        public void PostInit() => uiTeamSkillPoints.Init(BattleManager.Instance.BattleSettings.MaxTeamSkillPoints, BattleManager.Instance.PlayerTeam.StatsInstance.InstanceActions);

        public void ActivateEndTurnButton(bool value)
        {
            //LeanTween.alphaCanvas(enemyTurnIndicator, value ? 0 : 1, 0.5f);
            enemyTurnIndicator.DOFade(value ? 0 : 1, 0.5f);
            enemyTurnIndicator.interactable = !value;
            enemyTurnIndicator.blocksRaycasts = !value;

            uiSkillContainerCanvasGroup.interactable = value;
        }

        public void UpdateWaveUI() => waveManager.UpdateUI(BattleManager.Instance.EnemyTeam.WaveIndex, 
            LevelManager.LevelData.TeamData.Waves.Count);
        public void RefreshRoundUI() => roundText.text = $"{  "Round " + BattleManager.Instance.BattleData.roundCount.ToString("00")}";

        public void UpdateStage(string pStageName) => OnStageUpdate?.Invoke(pStageName);
        
        public void GenericButton(string value)
        {
            switch(value)
            {
                case "ActivityLog":
                    activityLogPanel.SetActive(!activityLogPanel.activeInHierarchy);
                    break;
                case "GenericBtn_1":
                    break;
                case "GenericBtn_2":
                    break;
            }
        }
        
        #if UNITY_EDITOR
        [Button]
        private void TestStageName(string stageName)
        {
            OnStageUpdate?.Invoke(stageName);
        }
        #endif
    }
}