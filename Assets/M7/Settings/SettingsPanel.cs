
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

namespace M7.Settings 
{
    public class SettingsPanel : MonoBehaviour 
    {
        private static readonly string SCENE_NAME = "SettingsScreen";

        [SerializeField] Toggle[] tabs;
        [SerializeField] Toggle pushNotificationToggle;
        [SerializeField] ToggleGroup tabGroup;
        [SerializeField] TextMeshProUGUI versionText;
        [SerializeField] TextAsset buildVersionTextAsset;
        //[SerializeField] private LocalizedString versionStringFormat = new LocalizedString("settings/popup/versionformat");

        public static int DefaultTabIndex { get; set; }

        #region Monobehaviour

        public void OnEnable()
        {
            //LocalizationManager.OnLocalizeEvent += OnLocalizationChanged;

        }

        public void OnDisable()
        {
            //LocalizationManager.OnLocalizeEvent -= OnLocalizationChanged;
        }

        private void Start() 
        {
            Init();
        }

        #endregion

        private void Init() 
        {
            if ( DefaultTabIndex < 0 )
                DefaultTabIndex = 0;
            else if ( DefaultTabIndex > tabs.Length - 1 )
                DefaultTabIndex = tabs.Length-1;
            Init( DefaultTabIndex );
            DefaultTabIndex = 0;

            //versionText.text = string.Format(versionStringFormat, buildVersionTextAsset.text.Substring(1));
        }

        private void Init(int idx) 
        {
            tabs[idx].isOn = true;

            //additiveScreen.AddListener( AdditiveScreen.TransitionState.Complete,
            //    isVisible => {

            //        if ( !isVisible )
            //            SceneTransition.UnloadScene( SCENE_NAME );

            //    } );

            InitPushNotifications();
        }

        private void OnLocalizationChanged()
        {
            //versionText.text = string.Format(versionStringFormat, Application.version);
        }

        public void OpenPage(string url) 
        {
            Application.OpenURL(url);
        }

        #region Push Notifications

        static readonly string KEY_PUSH_NOTIF = "push_notifs_enabled";

        public void TogglePushNotifications(bool isOn) 
        {
            PlayerPrefs.SetInt(KEY_PUSH_NOTIF, isOn ? 1 : 0);
        }

        void InitPushNotifications() 
        {
            var isOn = PlayerPrefs.GetInt(KEY_PUSH_NOTIF, 0) == 1;
            pushNotificationToggle.isOn = isOn;
        }

        #endregion
    }
}