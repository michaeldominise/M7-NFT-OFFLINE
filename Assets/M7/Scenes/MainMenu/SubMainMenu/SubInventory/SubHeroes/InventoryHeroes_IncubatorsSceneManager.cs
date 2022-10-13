using M7.CDN.Addressable;
using M7.GameData;
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
    public class InventoryHeroes_IncubatorsSceneManager : SceneManagerBase
    {
        public static InventoryHeroes_IncubatorsSceneManager Instance { get; private set; }

        [SerializeField] PopulateInventoryManager<IncubatorInstance_InventoryCard, SaveableIncubatorData> populateInventoryManager;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start() => populateInventoryManager.Populate(PlayerDatabase.Inventories.Incubators.GetSortedItems());
        private void Update() => populateInventoryManager.Update();
    }
}