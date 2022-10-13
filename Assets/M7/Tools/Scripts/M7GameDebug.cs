#if UNITY_EDITOR

using System;
using System.Globalization;
using M7.Skill;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace M7.Tools.Scripts
{
    public class M7GameDebug : MonoBehaviour
    {
        [SerializeField] private bool isGameDebugOn;
        
        [SerializeField] private CanvasGroup parentPanel;

        [SerializeField] private Toggle toggleKillable;

        [SerializeField] private TMP_InputField inputField;
        
        private readonly string key = "KillableToggleKey";

        private readonly string timeScaleKey = "TimeScaleKey";
        
        private bool _toggleParent;

        public static M7GameDebug Instance;

        public bool IsKillable => toggleKillable.isOn;

        public bool IsGameDebugOn => isGameDebugOn;
        
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                toggleKillable.isOn = PlayerPrefs.GetInt(key, 1) == 1;
                Time.timeScale = PlayerPrefs.GetFloat(timeScaleKey, 1);
                inputField.text = Time.timeScale.ToString(CultureInfo.InvariantCulture);
                
                return;
            }

            Destroy(gameObject);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Time.timeScale = Time.timeScale > 1 ? 1 : 10f;
                PlayerPrefs.SetFloat(timeScaleKey, Time.timeScale);
                return;
            }
        
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            
            _toggleParent = !_toggleParent;
            
            parentPanel.gameObject.SetActive(_toggleParent);
        }

        [Button("Test IsKillable")]
        private void TestKillable()
        {
            Debug.Log($"Is Killable {IsKillable}");
        }

        public void Toggle(bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnEndEdit(string value)
        {
            Time.timeScale = float.Parse(value);
            PlayerPrefs.SetFloat(timeScaleKey, Time.timeScale);
        }
    }
}

#endif
