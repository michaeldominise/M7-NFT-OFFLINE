using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Settings {
    public class LanguagePrefs : MonoBehaviour {

        #region Monobehaviour events
        private void Start() {
            var languageCode = PlayerPrefs.GetString( SettingsAPI.LANGUAGE_CODE_PREFS_KEY, "en" );
            //LocalizationManager.CurrentLanguageCode = languageCode;
        }
        #endregion
    }
}