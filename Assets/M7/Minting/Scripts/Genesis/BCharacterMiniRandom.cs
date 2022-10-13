using UnityEngine;
using UnityEngine.UI;
using M7.CDN.Addressable;
using Sirenix.OdinInspector;
using System.Linq;
using M7.GameData;
using System.Text;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;

namespace M7.GameRuntime
{
	[System.Serializable]
	public class CardTemplate 
	{
		public string title;
		public int starsCount;
		public Texture2D cardBannerImg;
		public Texture2D idContainerImg;
		public Color idContainerColor = Color.white;
	}
	
	[System.Serializable]
	public class CardSettings 
	{
		public string type; 
		public int exportCount;
		public int cardTemplateId;
	}
	
		
	[System.Serializable]
	public class StatsRandom
	{
		public string rarity;
		public int min;
		public int max;
	} 
	
	public class BCharacterMiniRandom : CharacterInstance_Spine
	{
		public List<GameData.EquipmentItem> equipments = new List<GameData.EquipmentItem>();
		
		[Header("Card Settings")]
		[SerializeField] List<CardSettings> cardSettings = new List<CardSettings> ();
		[SerializeField] List<string> heroesList = new	List<string>();
		[SerializeField] BUtilStringCombination bUtilStringCombination;

		[Header("Meta Data")]
		[SerializeField] BMinterMetaData bMinterMetaDataNft; // metada data for nft. 
		[SerializeField] BEquipmentData bEquipmentData; // get all exported equipment data's.
		[SerializeField] List <StatsRandom> bMinterStatsRandomList = new List<StatsRandom> ();

		[Header("UI")]
		[SerializeField] List<BEquipmentMiniItem> equipItems = new List<BEquipmentMiniItem>();
		[SerializeField] List<BEquipmentAttributes> attributeItems = new List<BEquipmentAttributes>();
		[SerializeField] List<CardTemplate> cardTemplates = new List<CardTemplate>();
		[SerializeField] List<Transform> cardStars = new List<Transform>();
		
		[SerializeField] Transform rollBttn;
		[SerializeField] Transform stopBttn;
		[SerializeField] Transform confrmBttn;
		[SerializeField] Transform equipmentsPanel;
		[SerializeField] Image elementIcon;
		[SerializeField] Image cardBannerPanel;
		[SerializeField] Image idContainerPanel;
		[SerializeField] TextMeshProUGUI idText;
		[Space (10)]
		[SerializeField] TextMeshProUGUI animText;
		List<string> heroAnimation = new List<string> {"Default", "TakenHit","Death", "Move", "Attack", "Skill", "SkillLegendary"};
		Animator heroMainSpineAnimator;
		Animator heroSubSpineAnimator;
		int animId = 0;
		
		public void OnPlayHeroAnim ()
		{
			animText.text = heroAnimation[animId];
			if (heroAnimation[animId] != "Move")
			{
				MainSpineInstance.SetTriggerAnimation(heroAnimation[animId]);
				MainSpineInstance.MoveAnimation(false, 1);
				
				if (SubSpineInstance != null)
				{
					SubSpineInstance.SetTriggerAnimation(heroAnimation[animId]);
					SubSpineInstance.MoveAnimation(false, 1);
				}
			}
			else
			{
				MainSpineInstance.MoveAnimation(true, 1);
				
				if (SubSpineInstance != null)
				{
					SubSpineInstance.MoveAnimation(true, 1);
				}
			}
		}
		
		public void OnPlayAnimNext ()
		{
			if (animId < heroAnimation.Count-1)
			{
				animId++;
				OnPlayHeroAnim ();
			}
		}
		
		
		public void OnPlayAnimPrev ()
		{
			if (animId != 0)
			{
				animId--;
				OnPlayHeroAnim ();
			}
		}

		public void LoadHero ()
		{
			animId = 0;
			Camera ce = FindObjectOfType<Camera>();
			ce.orthographicSize = 5;
			Init (new SaveableCharacterData (heroesList[0], heroesList[0], 0, new string [] {}), LoadAttributesDataUI);
			animText.text = heroAnimation[0];
		}
	    
		public void LoadAttributesDataUI ()
		{
			attributeItems[0].UpdateUIAttributes(CharacterObject.CombatStats.Hp, CharacterObject.CombatStats.Hp);
			attributeItems[1].UpdateUIAttributes(CharacterObject.CombatStats.Attack, CharacterObject.CombatStats.Attack);
			attributeItems[2].UpdateUIAttributes(CharacterObject.CombatStats.Defense, CharacterObject.CombatStats.Defense);
			attributeItems[3].UpdateUIAttributes(CharacterObject.CombatStats.Passion, CharacterObject.CombatStats.Passion);
			attributeItems[4].UpdateUIAttributes(CharacterObject.CombatStats.Luck, CharacterObject.CombatStats.Luck);
			attributeItems[5].UpdateUIAttributes(CharacterObject.CombatStats.Durability, CharacterObject.CombatStats.Durability);
			
			Texture2D tex = CharacterObject.Element.DisplayGemSprite.texture;
			elementIcon.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
			heroMainSpineAnimator = MainSpineInstance.GetComponentInChildren<Animator>();
			
			// Force to Stop Animation
			heroMainSpineAnimator.enabled = true;
			if (SubSpineInstance !=null)
			{
				heroSubSpineAnimator = SubSpineInstance.GetComponentInChildren<Animator>();
				heroSubSpineAnimator.enabled = true;
			}
		}
		
	    // Update is called once per frame
		public void SetRadomEquipements(GameData.Equipments.EquipmentType equipmentTypes, Action onFinish)
		{
			equipments.Clear();
            var equipmentAssetReferences = CharacterObject.Equipments.GetRandomEquipements(equipmentTypes).Select(x => x.GetAssetReference<GameData.EquipmentItem>()).ToArray();
            var loadedCount = 0;
            
			foreach (var equipmentAssetReference in equipmentAssetReferences)
            {
                equipmentAssetReference.LoadAssetAsync(result =>
                {
					//Debug.Log (result);
					equipments.Add(result);
                    loadedCount++;
                    if (loadedCount == equipmentAssetReferences.Length)
                        onFinish?.Invoke();
                    //MainSpineInstance.SpineSkinGenerator.SetSkin(result);
                });
            }
			equipments = ShuffleEquipment(equipments, true);
		}
		
		[SerializeField] StatsInstance_CharacterBasic statsInstance;
		public virtual StatsInstance_CharacterBasic StatsInstance => statsInstance;
        
		public override void OnPostLoadAssetReferenceLoaded()
		{
			base.OnPostLoadAssetReferenceLoaded();
			statsInstance.Init(this);
		}
       
		public void GetEquipements(GameData.Equipments.EquipmentType equipmentTypes, Action onFinish)
		{
			equipments.Clear();
			var equipmentAssetReferences = CharacterObject.Equipments.GetRandomEquipements(equipmentTypes).Select(x => x.GetAssetReference<GameData.EquipmentItem>()).ToArray();
			var loadedCount = 0;
			foreach (var equipmentAssetReference in equipmentAssetReferences)
			{
				equipmentAssetReference.LoadAssetAsync(result =>
				{
					Debug.Log (result);
					equipments.Add(result);
					loadedCount++;
					if (loadedCount == equipmentAssetReferences.Length)
						onFinish?.Invoke();
				});
			}
		}
		
		public void SelectEquipments (Equipments.EquipmentType equipmentType, int selectedId)
		{	
			Action actn = () => UpdateSelectEquipment(selectedId);
			GetEquipements(equipmentType, actn);
		}
		
		private void UpdateSelectEquipment (int selectedId)
		{
			for (int i = 0; i < equipItems.Count; i++)
			{
				if (equipItems[i].type.ToString() == equipments[selectedId].EquipmentType.ToString())
				{
					equipItems[i].name.text = equipments[selectedId].MasterID.Replace("EquipmentItem_", string.Empty);
					equipItems[i].description.text =  "";
					equipItems[i].slctId = selectedId;
					
					if (equipments[selectedId].IsSubSpine != true)
					{
						MainSpineInstance.SpineSkinGenerator.SetSkin(equipments[selectedId]);
					}
					else
					{
						if (equipItems[i].type == Equipments.EquipmentType.Weapon)
						{
							SubSpineInstance.SpineSkinGenerator.SetSkin(equipments[selectedId]);
						}
						if (equipItems[i].type != Equipments.EquipmentType.Weapon)
						{
							MainSpineInstance.SpineSkinGenerator.SetSkin(equipments[selectedId]);
						}
					}
				}
			}
		}
		
		private int resultCount = 0; 

		public void OnStartRoll (bool IsRollet)
		{
			if (IsRollet == true)
			{
				rollBttn.gameObject.SetActive (false);
				stopBttn.gameObject.SetActive (true);
				resultCount = 0;
				stopRoll = false;
				selectedId = 0;
				StartCoroutine(PlayRandomRoullet());
			}
			else 
			{
				StartCoroutine (PlayRandomRegular());
				rollBttn.gameObject.SetActive (false);
			}
		}

		public void OnStopRoll ()
		{
			stopRoll = true;
			stopBttn.gameObject.SetActive (false);
		}

		public void OnConfirm ()
		{
			//equipmentsPanel.gameObject.SetActive (false);
			confrmBttn.gameObject.SetActive (false);
			stopBttn.gameObject.SetActive (false);
			rollBttn.gameObject.SetActive (true);
			StopCoroutine (PlayRandomRoullet());
			LoadAttributesDataUI ();
		}
	
		[HideInInspector] public float delayTime = 0.1f;
		[HideInInspector] public bool stopRoll = false;
		[HideInInspector] public int selectedId = 0;
		IEnumerator PlayRandomRoullet() {
			GetEquipementType (resultCount);
			yield return new WaitForSeconds(0.3f);
			float delayTime = 0.1f;
			selectedId = 0;
			while (delayTime <= 0.6f)
			{
				if (selectedId < equipments.Count) {
					selectedId++;
						
					if (selectedId == equipments.Count){
						selectedId = 0;
					}
				}

				yield return new WaitForSeconds(delayTime);
				MainSpineInstance.SpineSkinGenerator.SetSkin(equipments[selectedId]);
				GetEquipmentResult();

				Debug.Log (delayTime);
					
				if (stopRoll == true){
					delayTime += 0.13f; // Adjust Speed Delay here.
				}
			}
				
			yield return new WaitForSeconds(0.12f);
			Debug.Log ("end");
			stopRoll = false;
			resultCount++;
			yield return new WaitForSeconds(0.1f);
			if (resultCount < 6)
			{
				StartCoroutine (PlayRandomRoullet());
			}
			else 
			{
				stopBttn.gameObject.SetActive (false);
				confrmBttn.gameObject.SetActive (true);
				yield return null;
			}
		}
		
		public void SetCardTemplate (int id)
		{
			Texture2D ctex = cardTemplates[id].cardBannerImg;
			Texture2D itex = cardTemplates[id].idContainerImg;

			cardBannerPanel.sprite = Sprite.Create(ctex, new Rect(0.0f, 0.0f, ctex.width, ctex.height), new Vector2(0.5f, 0.5f), 100.0f);
			idContainerPanel.sprite = Sprite.Create(itex, new Rect(0.0f, 0.0f, itex.width, itex.height), new Vector2(0.5f, 0.5f), 100.0f);
			idContainerPanel.color = cardTemplates[id].idContainerColor;
			
			for (int i = 0; i < cardStars.Count; i++) 
			{
				cardStars[i].gameObject.SetActive (false);
				for (int a = 0; a < cardTemplates[id].starsCount; a++) 
				{
					cardStars[a].gameObject.SetActive (true);
				}
			}
		}
		
		List <int> hIdList;
		public void SetRandomId () //Create and Random the List id's, to be used for filename.
		{
			hIdList = new List<int> ();
			int t = 0;
			for (int i = 0; i < cardSettings.Count; i++)
			{
				t += cardSettings[i].exportCount * heroesList.Count;
			}
			for (int e = 0; e < t; e++)
			{
				hIdList.Add (e+1);
			}
			hIdList = ShuffleId(hIdList, true);
		}
		
		IEnumerator PlayRandomRegular () 
		{
			SetRandomId ();
			int heroCount = 0;
			int hId = -1;// hero Id
			
			while (heroCount < heroesList.Count)
			{
				Init (new SaveableCharacterData (heroesList[heroCount], heroesList[heroCount], 0, new string [] {}), LoadAttributesDataUI);
				int cardCount = 0;
				
				while (cardCount < cardSettings.Count)
				{
					int r = 0;
					while (r < cardSettings[cardCount].exportCount)
					{
						SetCardTemplate (cardSettings[cardCount].cardTemplateId);
						List<EquipmentItem> tEquipment = new List<EquipmentItem>();
						resultCount = -1;
						
						yield return new WaitForSeconds(0.2f);						
						while (resultCount < 5)
						{
							resultCount++;
							GetEquipementType (resultCount);
							yield return new WaitForSeconds(0.3f);
							selectedId = UnityEngine.Random.Range(0, equipments.Count);
							tEquipment.Add(equipments[selectedId]);
						}
						yield return new WaitForSeconds(0.2f);
			
						hId++;
						idText.text = "#" + hIdList[hId].ToString("0000000");
						bEquipmentData.equipmentList.Add (new EquipmentList());
						bEquipmentData.equipmentList[hId].heroId = idText.text;

						for (int i  = 0; i < tEquipment.Count; i++) 
						{
							if (tEquipment[i].IsSubSpine != true)
							{
								MainSpineInstance.SpineSkinGenerator.SetSkin(tEquipment[i]);
							}
							else
							{
								SubSpineInstance.SpineSkinGenerator.SetSkin(tEquipment[i]);
							}
							
							bEquipmentData.equipmentList[hId].equipments.Add (tEquipment[i].name);
						}
						
						yield return new WaitForSeconds(0.2f);
				
						// Create New NFT Hero Metadata.
						bMinterMetaDataNft.name = "HERO " + idText.text;
						bMinterMetaDataNft.attributes[0].value = CharacterObject.DisplayStats.DisplayName;
						bMinterMetaDataNft.attributes[1].value = CharacterObject.Element.ElementType.ToString();
						bMinterMetaDataNft.attributes[2].value = CharacterObject.DisplayStats.Rarity.ToString();
					
						for (int i = 0; i < bMinterStatsRandomList.Count; i++)
						{
							if (bMinterStatsRandomList[i].rarity == bMinterMetaDataNft.attributes[2].value)
							{
								int rMin = bMinterStatsRandomList[i].min;
								int rMax = bMinterStatsRandomList[i].max;

								bMinterMetaDataNft.attributes[5].value = UnityEngine.Random.Range(rMin,rMax).ToString();
								bMinterMetaDataNft.attributes[6].value = UnityEngine.Random.Range(rMin,rMax).ToString();
								bMinterMetaDataNft.attributes[7].value = UnityEngine.Random.Range(rMin,rMax).ToString();
								bMinterMetaDataNft.attributes[8].value = UnityEngine.Random.Range(rMin,rMax).ToString();
							}
						}
				
						string [] socketsType = new string [] {"Attack", "Luck", "Passion", "Hp"};
						bMinterMetaDataNft.attributes[9].value = socketsType[UnityEngine.Random.Range (0, socketsType.Length)] + "/locked/empty";
						bMinterMetaDataNft.attributes[10].value = socketsType[UnityEngine.Random.Range (0, socketsType.Length)] + "/locked/empty";
						bMinterMetaDataNft.attributes[11].value = socketsType[UnityEngine.Random.Range (0, socketsType.Length)] + "/locked/empty";
						bMinterMetaDataNft.attributes[12].value = socketsType[UnityEngine.Random.Range (0, socketsType.Length)] + "/locked/empty";
					
						// Back Up Copy Json for In Game.
						Attribute ingAttri = new Attribute ();
						ingAttri.trait_type = "Equipment IDs";
						ingAttri.value = string.Join( ",", tEquipment).Replace(" (M7.GameData.EquipmentItem)", string.Empty);
						BCaptureWithAlpha.Instance.SaveCaptureRandom(hIdList[hId].ToString("0000000"), JsonUtility.ToJson(bMinterMetaDataNft), JsonUtility.ToJson(ingAttri));
				
						r++;
					}
					yield return new WaitForSeconds(0.1f);
					cardCount++;
				}
				yield return new WaitForSeconds(0.1f);
				heroCount++;
			}
			
			yield return new WaitForSeconds(0.1f);
			rollBttn.gameObject.SetActive (true);

			yield return null;
		}
		
		public void GetEquipementType (int id) {
			switch (resultCount)
			{
				case 0 : 
					SetRadomEquipements(Equipments.EquipmentType.Weapon, null);
				break;	
				case 1 :
					SetRadomEquipements(Equipments.EquipmentType.Armor, null);
				break;
				case 2 :
					SetRadomEquipements(Equipments.EquipmentType.Headgear, null);
				break; 
				case 3 :
					SetRadomEquipements(Equipments.EquipmentType.Gloves, null);
				break;
				case 4 :
					SetRadomEquipements(Equipments.EquipmentType.Shoes, null);
				break;
				case 5 : 
					SetRadomEquipements(Equipments.EquipmentType.Accessory, null);
				break;
			}
		}
	
		[Header("Stats")]
		public float hp; 
		public float attck; 
		public float df;
		public float ming; 
		public float lck; 
		public float drblty;
		
		private void UpdateCombatStats ()
		{
			hp = 0;
			attck = 0;
			df = 0;
			ming = 0;
			lck = 0; 
			drblty = 0;
			
			for (int i = 0; i < equipments.Count; i++)
			{
				hp += equipments[i].AdditionalStats.Hp;
				attck += equipments[i].AdditionalStats.Attack;
				df += equipments[i].AdditionalStats.Defense;
				ming += equipments[i].AdditionalStats.Passion;
				lck += equipments[i].AdditionalStats.Luck;
				drblty += equipments[i].AdditionalStats.Durability;
			}

			attributeItems[0].UpdateUIAttributes(CharacterObject.CombatStats.Hp, CharacterObject.CombatStats.Hp + hp);
			attributeItems[1].UpdateUIAttributes(CharacterObject.CombatStats.Attack, CharacterObject.CombatStats.Attack + attck);
			attributeItems[2].UpdateUIAttributes(CharacterObject.CombatStats.Defense, CharacterObject.CombatStats.Defense + df);
			attributeItems[3].UpdateUIAttributes(CharacterObject.CombatStats.Passion, CharacterObject.CombatStats.Passion + ming);
			attributeItems[4].UpdateUIAttributes(CharacterObject.CombatStats.Luck, CharacterObject.CombatStats.Luck + lck);
			attributeItems[5].UpdateUIAttributes(CharacterObject.CombatStats.Durability, CharacterObject.CombatStats.Durability + drblty);
		} 

		public void GetEquipmentResult () 
		{
			//equipmentsPanel.gameObject.SetActive (true);
			//equipItems[resultCount].gameObject.SetActive (true);

			for (int i = 0; i < equipItems.Count; i++)
            {
				if (equipItems[i].type.ToString() == equipments[selectedId].EquipmentType.ToString())
				{
					equipItems[i].name.text = equipments[selectedId].DisplayName;
					equipItems[i].description.text =  "";
				}
			}
			stopBttn.gameObject.SetActive (true);
			UpdateCombatStats ();
		}
		

		public static List<EquipmentItem> ShuffleEquipment (List<EquipmentItem> aList, bool isShuffle) {
		
			EquipmentItem e;
			if (isShuffle == true)
			{
				System.Random _random = new System.Random ();
				int n = aList.Count;
				for (int i = 0; i < n; i++)
				{
					int r = i + (int)(_random.NextDouble() * (n - i));
					e = aList[r];
					aList[r] = aList[i];
					aList[i] = e;
				}
			}	
	
			return aList;
		}
		
		public static List<int> ShuffleId (List<int> aList, bool isShuffle) {
		
			int e;
			if (isShuffle == true)
			{
				System.Random _random = new System.Random ();
				int n = aList.Count;
				for (int i = 0; i < n; i++)
				{
					int r = i + (int)(_random.NextDouble() * (n - i));
					e = aList[r];
					aList[r] = aList[i];
					aList[i] = e;
				}
			}	
			return aList;
		}     	
		
		private static BCharacterMiniRandom m_BCharacterMiniRandom;
		public static BCharacterMiniRandom Instance
		{
			get
			{
				if (m_BCharacterMiniRandom == null) { m_BCharacterMiniRandom = FindObjectOfType<BCharacterMiniRandom>(); }
				return m_BCharacterMiniRandom;
			}
		}
	}
}