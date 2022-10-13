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
    public abstract class NavigationData
    {
        public enum NavigationType_MainMenu { Play, Inventory, Map, Marketplace, Leaderboard, Settings, Shop }
        public enum NavigationType_Inventory { Heroes, Boosters, Gems }
        public enum NavigationType_Inventory_Heroes { Heroes, Incubators }
    }

    [Serializable]
    public class NavigationData<NavigationType> : NavigationData where NavigationType : Enum
    {

        [SerializeField] AssetReference sceneToLoad;
        [SerializeField] NavigationType type;
        [SerializeField] GameObject targetGameObject;

        public AssetReference SceneToLoad => sceneToLoad;
        public NavigationType Type => type;
        public GameObject TargetGameObject => targetGameObject;
    }
}
