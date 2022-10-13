using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using DG.Tweening;
using UnityEngine.AddressableAssets;

namespace M7
{
    public class MainMenuSceneManager : SceneManagerBaseNavigationTab<NavigationData.NavigationType_MainMenu>
    {
        public static MainMenuSceneManager Instance { get; private set; }

        [SerializeField] Cdn2Initializer cdn2Initializer;
        [SerializeField] Image headerBg;
        [SerializeField] Image navigationBg;
        [SerializeField] CanvasGroup comingSoonPanel;
        [SerializeField] AssetReference settingsScene;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void Start()
        {
            cdn2Initializer.StartDownloadCDN();
            GameRuntime.GameManager.Instance.InitAudio();
            base.Start();
        }

        protected override void ExecuteToggleEvent(Toggle toggle)
        {
            switch (toggle.gameObject.name)
            {
                case "NavigationButton_Leaderboard":
                    comingSoonPanel.DOFade(1, 0.5f).onComplete += () => comingSoonPanel.DOFade(0, 0.5f).SetDelay(2f);
                    break;
                case "NavigationButton_Settings":
                    LoadScene(settingsScene, UnityEngine.SceneManagement.LoadSceneMode.Additive, overwriteSceneLayer: 5);
                    break;
                default:
                    base.ExecuteToggleEvent(toggle);
                    break;
            }
        }

        protected override void LoadScene(NavigationData<NavigationData.NavigationType_MainMenu> navigationData)
        {
            if (navigationData == null || string.IsNullOrWhiteSpace(navigationData.SceneToLoad.AssetGUID) || navigationData == CurrentNavigation)
                return;

            headerBg.CrossFadeAlpha(navigationData.Type == NavigationData.NavigationType_MainMenu.Play ? 0.5f : 1f, 0, true);
            navigationBg.CrossFadeAlpha(navigationData.Type == NavigationData.NavigationType_MainMenu.Play ? 0.5f : 1f, 0, true);
            base.LoadScene(navigationData);
        }
    }
}
