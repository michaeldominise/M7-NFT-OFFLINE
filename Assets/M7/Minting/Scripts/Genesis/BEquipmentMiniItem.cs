using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace M7.GameRuntime
{	
	public class BEquipmentMiniItem : MonoBehaviour
	{
	
		public GameData.Equipments.EquipmentType type = new GameData.Equipments.EquipmentType();
		private BCharacterMiniRandom e;

		public TMP_Text name;
		public TMP_Text description;
		[Space (10)]
		public int slctId;
		
		public void Start ()
		{
			e = BCharacterMiniRandom.Instance; 
		}
		
		public void IncSlctEquipment ()
		{
			if (this.slctId < e.equipments.Count - 1)
				this.slctId++;
			e.SelectEquipments(this.type, slctId);
		}
		
		public void DecSlctEquipment ()
		{
			if (slctId > 0)
				this.slctId--;
			e.SelectEquipments(this.type, slctId);
		}
	}
}
