using M7.CDN.Addressable;
using M7.GameData;
using M7.GameData.Scripts.RPGObjects.Boosters;
using M7.GameRuntime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace M7
{
    public class InventoryBoostersSceneManager : SceneManagerBase
    {
        public static InventoryBoostersSceneManager Instance { get; private set; }

        [SerializeField] PopulateInventoryManager<BoosterInstance_InventoryCard, SaveableBoosterData> populateInventoryManager;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start() => populateInventoryManager.Populate(PlayerDatabase.Inventories.Boosters.GetSortedItems());
        private void Update() => populateInventoryManager.Update();
    }
}