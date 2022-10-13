using M7.CDN.Addressable;
using M7.GameData;
using M7.ServerTestScripts;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace M7
{
    public abstract class SceneManagerBaseNavigationTab<NavigationType> : SceneManagerBase where NavigationType : Enum
    {
        [SerializeField] protected NavigationData<NavigationType>[] navigations;
        [SerializeField] Toggle initialToggle;
        [SerializeField] NavigationType initialNavigationType;
        [ShowInInspector, ReadOnly] protected NavigationData<NavigationType> CurrentNavigation { get; set; }

        protected virtual void Start()
        {
            ExecuteButton(initialNavigationType);
        }

        protected override void ExecuteToggleEvent(Toggle toggle)
        {
            if (toggle.isOn)
            {
                ExecuteButtonEvent(toggle.gameObject);

                ////WILL WORK ON THIS TOMORROW////
                
                //if (toggle.gameObject.name != "NavigationButton_Heroes")
                //    ExecuteButtonEvent(toggle.gameObject);
                //else
                //    Debug.Log("Ok");
                //RUN A LOCAL LEVEL CHECKER HERE
                //DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.ReadyLevel);
            }
        }

        protected override void ExecuteButtonEvent(GameObject button) => LoadScene(navigations.FirstOrDefault(x => x.TargetGameObject == button));
        public void ExecuteButton(NavigationType navigationType) => LoadScene(navigations.FirstOrDefault(x => x.Type.Equals(navigationType)));
        protected virtual void LoadScene(NavigationData<NavigationType> navigationData)
        {
            if (navigationData == null || string.IsNullOrWhiteSpace(navigationData.SceneToLoad.AssetGUID) || navigationData == CurrentNavigation)
                return;

            LoadScene(navigationData.SceneToLoad, LoadSceneMode.Additive, result => CurrentNavigation = navigationData, forceLoad: true);
        }
    }
}