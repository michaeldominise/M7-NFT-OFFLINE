using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;

namespace M7
{
    public class Shop_GatchaReveal : SceneManagerBase
    {
        public static Shop_GatchaReveal Instance { get; private set; }

        [SerializeField] AssetReference gatchaScene;
        [SerializeField] AssetReference gatchRevealScene;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;
            }
        }
    }
}