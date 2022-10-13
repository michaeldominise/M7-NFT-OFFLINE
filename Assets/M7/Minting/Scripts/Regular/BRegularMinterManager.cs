using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using M7.GameData;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace M7.GameRuntime
{
	[System.Serializable]
	public class SlotTemplate 
	{
		public TextMeshProUGUI heroNameText;
		public TextMeshProUGUI heroIdTex;
		public List <Image> stars;
		public Image bgIdContainer;
		public Image bgSlotContainer;
		public TextMeshProUGUI emptyText;
	}
	
	public class BRegularMinterManager : BaseTeamManager<BaseCharacterInstance>
	{
		public List <string> heroInstanceIdList = new List<string> ();
		[SerializeField] BIncubatorItem bIncubatorItem;
		List<SaveableCharacterData> sCharacterData;
		
		[Header ("UI")]
		public Color normalColor;
		public Color hoverColor;
		[Space (5)]
		[SerializeField] TextMeshProUGUI tokenText;
		
		[Space (10)]
		public Image imgMintConnector;
		public Button bttnMintHero;
		public List <SlotTemplate> slotTemplate = new List<SlotTemplate> ();
		
		[Space (10)]
		[SerializeField] Transform trnPopOutBox;
		[SerializeField] Transform trnIncubatorBox;
		[SerializeField] Transform sltHeroContainer;
		[SerializeField] CharacterInstance_Avatar charactersInstancePrefAvatar;
		[SerializeField] Camera cmrPopOutBox;
		[Space (10)]
		[SerializeField] TextMeshProUGUI sltHeroIdText;
		[SerializeField] TextMeshProUGUI sltHeroElementText;
		[SerializeField] TextMeshProUGUI sltHeroLvlText;
		[SerializeField] TextMeshProUGUI sltHeroMintText;
		
		public void Awake ()
		{	
			InitMintData ();
		}
		
		private void InitMintData ()
		{
			foreach (Transform e in containers)
			{
				ClearAllChild (e);
			}
			
			sCharacterData = new List<SaveableCharacterData>();
			for (int i = 0; i < heroInstanceIdList.Count; i++) 
			{
				SaveableCharacterData f = PlayerDatabase.Inventories.Characters.FindItem(heroInstanceIdList[i]);
				sCharacterData.Add (f);
			}
					
			Init(sCharacterData, OnLoadDataSlots);
			SetLayerRecursively (containers[2].gameObject, 13); // Set Layer infront of the UI Layer.
		}
		
		private void OnLoadDataSlots ()
		{
			for (int i = 0; i < ActiveCharacters.Count; i++)
			{
				if (i < 2)
				{
					SetDataSlots (i, SetTokenCost);
				}
			}
		}
		
		private void SetTokenCost ()
		{
			tokenText.text = "100 GAI 0 M7";
			
			SaveableCharacterData f = PlayerDatabase.Inventories.Characters.FindItem(heroInstanceIdList[2]);
			
			for (int i = 0; i < ActiveCharacters.Count; i++)
			{
				if (f.InstanceID == ActiveCharacters[i].SaveableCharacterData.InstanceID)
				{
					sltHeroIdText.text = "ID:#  " + $"{int.Parse(ActiveCharacters[i].SaveableCharacterData.InstanceID):0000000}";
					sltHeroElementText.text = "Element:  " + ActiveCharacters[i].CharacterObject.Element.ElementType.ToString();
					sltHeroLvlText.text = "Lvl: " + ActiveCharacters[i].SaveableCharacterData.Level + "/30";
					sltHeroMintText.text = "Mint: " + "0/7";
				}
			}
		}
		
		private void SetDataSlots (int id, Action onFinish)
		{
			slotTemplate[id].heroNameText.text = ActiveCharacters[id].CharacterObject.DisplayStats.DisplayName;
			slotTemplate[id].heroIdTex.text = "#" + $"{int.Parse(ActiveCharacters[id].SaveableCharacterData.InstanceID):0000000}";
			slotTemplate[id].emptyText.text = "";
			
			switch(ActiveCharacters[id].CharacterObject.DisplayStats.Rarity)
			{
				case Skill.SkillEnums.RarityFilter.Common : 
					GetStars(id, 1);
					break;
				case Skill.SkillEnums.RarityFilter.Uncommon : 
					GetStars(id, 2);	
					break;
				case Skill.SkillEnums.RarityFilter.Rare: 
					GetStars(id, 3);
					break;
				case Skill.SkillEnums.RarityFilter.Epic : 
					GetStars(id, 4);
					break;
				case Skill.SkillEnums.RarityFilter.Legendary : 
					GetStars(id, 5);	
					break;
			}
			
			// Update UI effect in slots.
			slotTemplate[id].bgSlotContainer.color = hoverColor;
			for (int e = 0; e < 2; e++)
			{	
				if (heroInstanceIdList[e] != "-1")
					slotTemplate[e].bgIdContainer.gameObject.SetActive (true);
				else
					slotTemplate[e].heroNameText.text = "";
			}
			
			if (id < 1)
			{
				bttnMintHero.interactable = false;
				imgMintConnector.color = normalColor;
			}
			else
			{
				bttnMintHero.interactable = true;
				imgMintConnector.color = hoverColor;
				onFinish.Invoke();
			}
		}
		
		public void GetStars (int slotId, int countStars)
		{
			for (int i = 0; i < slotTemplate[slotId].stars.Count; i++)
			{
				slotTemplate[slotId].stars[i].gameObject.SetActive (false);
			}
			
			for (int i = 0; i < countStars; i++)
			{
				slotTemplate[slotId].stars[i].gameObject.SetActive (true);
			}
		}
		
		public void SetIcon(int index, SaveableCharacterData saveableCharacter, System.Action onFinish)
		{
			if (saveableCharacter == null || string.IsNullOrWhiteSpace(saveableCharacter.MasterID) || index < 0)
			{
				onFinish?.Invoke();
				return;
			}

			var charInstance = Instantiate(charactersInstancePrefAvatar, sltHeroContainer, false);
			charInstance.name = $"{charactersInstancePref.name} {saveableCharacter.MasterID}";
			charInstance.Init(saveableCharacter, onFinish);

			charInstance.gameObject.AddComponent<Button>().onClick.AddListener(() => OnSetSelectHero(saveableCharacter.InstanceID));
		}
		
		public void OnMatchConfirm ()
		{
			trnPopOutBox.gameObject.SetActive (false);
			cmrPopOutBox.enabled = true;
		
			ClearAllChild(containers[2]);
		}
		
		public void OnMintIncubator ()
		{
			trnIncubatorBox.gameObject.SetActive (true);
		}
		
		public void OnMintHero ()
		{
			bIncubatorItem.trnCanister.gameObject.SetActive (false);
			bIncubatorItem.trnCharacter.gameObject.SetActive (true);
		}
	
		public void OnSetSelectHero (string id)
		{
			Debug.Log ("OnSelect Hero : " + id);
			heroInstanceIdList[1] = id;
			heroInstanceIdList[2] = id;
			InitMintData ();
		}
		
		public void ClearAllChild (Transform parent)
		{
			foreach (Transform child in parent) {
				GameObject.Destroy(child.gameObject);
			}
		}
		
		public void OnSelectAvailableHero (bool IsOpen)
		{
			trnPopOutBox.gameObject.SetActive (IsOpen);
			cmrPopOutBox.enabled = IsOpen;
			ClearAllChild (sltHeroContainer);
			
			List<SaveableCharacterData> availableHero = PlayerDatabase.Inventories.Characters.GetItems();
			for (int i = 0; i < availableHero.Count; i++)
			{
				if (heroInstanceIdList[0].ToString() != availableHero[i].InstanceID)
				{
					SetIcon (i, availableHero[i], null);
				}
				Debug.Log (availableHero[i].MasterID);
			}
		}
	}	
}