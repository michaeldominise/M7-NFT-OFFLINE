using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BGeneratorHeroData 
{	
	[System.Serializable]
	public class Creator
	{
		public string address;
		public int share;
	}
	
	[System.Serializable]
	public class Properties
	{
		public List<Creator> creators = new List<Creator>();
	}
	
	[System.Serializable]
	public class Attribute
	{
		public string trait_type;
		public string value;
		public string id;
	}
	
	public string name;
	public string itemId;
	public string description;
	public string image;
	public string external_url;
	public string type;
	public string origin;
	public Properties properties = new Properties();
	public List<Attribute> attributes = new List<Attribute>();
}
