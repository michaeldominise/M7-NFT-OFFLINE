using UnityEngine;
using TMPro;
using UnityEngine.UI;
using M7.GameData;
using M7.GameRuntime;
using M7;
using Newtonsoft.Json;
using M7.ServerTestScripts;

public class AdditionalStatsSceneManager : SceneManagerBase
{
	public static AdditionalStatsSceneManager Instance;

	[SerializeField] TextMeshProUGUI availablePointsText;
	[SerializeField] TextMeshProUGUI attckText;
	[SerializeField] TextMeshProUGUI lckText;
	[SerializeField] TextMeshProUGUI pssnText;
	[SerializeField] TextMeshProUGUI hpText;

	[SerializeField] Button[] minusBtns;
	
	[SerializeField] Color hcolor;
	
	CharacterInfoManager chInfoManager;
	
	int points, attck, lck, hp, pssn;
	int attckAdded, lckAdded, hpAdded, pssnAdded;

	protected override void Awake()
	{
		Instance = this;
		base.Awake();
	}

	private void Start ()
	{
		LoadStatsPoints ();

		attckAdded = attck;
		lckAdded = lck;
		hpAdded = hp;
		pssnAdded = pssn;

		for (int i = 0; i < minusBtns.Length; i++)
		{
			minusBtns[i].targetGraphic.color = Color.gray;
		}
	}
	
	public void LoadStatsPoints ()
	{
		chInfoManager = FindObjectOfType<CharacterInfoManager>();
		points = chInfoManager.sCharacterData[0].AvailableStatsPoints;
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";
		attck = (int)chInfoManager.sCharacterData[0].SaveableStats.Attack;
		lck = (int)chInfoManager.sCharacterData[0].SaveableStats.Luck;
		pssn = (int)chInfoManager.sCharacterData[0].SaveableStats.Passion;
		hp = (int)chInfoManager.sCharacterData[0].SaveableStats.Hp;

		attckText.text = attck.ToString();
		lckText.text = lck.ToString();
		pssnText.text = pssn.ToString();
		hpText.text = hp.ToString();
	}

	public void OnDecreasedPssn (Image e)
	{
		
        if (chInfoManager.sCharacterData[0].SaveableStats.Passion > pssn)
        {
            points++;

			AvailableStats("passion", false);
        }
		if (points == chInfoManager.sCharacterData[0].AvailableStatsPoints || pssnAdded == pssn)
		{
			e.color = Color.gray;
		}
		
		pssnText.text = chInfoManager.sCharacterData[0].SaveableStats.Passion.ToString();
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";;
	}
	
	public void OnIncreasePssn (Image e)
	{
		if (points != 0){
			e.color = hcolor;
		}
		
		if (points != 0)
		{
			points--;

			AvailableStats("passion", true);
		}

		pssnText.text = chInfoManager.sCharacterData[0].SaveableStats.Passion.ToString();
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";;
	}
	
	/// <summary>
	/// LCK
	/// </summary>
	/// <param name="e"></param>
	public void OnDecreasedLck (Image e)
	{
			if (chInfoManager.sCharacterData[0].SaveableStats.Luck > lck)
			{
				points++;

				AvailableStats("luck", false);
			}
		
		if (points == chInfoManager.sCharacterData[0].AvailableStatsPoints || lckAdded == lck)
		{
			e.color = Color.gray;
		}
		lckText.text = chInfoManager.sCharacterData[0].SaveableStats.Luck.ToString();
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";;
	}
	
	public void OnIncreaseLck (Image e)
	{
		if (points != 0){
			e.color = hcolor;
		}
		
		if (points != 0)
		{
			points--;
			AvailableStats("luck", true);
		}

		lckText.text = chInfoManager.sCharacterData[0].SaveableStats.Luck.ToString();
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";;
	}

	/// <summary>
	/// ATTACK
	/// </summary>
	/// <param name="e"></param>
	public void OnDecreasedAttck (Image e)
	{
			if (chInfoManager.sCharacterData[0].SaveableStats.Attack > attck)
			{
				points++;
				AvailableStats("attack", false);
			}
		
		if (points == chInfoManager.sCharacterData[0].AvailableStatsPoints || attckAdded == attck)
		{
			e.color = Color.gray;
		}
		attckText.text = chInfoManager.sCharacterData[0].SaveableStats.Attack.ToString();
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";;
	}
	
	public void OnIncreaseAttck (Image e)
	{
		if (points != 0){
			e.color = hcolor;
		}
		
		if (points != 0)
		{
			points--;
			AvailableStats("attack", true);
		}
	
		attckText.text = chInfoManager.sCharacterData[0].SaveableStats.Attack.ToString();
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";;
	}
	/// <summary>
	/// HP
	/// </summary>
	/// <param name="e"></param>
	public void OnDecreasedHp (Image e)
	{
			if (chInfoManager.sCharacterData[0].SaveableStats.Hp > hp)
			{
				points++;
				AvailableStats("hp", false);
			}
		if (points == chInfoManager.sCharacterData[0].AvailableStatsPoints || hpAdded == hp)
		{
			e.color = Color.gray;
		}
		hpText.text = chInfoManager.sCharacterData[0].SaveableStats.Hp.ToString();
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";;
	}
	
	public void OnIncreasedHp (Image e)
	{
		if (points != 0){
			e.color = hcolor;
		}
		
		if (points != 0)
		{
			points--;
			AvailableStats("hp", true);
		}
	
		hpText.text = chInfoManager.sCharacterData[0].SaveableStats.Hp.ToString();
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";;
	}
	
	public void OnClickBack ()
	{
		OnClickCancel ();
		UnloadAtSceneLayer(sceneLayer);
	}
	
	public void OnClickCancel ()
	{
		attckText.text = attck.ToString();
		lckText.text = lck.ToString();
		pssnText.text = pssn.ToString();
		hpText.text = hp.ToString();

		points = chInfoManager.sCharacterData[0].AvailableStatsPoints;
		availablePointsText.text = "<size=80%>Available Points:</size>  <color=#A75ED6>" + points.ToString() + "</color>";

		attckAdded = attck;
		lckAdded = lck;
		pssnAdded = pssn;
		hpAdded = hp;

		var statsUpdate = PlayerDatabase.Inventories.Characters.FindItem(chInfoManager.sCharacterData[0].InstanceID);
		statsUpdate.OverwriteValues($"{{\"availableStatsPoints\" : " + points + "}}");
		statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"attack\":" + attck.ToString() + "}}}");
		statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"hp\":" + hp.ToString() + "}}}");
		statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"luck\":" + lck.ToString() + "}}}");
		statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"passion\":" + pssn.ToString() + "}}}");
	
		
		for (int i = 0; i < minusBtns.Length; i++)
		{
			minusBtns[i].targetGraphic.color = Color.gray;
		}
	}
	
	public void OnClickConfirm ()
	{
		var jsonString = JsonConvert.SerializeObject(chInfoManager.sCharacterData[0]);
		var statsUpdate = PlayerDatabase.Inventories.Characters.FindItem(chInfoManager.sCharacterData[0].InstanceID);
		statsUpdate.OverwriteValues($"{{\"availableStatsPoints\" : " + points + "}}");
		DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.StatsData, jsonString);
	}
	void AvailableStats(string value, bool inc)
	{
		var statsUpdate = PlayerDatabase.Inventories.Characters.FindItem(chInfoManager.sCharacterData[0].InstanceID);
		switch (value)
        {
			case "attack":

				if (inc)
				{
					attckAdded++;

					statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"attack\":" + attckAdded.ToString() + "}}}");
				}
				else
				{
					attckAdded--;

					statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"attack\":" + attckAdded.ToString() + "}}}");
				}

				break;
			case "hp":

				if (inc)
				{
					hpAdded++;

					statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"hp\":" + hpAdded.ToString() + "}}}");
				}
				else
				{
					hpAdded--;

					statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"hp\":" + hpAdded.ToString() + "}}}");
				}

				break;
			case "passion":

				if (inc)
				{
					pssnAdded++;

					statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"passion\":" + pssnAdded.ToString() + "}}}");
				}
                else
                {
					pssnAdded--;

					statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"passion\":" + pssnAdded.ToString() + "}}}");
				}

				break;
			case "luck":

				if (inc)
				{
					lckAdded++;

					statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"luck\":" + lckAdded.ToString() + "}}}");
				}
				else
				{
					lckAdded--;

					statsUpdate.OverwriteValues($"{{\"saveableStats\": " +
						"{\"luck\":" + lckAdded.ToString() + "}}}");
				}
				break;
		}

	}
	public void SetData()
    {
		chInfoManager.OnLoadHeroInfo();
		UnloadAtSceneLayer(sceneLayer);
	}
	
}
