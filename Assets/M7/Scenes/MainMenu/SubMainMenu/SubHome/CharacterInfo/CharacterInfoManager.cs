using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using M7.GameData;
using System.Linq;
using M7.GameRuntime;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using M7.Skill;
using M7;
using M7.ServerTestScripts;
using System;
using PlayFab;
using Newtonsoft.Json;
using DG.Tweening;

namespace M7.GameRuntime
{
	public class CharacterInfoManager : BaseTeamManager<BaseCharacterInstance>
	{
		public static CharacterInfoManager Instance;
		[SerializeField] List <string> heroInstanceIdList = new List<string> ();
		public List<SaveableCharacterData> sCharacterData;
		
		[Header ("UI")]
		[SerializeField] TextMeshProUGUI adjectiveText;
		[SerializeField] TextMeshProUGUI heroNameText;
		[SerializeField] Image heroElement;
		[SerializeField] TextMeshProUGUI heroRarityText;
		[SerializeField] TextMeshProUGUI heroIdText;
		
		[SerializeField] Image rarityBg;
		[SerializeField] Color[] rarityColor;
		[SerializeField] Sprite[] rarityImages;
		[SerializeField] Sprite[] elementImages;
		[SerializeField] Sprite[] socketsImages;

		[SerializeField] CharacterSocketSlotItem [] socketSlotItems;

		
		[SerializeField] UIDelayValue mnt_Progressbar;
		[SerializeField] UIDelayValue lvl_Progressbar;
		[SerializeField] UIDelayValue drb_Progressbar;
		[SerializeField] TextMeshProUGUI levelingText;

		[Space (10)]
		[SerializeField] TextMeshProUGUI mintText;
		[SerializeField] TextMeshProUGUI lvlText;
		[SerializeField] TextMeshProUGUI durabilityText;
		[Space(10)]
		[SerializeField] TextMeshProUGUI skillNameText;
		[SerializeField] TextMeshProUGUI skillEffectCostText;
		[SerializeField] Image skillIconImg;
		[SerializeField] TextMeshProUGUI skillDescriptionText;
		
		[Space(10)]
		[SerializeField] TextMeshProUGUI sPointsText;
		[SerializeField] Button sPointsButton;
		// Stats
		[SerializeField] TextMeshProUGUI sAttckText;
		[SerializeField] TextMeshProUGUI sLckText;
		[SerializeField] TextMeshProUGUI sPssnText;
		[SerializeField] TextMeshProUGUI sHpText;
		
		[Space(10)]
		[SerializeField] UIDelayValue battck_Progressbar;
		[SerializeField] UIDelayValue blck_Progressbar;
		[SerializeField] UIDelayValue bpssn_Progressbar;
		[SerializeField] UIDelayValue bshp_Progressbar;
		
		[Space(10)]
		[SerializeField] UIDelayValue aattck_Progressbar;
		[SerializeField] UIDelayValue ablck_Progressbar;
		[SerializeField] UIDelayValue abpssn_Progressbar;
		[SerializeField] UIDelayValue abshp_Progressbar;
		
		
		[Space (20)]
		[SerializeField] Transform popOutTrans;
		[SerializeField] Camera popOutCam;
		
		[SerializeField] Transform lvlUpContainer;
		[SerializeField] Transform rcvrContainer;
		[SerializeField] Color colorInactive;
		[SerializeField] Button lvlButton;
		
		[Header ("PopOut UI")]
		[SerializeField] List<Transform> pPopUpItem = new List<Transform>();

		// Level Panel.
		[SerializeField] TextMeshProUGUI pLvlText;
		[SerializeField] TextMeshProUGUI pLvlToText;
		[SerializeField] TextMeshProUGUI pLvlCostText;
		[SerializeField] TextMeshProUGUI pLvlCostTimeText;
		[SerializeField] TextMeshProUGUI[] pLvlRemainingTimeText;
		// Recover
		[SerializeField] TextMeshProUGUI pDurabilityText;
		[SerializeField] TextMeshProUGUI pDurabilityCostText;
		[SerializeField] Slider pDurabilitySlider;
		
		InventoryHeroes_HeroesSceneManager hScManager;

		[SerializeField] CanvasGroup notEnoughCurrencyPanel;
		[SerializeField] TextMeshProUGUI notEnoughCurrencyText;

		public TimeSpan remainingTime;
		public static bool isReadyLevelUp;
		[SerializeField] Image baseButtonImage;
		private Color baseSelectedColor = Color.white;
		private Color baseUnselectedColor = new Color(0.73f, 0.73f, 0.73f, 1f);
		private bool baseSelected = true;

		string MarketplaceLink => "https://www.murasaki.com";

		private void Awake()
		{
			Instance = this;
			hScManager = FindObjectOfType<InventoryHeroes_HeroesSceneManager>();
			//Debug.Log (hScManager.CurentInstanceId);
			foreach (Transform e in containers)
			{
				ClearAllChild(e);
			}

			PopulateList();

			Init(sCharacterData, OnLoadHeroInfo);

			SetLayerRecursively(containers[0].gameObject, 5); // Set Layer infront of the UI Layer.
			SetLayerRecursively(containers[1].gameObject, 13); // Set Layer infront of the UI Layer.	
		}
        public class Levels 
		{
			public float amount;
		}
		
		Levels levels = new Levels();
		
		public void Start ()
		{		
		}

		SaveableCharacterData PopulateList()
        {
			sCharacterData.Clear();
			sCharacterData = new List<SaveableCharacterData>();
			for (int i = 0; i < heroInstanceIdList.Count; i++)
			{
				//heroInstanceIdList[i] = hScManager.CurentInstanceId;
				SaveableCharacterData f = PlayerDatabase.Inventories.Characters.FindItem(InventoryHeroes_HeroesSceneManager.CurrentInstanceId);
				sCharacterData.Add(f);
			}
			GlobalLevelData.SetValues(sCharacterData[0]);
			return sCharacterData[0];
		}

        private void Update()
        {
			if (IsLeveling())
			{
				var remainingTimeTemp = sCharacterData[0]?.LevelUpTimeComplete - DateTime.UtcNow;
				if (remainingTimeTemp?.Ticks <= 0)
				{
					sCharacterData[0].level++;
					sCharacterData[0].levelUpTimeComplete = DateTime.MinValue;
					sCharacterData[0].IsDirty = true;
					//if (isReadyLevelUp)
					//{
					//	//DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.LevelTimeFinish, sCharacterData[0].InstanceID);
					//	//Debug.Log("POP UP");
					//	isReadyLevelUp = false;
					//}
				}
				else
				{
					remainingTime = (TimeSpan)remainingTimeTemp;
				}
			}

			levelingText.gameObject.SetActive(IsLeveling());
			for (int i = 0; i < pLvlRemainingTimeText.Length; i++)
			{
				pLvlRemainingTimeText[i].text = remainingTime.ToString(@"hh\:mm\:ss");
			}
		}

        public void OnLoadHeroInfo ()
		{
			GlobalLevelData.SetValues(sCharacterData[0]);

			adjectiveText.text = ActiveCharacters[0].SaveableData.GetAdjectiveName(ActiveCharacters[0].CharacterObject.DisplayStats.DisplayName);
			heroNameText.text = ActiveCharacters[0].CharacterObject.DisplayStats.DisplayName;
			heroElement.sprite = ActiveCharacters[0].CharacterObject.Element.DisplayGemSprite;
			heroRarityText.text = ActiveCharacters[0].CharacterObject.DisplayStats.Rarity.ToString();
			heroIdText.text = "#" + $"{int.Parse(ActiveCharacters[0].SaveableCharacterData.InstanceID):0000000}";
			
			var rarityIndex = (int)Mathf.Log((int)ActiveCharacters[0].CharacterObject.DisplayStats.Rarity, 2);
			rarityBg.color = rarityColor[rarityIndex];
			rarityBg.sprite = rarityImages[rarityIndex];
			
			mintText.text = "Mint " + sCharacterData[0].MintCount.ToString() + "/" + sCharacterData[0].MintMax.ToString();
			lvlText.text = "Lv " + sCharacterData[0].Level.ToString();
			durabilityText.text = Mathf.Floor(sCharacterData[0].SaveableStats.Durability).ToString("0") + "/100";
				
			mnt_Progressbar.SetValueInstant((float)sCharacterData[0].MintCount/7f);
			lvl_Progressbar.SetValueInstant((float)sCharacterData[0].Level/30f);
			drb_Progressbar.SetValueInstant((float)sCharacterData[0].SaveableStats.Durability/100f);
			
			// stats point text.
			sPointsText.text = "+Point <color=red> (" + sCharacterData[0].AvailableStatsPoints.ToString() + ")</color>";

			//stats text
			setStringStats();


			float divisor = getHighestStats(sCharacterData[0].SaveableStats);
			// base stats progress bar.
			battck_Progressbar.SetValueInstant((float)sCharacterData[0].BaseStats.Attack / divisor);
            blck_Progressbar.SetValueInstant((float)sCharacterData[0].BaseStats.Luck / divisor);
            bpssn_Progressbar.SetValueInstant((float)sCharacterData[0].BaseStats.Passion / divisor);
            bshp_Progressbar.SetValueInstant((float)sCharacterData[0].BaseStats.Hp / divisor);

			// additional stats progress bar.
			aattck_Progressbar.SetValueInstant((float)sCharacterData[0].SaveableStats.Attack / divisor);
            ablck_Progressbar.SetValueInstant((float)sCharacterData[0].SaveableStats.Luck / divisor);
            abpssn_Progressbar.SetValueInstant((float)sCharacterData[0].SaveableStats.Passion / divisor);
            abshp_Progressbar.SetValueInstant((float)sCharacterData[0].SaveableStats.Hp / divisor);

   //         Animator heroMainSpineAnimator = ActiveCharacters[1].GetComponent<CharacterInstance_Spine>().MainSpineInstance.GetComponentInChildren<Animator>();
			//heroMainSpineAnimator.enabled = false;

			//if (ActiveCharacters[1].GetComponent<CharacterInstance_Spine>().SubSpineInstance != null)
			//{
			//	Animator heroSupMainSpineAnimator = ActiveCharacters[1].GetComponent<CharacterInstance_Spine>().SubSpineInstance.GetComponentInChildren<Animator>();;
			//	heroSupMainSpineAnimator.enabled = false;
			//}

			// slots ui
			if (!sCharacterData[0].IsNonNFT)
			{
				for (int i = 0; i < (sCharacterData[0].Sockets?.Length ?? 0); i++)
				{
					Debug.Log(sCharacterData[0].Sockets[i].statsType);
					switch (sCharacterData[0].Sockets[i].statsType)
					{
						case "Attack":
							socketSlotItems[i].socketsImage.sprite = socketsImages[0];
							socketSlotItems[i].socketsText.text = "A";
							break;
						case "Luck":
							socketSlotItems[i].socketsImage.sprite = socketsImages[1];
							socketSlotItems[i].socketsText.text = "L";
							break;
						case "Passion":
							socketSlotItems[i].socketsImage.sprite = socketsImages[2];
							socketSlotItems[i].socketsText.text = "P";
							break;
						case "Hp":
							socketSlotItems[i].socketsImage.sprite = socketsImages[3];
							socketSlotItems[i].socketsText.text = "H";
							break;
					}
				}
			}

			/// Available Points Button.
			if (sCharacterData[0].AvailableStatsPoints > 0)
			{
				sPointsButton.enabled = true;
				sPointsButton.transform.GetChild(0).gameObject.SetActive (true);
			}
			else
			{
				sPointsButton.enabled = false;
				sPointsButton.transform.GetChild(0).gameObject.SetActive (false);
			}

			Addressables.LoadAssetAsync<GameObject>(ActiveCharacters[0].CharacterObject.CharacterSkillObject).Completed += OnLoadSkillObjDone;
			//Disable Level Up Button if the chracter reach the max Level.
			if(ActiveCharacters[0].SaveableData.Level >= 30)
			{
				lvlButton.interactable = false;
				lvlButton.targetGraphic.color = colorInactive; 
			}			


			GetShowPopOutBox (false);
		}

		public bool IsLeveling()
		{
			if (sCharacterData[0].LevelUpTimeComplete == DateTime.MinValue)
				return false;
			else
				return true;
		}

		public void OnClickBoost()
        {
			if (!CheckGianiteAndM7Cost())
			{
				notEnoughCurrencyPanel.DOFade(1, 0.5f).onComplete += () => notEnoughCurrencyPanel.DOFade(0, 0.5f).SetDelay(2f);
			}
			else
			{
				pPopUpItem[0].gameObject.SetActive(false);
				pPopUpItem[1].gameObject.SetActive(false);
				pPopUpItem[2].gameObject.SetActive(true);
			}
		}

		public void OnClickConfirmBoost()
        {
            //DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.BoostLevel, sCharacterData[0].InstanceID);
			sCharacterData[0].level++;
			sCharacterData[0].levelUpTimeComplete = DateTime.MinValue;
			sCharacterData[0].IsDirty = true;

			lvlUpContainer.gameObject.SetActive(false);
			pPopUpItem[0].gameObject.SetActive(true);
			pPopUpItem[1].gameObject.SetActive(false);
			pPopUpItem[2].gameObject.SetActive(false);
		}

		public void LevelBoosted()
        {
			PopulateList();
			OnLoadHeroInfo();

			lvlUpContainer.gameObject.SetActive(false);
			rcvrContainer.gameObject.SetActive(false);
		}
		public void OnClickLevelUp ()
		{
			ShowPopUp();
		}

		public void ShowPopUp()
		{
			PopulateList();
			OnLoadHeroInfo();

			GetShowPopOutBox(true);
			lvlUpContainer.gameObject.SetActive(true);
			rcvrContainer.gameObject.SetActive(false);

			var utcZero = DateTime.MinValue;
			
			if (!IsLeveling())
			{
				pPopUpItem[0].gameObject.SetActive(true);
				pPopUpItem[1].gameObject.SetActive(false);
				pPopUpItem[2].gameObject.SetActive(false);
			}
			else
            {
				pPopUpItem[0].gameObject.SetActive(false);
				pPopUpItem[1].gameObject.SetActive(true);
				pPopUpItem[2].gameObject.SetActive(false);
			}

			pLvlText.text = lvlText.text;
			pLvlToText.text = "Level up to " + (sCharacterData[0].Level + 1).ToString();

			if(GlobalLevelData.m7Cost > 0)
				pLvlCostText.text = $"{GlobalLevelData.gaiCost} GAI + {GlobalLevelData.m7Cost} M7";
			else
				pLvlCostText.text = $"{GlobalLevelData.gaiCost} GAI";

			float mins = GlobalLevelData.time * 60;
			pLvlCostTimeText.text = $"{mins} Mins";
		}

		float sliderDefaultValue;
		float durMaxValue;

		public void OnClicKRecover ()
		{
			GetShowPopOutBox (true);
			rcvrContainer.gameObject.SetActive (true);
			lvlUpContainer.gameObject.SetActive (false);

			pDurabilityText.text = "Durability " + durabilityText.text;
			pDurabilitySlider.value = sCharacterData[0].SaveableStats.Durability / 100;

			//var initialDur = (pDurabilitySlider.value * 100) * DurabilityCost(sCharacterData[0].rarity.ToString());
			//if (sCharacterData[0].SaveableStats.Durability < sCharacterData[0].BaseStats.Durability)
			//	pDurabilityCostText.text = initialDur.ToString("0.0") + "GAI";
			//else
			pDurabilityCostText.text = "0 GAI";

			sliderDefaultValue = pDurabilitySlider.value;
			pDurabilitySlider.maxValue = 1;
			var absValue = pDurabilitySlider.value * 100;
			durMaxValue = Mathf.Abs(absValue - 100);
			Debug.Log(durMaxValue);
		}
		
		public void OnClickMint ()
		{
			GetShowPopOutBox (true);
		}
		
		public void OnClickMarketPlace () => Application.OpenURL(MarketplaceLink);
		
		public void OnCheckGianiteAndM7 ()
		{
			if (!CheckGianiteAndM7Cost())
			{
				notEnoughCurrencyPanel.DOFade(1, 0.5f).onComplete += () => notEnoughCurrencyPanel.DOFade(0, 0.5f).SetDelay(2f);
			}
			else
			{
				var gai = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite");
				gai.Amount -= GlobalLevelData.gaiCost;
				gai.IsDirty = true;
				var m7 = PlayerDatabase.Inventories.SystemCurrencies.FindItem("M7");
				m7.Amount -= GlobalLevelData.m7Cost;
				m7.IsDirty = true;

				sCharacterData[0].levelUpTimeComplete = DateTime.Now.AddMinutes(GlobalLevelData.time);
				sCharacterData[0].IsDirty = true;
				//DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.ConfirmLevel);
				OnConfirmLevel();
			}
		}

		public void OnConfirmLevel()
        {
			PopulateList();

			pPopUpItem[0].gameObject.SetActive(false);
			pPopUpItem[1].gameObject.SetActive(false);
			pPopUpItem[2].gameObject.SetActive(false);
			GetShowPopOutBox(false);

		}

		public bool CheckGianiteAndM7Cost()
		{
			var gianite = PlayerDatabase.Inventories.SystemCurrencies.GetAmount("Gaianite");
			var m7 = PlayerDatabase.Inventories.Currencies.GetAmount("M7");
			bool gianiteAvailable = GlobalLevelData.gaiCost <= gianite;
			bool m7Available = GlobalLevelData.m7Cost <= m7;

			if(gianiteAvailable && m7Available)
            {
				return true;
            }
			else
			{
				if (!gianiteAvailable && !m7Available)
				{
					notEnoughCurrencyText.text = "Not Enough GAI and M7";
					return false;
				}
				if (!gianiteAvailable && m7Available)
				{
					notEnoughCurrencyText.text = "Not Enough GAI";
					return false;
				}
				if (gianiteAvailable && !m7Available)
				{
					notEnoughCurrencyText.text = "Not Enough M7";
					return false;
				}
			}
			return false;
		}

		
		//public void OnClickConfirmLevelUp ()
		//{	
		//	var level = PlayerDatabase.Inventories.Characters.FindItem(hScManager.CurentInstanceId);	
		//	int l = sCharacterData[0].Level;
		//	int p = sCharacterData[0].AvailableStatsPoints;
		//	l++;
		//	p+=20;
			
		//	OnLoadHeroInfo ();
		//}
		
		//public void OnClickConfirmAddDurability (Transform e)
		//{
		//	if (CheckGianiteAndM7Cost(totalDurbilityCost) != true)
		//	{
		//		Debug.Log ("Not enough Gianite");
		//		e.gameObject.SetActive (true);	
		//	}
		//	else
		//	{
		//		var durability = PlayerDatabase.Inventories.Characters.FindItem(hScManager.CurentInstanceId);
		//		durability.OverwriteValues($"{{\"saveableStats\": " + 
		//			"{\"durability\":"+ Mathf.Floor(pDurabilitySlider.value * 100).ToString()+"}}}");
		//		OnLoadHeroInfo ();
		//		GetShowPopOutBox(false);
		//		pPopUpItem[2].gameObject.SetActive (false);
		//	}
		//}
		
		public float DurabilityCost(string rarity)
        {
			if (rarity == "Common")
			{
				return PlayerDatabase.GlobalDataSetting.RecoveryCostAllChartData.RecoveryCost.common_cost[sCharacterData[0].Level];
            }
            else if (rarity == "Uncommon")
			{
				return PlayerDatabase.GlobalDataSetting.RecoveryCostAllChartData.RecoveryCost.uncommon_cost[sCharacterData[0].Level];
			}
			else if (rarity == "Rare")
            {
				return PlayerDatabase.GlobalDataSetting.RecoveryCostAllChartData.RecoveryCost.rare_cost[sCharacterData[0].Level];
			}
			else if (rarity == "Epic")
            {
				return PlayerDatabase.GlobalDataSetting.RecoveryCostAllChartData.RecoveryCost.epic_cost[sCharacterData[0].Level];
			}
			else if (rarity == "Legendary")
            {
				return PlayerDatabase.GlobalDataSetting.RecoveryCostAllChartData.RecoveryCost.legendary_cost[sCharacterData[0].Level];
			}
				
			return 0;
		}
		
        public void OnClickConfirmAddDurability()
		{
			string[] splitString = pDurabilityCostText.text.Split(char.Parse(" "));
			float durabilityFinalCost = float.Parse(splitString[0]);

			if (durabilityFinalCost > 0)
			{ 
				RecoveryData recoverData = new RecoveryData();

				recoverData.instanceID = sCharacterData[0].InstanceID;
				recoverData.gaianiteCost = durabilityFinalCost;

				string jsonString = JsonConvert.SerializeObject(recoverData);
				sCharacterData[0].saveableStats.durability = pDurabilitySlider.value * 100;
				var gai = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite");
				gai.Amount -= recoverData.gaianiteCost;
				gai.IsDirty = true;
				//DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.SetRecovery, jsonString);
			}
        }

		public void RecoveryAddCallback(bool isSuccess)
        {
			//THIS WILL BE MOVED
			if (!isSuccess)
			{
				Debug.Log("Not enough Gianite");
				//e.gameObject.SetActive(true);
			}
			else
			{
				//var durability = PlayerDatabase.Inventories.Characters.FindItem(hScManager.CurentInstanceId);
				//durability.OverwriteValues($"{{\"saveableStats\": " +
				//    "{\"durability\":" + Mathf.Floor(pDurabilitySlider.value * 100).ToString() + "}}}");

				PopulateList();
				OnLoadHeroInfo();
				GetShowPopOutBox(false);
				pPopUpItem[2].gameObject.SetActive(false);
			}
		}

        public void OnChangeSliderDurability(Slider e)
        {
            if (sliderDefaultValue >= e.value)
            {
                e.value = sliderDefaultValue;
                //var dura = (e.value * 100) * DurabilityCost(sCharacterData[0].rarity.ToString());
                //if (sCharacterData[0].SaveableStats.Durability < sCharacterData[0].BaseStats.Durability)
                //	pDurabilityCostText.text = dura.ToString("0.0") + " GAI";
                //else
                //pDurabilityCostText.text = "0 GAI";
                //DurabilityCost(sCharacterData[0].rarity.ToString());
            }

            var sliderValue = e.value;
            float sliderVal = Mathf.RoundToInt(sliderValue * 100);
            //float a = sliderValue * 100;
            pDurabilityText.text = "Durability " + sliderVal.ToString("0") + "/100";
            // var initialDur = durTest * DurabilityCost(ActiveCharacters[0].CharacterObject.DisplayStats.Rarity.ToString());
            
            var sliderDefaultVal = Mathf.RoundToInt(sliderDefaultValue * 100);
            var total = sliderVal - sliderDefaultVal;

            var initialDur = total * DurabilityCost(ActiveCharacters[0].CharacterObject.DisplayStats.Rarity.ToString());
            pDurabilityCostText.text = initialDur + " GAI";

            //var sliderValue = e.value;
            //float a = e.value * durMaxValue;
            //float b = sliderValue * 100;
            //pDurabilityText.text = "Durability " + Mathf.Floor(b).ToString("0.0") + "/100";
            //var initialDur = Mathf.Floor(a) * DurabilityCost(ActiveCharacters[0].CharacterObject.DisplayStats.Rarity.ToString());
            //pDurabilityCostText.text = initialDur.ToString("0.0") + " GAI";
        }

        public void GetShowPopOutBox (bool show)
		{
			popOutCam.enabled = show;
			popOutTrans.gameObject.SetActive (show);
		}
	
		private void OnLoadSkillObjDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
		{
			SkillObject e = obj.Result.GetComponent<SkillObject>();
			skillNameText.text = e.DisplayStats.DisplayName;
			skillDescriptionText.text = e.DisplayStats.Description;
			skillEffectCostText.text = "x" + ActiveCharacters[0].CharacterObject.CharacterSkillConditionList[0].RequiredPoints.ToString();
			
			switch (ActiveCharacters[0].CharacterObject.Element.ElementType)
			{
				case SkillEnums.ElementFilter.Dark : 
					skillIconImg.sprite = elementImages[0];
					break;
				case SkillEnums.ElementFilter.Earth : 
					skillIconImg.sprite = elementImages[1];
					break;
				case SkillEnums.ElementFilter.Fire : 
					skillIconImg.sprite = elementImages[2];
					break;
				case SkillEnums.ElementFilter.Light : 
					skillIconImg.sprite = elementImages[3];
					break;
				case SkillEnums.ElementFilter.Water : 
					skillIconImg.sprite = elementImages[4];
					break;
				
			}
		}
		
		public void ShowAdditionalStatsProgressBar ()
		{
			baseSelected = !baseSelected;
			aattck_Progressbar.gameObject.SetActive(baseSelected);
			ablck_Progressbar.gameObject.SetActive(baseSelected);
			abpssn_Progressbar.gameObject.SetActive(baseSelected);
			abshp_Progressbar.gameObject.SetActive(baseSelected);
			baseButtonImage.color = baseSelected ? baseSelectedColor : baseUnselectedColor;
			setStringStats();
		}
		
		public void ClearAllChild (Transform parent)
		{
			foreach (Transform child in parent) {
				GameObject.Destroy(child.gameObject);
			}
		}

		public void OnRemainingTimeFinished()
        {

        }

		private void setStringStats()
		{
			if(baseSelected)
            {
				sAttckText.text = sCharacterData[0].SaveableStats.Attack.ToString();
				sLckText.text = sCharacterData[0].SaveableStats.Luck.ToString();
				sPssnText.text = sCharacterData[0].SaveableStats.Passion.ToString();
				sHpText.text = sCharacterData[0].SaveableStats.Hp.ToString();

			}
			else
			{
				sAttckText.text = sCharacterData[0].BaseStats.Attack.ToString();
				sLckText.text = sCharacterData[0].BaseStats.Luck.ToString();
				sPssnText.text = sCharacterData[0].BaseStats.Passion.ToString();
				sHpText.text = sCharacterData[0].BaseStats.Hp.ToString();
			}
		}

		private float getHighestStats(CombatStats stats)
        {
			return new float[] { stats.Attack, stats.Luck, stats.Passion, stats.Hp }.Max();
        }
	}
	public class RecoveryData
    {
		public string instanceID;
		public float gaianiteCost;
    }
}
