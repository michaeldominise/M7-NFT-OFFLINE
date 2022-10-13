using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
	[System.Serializable]
	public class Attribute
	{
		public string trait_type;
		public string value;
	}
	
	//[CreateAssetMenu(fileName = "MinteDatabase", menuName = "Assets/M7/GameData/MinterDatabase")]
	public class BMinterMetaData : ScriptableObject
	{
		public string name;
		public string description;
		public string image;
		public string external_url;
		
		[Space (10)]
		public List <Attribute> attributes = new List<Attribute> ();
	}
}
