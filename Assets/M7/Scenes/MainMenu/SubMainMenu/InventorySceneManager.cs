using System.Collections;
using M7;
using M7.CDN;
using M7.GameRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Linq;
using DarkTonic.MasterAudio;

namespace M7
{
    public class InventorySceneManager : SceneManagerBaseNavigationTab<NavigationData.NavigationType_Inventory>
    {
        public static InventorySceneManager Instance { get; private set; }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

    }
}
