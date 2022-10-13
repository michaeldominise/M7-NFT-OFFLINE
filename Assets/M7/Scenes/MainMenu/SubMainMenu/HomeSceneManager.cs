using System;
using M7.CDN.Addressable;
using M7.GameData;
using M7.GameRuntime;
using System.Collections;
using System.Collections.Generic;
using M7.GameRuntime.Scripts.Energy;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Environment = M7.GameData.Environment;
using M7.ServerTestScripts;
using System.Linq;

namespace M7
{
    public class HomeSceneManager : SceneManagerBase
    {
        public static HomeSceneManager Instance { get; private set; }

        [SerializeField] ChapterDataList chapterDataList;
        [SerializeField] Camera targetCamera;
        [SerializeField] AssetReference stageInfoScene;
        [SerializeField] TextMeshProUGUI stageButtonName;
        [SerializeField] MenuTeamSelector menuTeamSelector;

        [SerializeField] TextMeshProUGUI maxGaiText;
        [SerializeField] TextMeshProUGUI collectedGaiText;
        //[SerializeField] UIDelayValue collectedGaiText;
        [SerializeField] UIDelayValue gaiMeter;
        [SerializeField] GameObject GaiLoadingObj;
        
        [SerializeField] TextMeshProUGUI energyText;
        [SerializeField] TextMeshProUGUI maxEnergyText;
        [SerializeField] TextMeshProUGUI eneryTimer;
        [SerializeField] UIDelayValue energyFill;

        string MarketplaceLink => "https://www.murasaki.com";

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        void Start()
        {
            var levelData = chapterDataList.GetCurrentLevelData();
            if (levelData == null)
                return;

            stageButtonName.text = levelData.DisplayName;
            levelData.Environment.LoadAssetAsync(environment =>
            {
                TransitionOverlay.Hide();
                var environmentObject = Instantiate(environment.GetComponent<Environment>(), targetCamera.transform);
                environmentObject.transform.localScale = Vector3.one * 0.76f; 
                environmentObject.Init(targetCamera);
                environmentObject.horizontalOffset = menuTeamSelector.PlatformParentObject.localPosition.x;
                menuTeamSelector.horizontalTransition.AddListener(value => environmentObject.horizontalOffset = value);
            });

            GaiLoadingObj.SetActive(true);
            DisplayDataFromServer();

            //EnergyManager.OnTimerTick += EnergyTimerUpdater;
            //EnergyManager.OnUpdateEnergyData += DisplayEnergy;
            //PlayfabDataManager.Instance.GetCollectedGaianiteInitial();
        }

        protected override void OnEnable()
        {
            EnergyManager.OnTimerTick += EnergyTimerUpdater;
            EnergyManager.OnUpdateEnergyData += DisplayEnergy;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            EnergyManager.OnTimerTick -= EnergyTimerUpdater;
            EnergyManager.OnUpdateEnergyData -= DisplayEnergy;
            base.OnDisable();
        }

        public void DisplayDataFromServer()
        {
            var gaiMaxTotal = PlayerDatabase.Inventories.SystemCurrencies.GetAmount("Gaianite_MaxTotal");
            var gaiToday = PlayerDatabase.Inventories.SystemCurrencies.GetAmount("Gaianite_Today");

            string gaiMaxTotalString = string.Format("{0:0.00}", gaiMaxTotal);
            string gaiTodayString = string.Format("{0:0.00}", gaiToday);
            maxGaiText.SetText(" / " + gaiMaxTotalString);
            collectedGaiText.SetText(gaiTodayString);

            gaiMeter.SetValue(gaiToday / gaiMaxTotal);
            GaiLoadingObj.SetActive(false);
        }

        private void DisplayEnergy()
        {
            eneryTimer.text = "Full";

            //energyText.text = $"{PlayerDatabase.Inventories.Energy.currentEnergy}/{PlayerDatabase.Inventories.Energy.energyCap}";
            maxEnergyText.SetText(" / " + PlayerDatabase.Inventories.Energy.energyCap);
            energyText.SetText(PlayerDatabase.Inventories.Energy.currentEnergy.ToString());

            var fillValue = (float)PlayerDatabase.Inventories.Energy.currentEnergy /
                            PlayerDatabase.Inventories.Energy.energyCap;
            energyFill.SetValue(fillValue);
        }

        private void EnergyTimerUpdater(string time)
        {
            eneryTimer.text = $"Next refill in {time}";
        }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch(gameObject.name)
            {
                case "Stage_Button":
                    LevelManager.ChapterData = chapterDataList.GetCurrentChapterData();
                    LevelManager.LevelData = chapterDataList.GetCurrentLevelData();
                    LoadScene(stageInfoScene, UnityEngine.SceneManagement.LoadSceneMode.Additive, overwriteSceneLayer: 3);
                    break;
                case "HelpButton":
                case "EmailButton":
                    Application.OpenURL(MarketplaceLink);
                    break;
            }
        }
    }
}