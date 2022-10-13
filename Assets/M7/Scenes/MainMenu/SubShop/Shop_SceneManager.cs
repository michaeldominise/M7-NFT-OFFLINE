using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;

namespace M7
{
    public class Shop_SceneManager : SceneManagerBase
    {
        public static Shop_SceneManager Instance { get; private set; }

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
                case "Gatcha_Info_Button":
                    LoadScene(gatchaScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
                case "Disclaimer_Hero_Button":
                    MessageBox.Create("Hero Recruit Disclaimer", MessageBox.ButtonType.Ok).gameObject.SetActive(true);
                    break;
                case "Disclaimer_Ticket_Button":
                    MessageBox.Create("Hero Ticket Disclaimer", MessageBox.ButtonType.Ok).gameObject.SetActive(true);
                    break;
            }
        }

        public void ShowGatcha()
        {
            LoadScene(gatchRevealScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }
}
