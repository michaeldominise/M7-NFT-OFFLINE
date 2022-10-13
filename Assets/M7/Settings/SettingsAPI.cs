using DarkTonic.MasterAudio;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Settings {
    public static class SettingsAPI {

        const string SOUND_BUS_KEY = "";
        public const string SOUND_PREF_KEY = "sounds_enabled";
        public const string MUSIC_PREF_KEY = "music_enabled";

        public const string GRAPHICS_QUALITY_KEY = "graphics_quality";
        public const string GRAPHICS_FRAMERATE_KEY = "graphics_framerate";

        const string GRAPHICS_BLOOM_KEY = "bloom_enabled";
        const string GRAPHICS_SHADOW_KEY = "shadow_enabled";
        const string GRAPHICS_FOG_KEY = "fog_enabled";
        const string GRAPHICS_ANTI_ALIAS_KEY = "anti_alias_enabled";
        const string GRAPHICS_VSYNC_KEY = "vsync_enabled";

        public const string INIT_SCENE_NAME = "Scene_Initialization";
               
        public const string LANGUAGE_CODE_PREFS_KEY = "lang-code";
        public const string VOICE_LANGUAGE_CODE_PREFS_KEY = "voice-lang-code";

        public static Action OnVoiceOverUpdated = null;

        public static bool SoundsEnabled {
            get { return PlayerPrefs.GetInt(SOUND_PREF_KEY, 1) == 1; }
            set {
                MasterAudio.MixerMuted = !value;
                PlayerPrefs.SetInt(SOUND_PREF_KEY, value ? 1 : 0);
            }
        }

        public static bool MusicEnabled {
            get { return PlayerPrefs.GetInt(MUSIC_PREF_KEY, 1) == 1; }
            set {
                MasterAudio.PlaylistsMuted = !value;
                PlayerPrefs.SetInt(MUSIC_PREF_KEY, value ? 1 : 0);
            }
        }

        public static string LanguageCode
        {
            get { return PlayerPrefs.GetString(LANGUAGE_CODE_PREFS_KEY, "en"); }
            set { PlayerPrefs.SetString(LANGUAGE_CODE_PREFS_KEY, value); }
        }

        public static string VoiceLanguageCode
        {
            get { return PlayerPrefs.GetString(VOICE_LANGUAGE_CODE_PREFS_KEY, "en"); }
            set
            {
                PlayerPrefs.SetString(VOICE_LANGUAGE_CODE_PREFS_KEY, value);

                if(OnVoiceOverUpdated != null)
                {
                    OnVoiceOverUpdated();
                }
            }
        }

        public static QualityManager.QualityLevel GraphicsQuality
        {
            get { return (QualityManager.QualityLevel)Enum.Parse(typeof(QualityManager.QualityLevel), PlayerPrefs.GetString(GRAPHICS_QUALITY_KEY, "HIGH")); }
            set
            {
                QualityManager.QLevel = value;
                PlayerPrefs.SetString(GRAPHICS_QUALITY_KEY, QualityManager.QLevel.ToString());
            }
        }

        public static QualityManager.QualityLevel FrameRate
        {
            get { return (QualityManager.QualityLevel)Enum.Parse(typeof(QualityManager.QualityLevel), PlayerPrefs.GetString(GRAPHICS_FRAMERATE_KEY, "NORMAL")); }
            set
            {
                QualityManager.FrameRateQualityLevel = value;
                PlayerPrefs.SetString(GRAPHICS_FRAMERATE_KEY, QualityManager.FrameRateQualityLevel.ToString());
            }
        }

        public static bool BloomEnabled
        {
            get { return PlayerPrefs.GetInt(GRAPHICS_BLOOM_KEY, 1) == 1; }
            set { PlayerPrefs.SetInt(GRAPHICS_BLOOM_KEY, value ? 1 : 0); }
        }

        public static bool ShadowEnabled
        {
            get { return PlayerPrefs.GetInt(GRAPHICS_SHADOW_KEY, 1) == 1; }
            set 
            { 
                QualitySettings.shadows = value ? ShadowQuality.HardOnly : ShadowQuality.Disable;
                PlayerPrefs.SetInt(GRAPHICS_SHADOW_KEY, value ? 1 : 0); 
            }
        }

        public static bool FogEnabled
        {
            get { return PlayerPrefs.GetInt(GRAPHICS_FOG_KEY, 1) == 1; }
            set 
            { 
                RenderSettings.fog = value;
                PlayerPrefs.SetInt(GRAPHICS_FOG_KEY, value ? 1 : 0); 
            }
        }

        public static bool AntiAliasingEnabled
        {
            get { return PlayerPrefs.GetInt(GRAPHICS_ANTI_ALIAS_KEY, 0) == 1; }
            set 
            { 
                QualitySettings.antiAliasing = value ? 2 : 0;
                PlayerPrefs.SetInt(GRAPHICS_ANTI_ALIAS_KEY, value ? 1 : 0); 
            }
        }

        public static bool VSyncEnabled
        {
            get { return PlayerPrefs.GetInt(GRAPHICS_VSYNC_KEY, 0) == 1; }
            set 
            { 
                QualitySettings.vSyncCount = value ? 2 : 0;
                PlayerPrefs.SetInt(GRAPHICS_VSYNC_KEY, value ? 1 : 0); 
            }
        }

        public static void TogglePushNotifications(bool isOn) {
            // TODO: implement
        }
    }
}