using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using M7.GameData;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using M7.Skill;
using M7.FX;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

namespace M7.GameRuntime
{
    public class UIBattleSettingsManager : SceneManagerBase
    {
        [SerializeField] Transform mainContanier;
        [SerializeField] CanvasGroup dimContainer;
        [SerializeField] GameObject surrenderContanier;
        [SerializeField] GameObject gearItems;
        [SerializeField] Transform[] gearIcon;

        [SerializeField] Sprite sfxOn;
        [SerializeField] Sprite sfxOff;

        [SerializeField] Sprite mscOn;
        [SerializeField] Sprite mscOff;

        bool IsAllowToClickGear { get; set; } = true;

        public static UIBattleSettingsManager Instance;
        public bool isGameDone;

        private void Start()
        {
            Instance = this;
            isGameDone = false;
        }

        public void ExecuteButton(Button button)
        {
            switch(button.name)
            {
                case "PauseButton":
                    if(IsAllowToClickGear)
                        Show(true);
                    break;
                case "ResumeButton":
                    if(IsAllowToClickGear)
                        Show(false);
                    break;
                case "Exit_Btn":
                    mainContanier.gameObject.SetActive(false);
                    surrenderContanier.SetActive(true);
                    break;
                case "SurrenderYesButton":
                    Show(false);
                    TransitionOverlay.Show(0);
                    PlayerDatabase.CampaignData.customStage = PlayerDatabase.CampaignData.currentStage;
                    LoadScene(BattleManager.Instance.GameFinishManager.mainScene, LoadSceneMode.Single, overwriteSceneLayer: 0, forceLoad: true);
                    break;
                case "SurrenderCancelButton":
                    Show(false);
                    break;
            }
        }

        public void SfxClick(Toggle toggle)
        {
            if (toggle.isOn)
                toggle.GetComponent<Image>().sprite = sfxOn;
            else
                toggle.GetComponent<Image>().sprite = sfxOff;
        }
        public void MscClick(Toggle toggle)
        {
            if (toggle.isOn)
                toggle.GetComponent<Image>().sprite = mscOn;
            else
                toggle.GetComponent<Image>().sprite = mscOff;
        }

#if !UNITY_EDITOR
        //void OnApplicationFocus(bool hasFocus) => Show(true);
        //void OnApplicationPause(bool pauseStatus) => Show(true);
        void OnApplicationFocus(bool hasFocus) {
            Debug.Log("OnApplicationFocus: " + hasFocus);
            if (!hasFocus)
            {
                Show(true);
            }
            else
            {
                IsAllowToClickGear = true;
            }
        }

        void OnApplicationPause(bool hasFocus)
        {
            Debug.Log("OnApplicationPause: " + hasFocus);
            if (!hasFocus)
            {
                Show(true);
            }
            else
            {
                IsAllowToClickGear = true;
            }
        }
#endif

        void Show(bool value)
        {
            if (isGameDone)
                return;

            if (TransitionOverlay.Instance.IsShow)
                return;

            dimContainer.DOFade(value ? 1 : 0, 0.5f);
            dimContainer.blocksRaycasts = value;

            IsAllowToClickGear = false;


            mainContanier.gameObject.SetActive(true);
            if (value)
            {
                Time.timeScale = 1;
                gearItems.transform.DOLocalMoveY(0, 0.5f).onComplete = () => { Time.timeScale = 0; IsAllowToClickGear = true; };

                for(int i = 0; i < gearIcon.Length; i++)
                    gearIcon[i].DORotate(new Vector3(0,0, -90f), 0.5f);
            }
            else
            {
                gearItems.transform.DOLocalMoveY(-460, 0.5f).onComplete = () => { mainContanier.gameObject.SetActive(false); IsAllowToClickGear = true; };
                for (int i = 0; i < gearIcon.Length; i++)
                    gearIcon[i].DORotate(new Vector3(0, 0, 0), 0.5f);
                Time.timeScale = 1;
            }

            surrenderContanier.SetActive(false);
        }
    }
}

