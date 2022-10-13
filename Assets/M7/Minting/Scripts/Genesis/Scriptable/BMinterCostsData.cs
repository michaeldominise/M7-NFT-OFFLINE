using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;
using System;

public enum Rarity
{
	Common,
	UnCommon,
	Rare,
	Epic,
	Legendary,
}

[Serializable]
public class MintPercentage
{
	public Rarity rarity;
	public float chances;
}

[Serializable]
public class RequireItem
{
	public Rarity rarity;
	public int quantiy = 1;
}

public class BMinterCostsData : ScriptableObject
{

	[Serializable]
	public class ItemMint 
	{
		public int id;
		
		[Header ("Required Item List")]
		public List<RequireItem> requiredItems = new List<RequireItem>();
		
		[Header ("Chances based on (%)")]
		public List <MintPercentage> mintPercentages = new List<MintPercentage>();
	}
	
	public List <ItemMint> itemMints = new List<ItemMint> ();
	
}
