using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeightedRandomization;

[Serializable]
public class MintCollection 
{
	public Rarity rarity;
	public List <Sprite> items = new List<Sprite> ();
}

public class BMinterManager : MonoBehaviour
{
		/*
	public List<BMinterSlots> bMinterSlots = new List<BMinterSlots> ();
	[Space(10)]
	public BMinterCostsData minterCostData;
	[Space(15)]
	public TMP_Text resultLog;
	public Image cardResult;
	public GameObject bttnGenerate;
	
	[Space (20)]
	public List <MintCollection> mintCollections = new	List<MintCollection>();
	
	private List <RequireItem> refItemReq = new List<RequireItem>();
	
	public void OnGenerateMint () 
	{
		if (bMinterSlots.Any(c=> c.targetLean.Data.Target == null)) 
		{
			resultLog.text = "Can't Proceed Minting!";
			return;
		}
		refItemReq.Clear();

		for (int i = 0; i < bMinterSlots.Count; i++)
		{
			resultLog.text = "Success Minting!";
			RequireItem rI = bMinterSlots[i].targetLean.Data.Target.GetComponent<BMinterItem>().requireItem;
			refItemReq.Add (rI);
			Destroy(bMinterSlots[i].targetLean.Data.Target.gameObject);
		}
		ProcessMintResult ();
	}
	
	public void ProcessMintResult () 
	{
		for (int i = 0; i < minterCostData.itemMints.Count; i++) 
		{
			bool IsMatch = IsListMatch(minterCostData.itemMints[i].requiredItems, refItemReq);
			if (IsMatch == true) 
			{
				WeightedRandomizer <float> randFloat = new	WeightedRandomizer<float>();
				for (int e = 0; e < minterCostData.itemMints[i].mintPercentages.Count; e++) 
				{
					float percentage = minterCostData.itemMints[i].mintPercentages[e].chances / 100;
					randFloat.AddOrUpdateWeight(e, percentage);
				}
				Debug.Log ("Item ID : " + minterCostData.itemMints[i].id + " | Result ID : " + randFloat.GetNext());
				int mId = minterCostData.itemMints[i].id;
				int f = (int)randFloat.GetNext();
				int maxRarityCollection = mintCollections[f].items.Count - 1;
				int rand = UnityEngine.Random.Range(0, maxRarityCollection);				
				
				
				
				Image cloneCard = Instantiate(cardResult, cardResult.transform.position, cardResult.transform.rotation);
				cloneCard.transform.SetParent (cardResult.transform.parent);
				cloneCard.sprite = mintCollections[f].items[rand];
				cloneCard.color = Color.white;
				cloneCard.name = "Card - Rare " +  cloneCard.GetInstanceID().ToString();
				
				cloneCard.gameObject.GetComponent<BMinterItem>().requireItem.rarity = minterCostData.itemMints[mId].mintPercentages[f].rarity;
				cloneCard.rectTransform.localScale = new Vector3 (1,1,1);
			}
		}
	}
	
	public bool IsListMatch(List<RequireItem> sourReqItm, List<RequireItem> refReqItm) {

		if(sourReqItm == null && refReqItm == null)
		{
			return true;
		}
		else if(sourReqItm == null || refReqItm == null)
		{
			return false;
		}
 
		if (sourReqItm.Count != refReqItm.Count)
			return false;
		for (int i = 0; i < refReqItm.Count; i++) {
			if (sourReqItm[i].rarity != refReqItm[i].rarity)
				return false;
		}
		return true;
	}
		*/
}
