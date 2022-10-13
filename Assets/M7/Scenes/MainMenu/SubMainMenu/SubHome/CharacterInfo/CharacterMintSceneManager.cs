using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using M7.GameData;
using DG.Tweening;
using M7.GameRuntime;
using M7.GameData;
using M7;
using Newtonsoft.Json;
using M7.ServerTestScripts;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterMintSceneManager : SceneManagerBase
{
   
    [System.Serializable]
    public class TicketUI
    {
        public Transform trParent;
        public Image sprite;
        public TMP_Text tMP_Text;
    }

    [SerializeField] CharacterInstance_InventoryCard characterSelectedL;
    [SerializeField] CharacterInstance_InventoryCard characterSelectedR;

    [SerializeField] CharacterInstance_InventoryCard characterSelectedM;

	[SerializeField] AssetReference incubatorScene;

    [Space(10)]
    [SerializeField] GameObject PopOutUI_Root;
    [SerializeField] GameObject Matching_Panel;
    [SerializeField] GameObject Gatcha_Panel;
    [Space(10)]
    [SerializeField] CanvasGroup noCompatibleHeroPanel;
    [SerializeField] Button plusBttn;
    [SerializeField] MintSelectableItem heroAvatar;

    [Space (10)]
    [SerializeField] Button mintHeroBttn;
    [SerializeField] GameObject mintBG;

    [SerializeField] GameObject mintTicketGroup;

    [SerializeField] TMP_Text mintTokenConsumption;

    [SerializeField] List<TicketUI> ticketUISlots = new List<TicketUI> ();

    [SerializeField] Transform heroContainer;   

    [SerializeField] MintUIColorContainer mintUIContainer;

    public void Start ()
    {
        var e = FindObjectOfType<CharacterInfoManager>();
        characterSelectedL.Init(e.sCharacterData[0], null);
    }

    List <SaveableCharacterData> pCharactersList = new List<SaveableCharacterData>();
    public void OnLoadHeroMatch ()
    {
        var pLCharacters = PlayerDatabase.Inventories.Characters;
        pCharactersList = pLCharacters.GetItems().FindAll(x => x.Level >= 10);
        if (pCharactersList.Count == 0)
        {
            noCompatibleHeroPanel.DOFade(1, 0.5f).onComplete += () => noCompatibleHeroPanel.DOFade(0, 0.5f).SetDelay(2f);
            return;
        }
        else
        {  
            PopOutUI_Root.gameObject.SetActive (true);
            Matching_Panel.gameObject.SetActive (true);

            characterSelectedM.Init(characterSelectedL.SaveableCharacterData, null); // Temporary.
            characterSelectedM.Init(pCharactersList[0], null);
            
            for (int i = 0; i < pCharactersList.Count; i++)
            {
                MintSelectableItem btAvatar = Instantiate (heroAvatar);
                btAvatar.transform.SetParent(heroContainer);
                btAvatar.transform.localScale = new Vector3 (1,1,1);
                btAvatar.transform.localPosition = Vector3.zero;
                btAvatar.gameObject.SetActive (true);
                btAvatar.charId = pCharactersList[i].InstanceID;
                btAvatar.characterSelectedM = characterSelectedM;

                if (i == 0)
                    btAvatar.buttonCharacter.interactable = false;

                btAvatar.Init(pCharactersList[i], null);
            }
        }
    }

    public void OnMint ()
    {
        PopOutUI_Root.gameObject.SetActive (true);
        Gatcha_Panel.gameObject.SetActive (true);
        Matching_Panel.gameObject.SetActive (false);
    }

    public void OnCollectTo ()
    {
        LoadScene(incubatorScene, UnityEngine.SceneManagement.LoadSceneMode.Single);

        var incbtr = new SaveableIncubatorData();
        var data = new Dictionary<string, object>();
        data["masterId"] = "IncubatorObject_Common";
        data["instanceID"] = (PlayerDatabase.Inventories.Incubators.GetItems().Count + 1).ToString();
        incbtr.OverwriteValues(JsonConvert.SerializeObject(data));

        PlayerDatabase.Inventories.Incubators.AddItem(incbtr);
    }

    public void OnConfirm ()
    {
        characterSelectedR.gameObject.SetActive(true);
        characterSelectedR.Init (characterSelectedM.SaveableCharacterData, null);
        plusBttn.targetGraphic.gameObject.SetActive (false);

        mintBG.gameObject.SetActive (false);
        mintHeroBttn.interactable = true;
        mintTokenConsumption.text = "1 GAI + 1 M7";

        mintTicketGroup.gameObject.SetActive (true);

        if (IsEqualMatchRarity (characterSelectedR.CharacterObject, characterSelectedL.CharacterObject))
        {
            ticketUISlots[0].trParent.gameObject.SetActive (true);
            var rarityIndex = (int)Mathf.Log((int)characterSelectedR.CharacterObject.DisplayStats.Rarity, 2);
            ticketUISlots[0].sprite.sprite = mintUIContainer.incubatorDesign[rarityIndex].ticket;
            ticketUISlots[0].tMP_Text.text = "x2";

            ticketUISlots[1].trParent.gameObject.SetActive (false);
        }
        else
        {
            ticketUISlots[0].trParent.gameObject.SetActive (true);
            var rarityIndexR = (int)Mathf.Log((int)characterSelectedR.CharacterObject.DisplayStats.Rarity, 2);
            ticketUISlots[0].sprite.sprite = mintUIContainer.incubatorDesign[rarityIndexR].ticket;
            ticketUISlots[0].tMP_Text.text = "x1";

            ticketUISlots[1].trParent.gameObject.SetActive (true);
            var rarityIndexL = (int)Mathf.Log((int)characterSelectedL.CharacterObject.DisplayStats.Rarity, 2);
            ticketUISlots[1].sprite.sprite = mintUIContainer.incubatorDesign[rarityIndexL].ticket;
            ticketUISlots[1].tMP_Text.text = "x1";
        }
    }

    public bool IsEqualMatchRarity (CharacterObject a, CharacterObject b) 
    {
        if (a.DisplayStats.Rarity == b.DisplayStats.Rarity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ClearAllChild (Transform parent)
    {
        foreach (Transform child in parent) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void OnClickBack ()
	{
		UnloadAtSceneLayer(sceneLayer);
	}
}
