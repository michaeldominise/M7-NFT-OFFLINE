using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M7.Settings
{
    public class SettingsVoiceLocalizationToggle : MonoBehaviour
    {
        [SerializeField] Toggle toggle;
        [SerializeField] string languageCode;

        public string LanguageCode { get { return languageCode; } }

        #region Monobehaviour events
        private void Reset()
        {
            toggle = GetComponent<Toggle>() ?? gameObject.AddComponent<Toggle>();
        }

        private void Start()
        {
            toggle.isOn = languageCode == SettingsAPI.VoiceLanguageCode;
            toggle.onValueChanged.AddListener(OnToggle);
        }
        #endregion

        void OnToggle(bool isOn)
        {
            if (!isOn)
                return;
            // LocalizationManager.CurrentLanguageCode = languageCode;
            SettingsAPI.VoiceLanguageCode = languageCode;
        }
    }
}
