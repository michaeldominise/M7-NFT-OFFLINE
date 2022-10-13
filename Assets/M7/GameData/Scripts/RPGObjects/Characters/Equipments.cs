using M7.CDN.Addressable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    [System.Serializable]
    public class Equipments
    {
		[System.Flags] 
		public enum EquipmentType 
        { 
            None,
            Headgear = 1 << 0,
            Armor = 1 << 1,
            Gloves = 1 << 2,
            Weapon = 1 << 3,
            Accessory = 1 << 4,
            Shoes = 1 << 5,
            All = (1 << 6) - 1
        }

	    public AssetReferenceDataArray<EquipmentItem> headgears;
    	public AssetReferenceDataArray<EquipmentItem> armors;
	    public AssetReferenceDataArray<EquipmentItem> gloves;
	    public AssetReferenceDataArray<EquipmentItem> weapon;
	    public AssetReferenceDataArray<EquipmentItem> accessories;
	    public AssetReferenceDataArray<EquipmentItem> shoes;

	    public AssetReferenceData<EquipmentItem>[] GetEquipements(params string[] masterIDs)
	    {
		    var list = new List<AssetReferenceData<EquipmentItem>>();
		    list.AddRange(headgears.FindAssetReferences(masterIDs));
		    list.AddRange(armors.FindAssetReferences(masterIDs));
		    list.AddRange(gloves.FindAssetReferences(masterIDs));
		    list.AddRange(weapon.FindAssetReferences(masterIDs));
		    list.AddRange(accessories.FindAssetReferences(masterIDs));
		    list.AddRange(shoes.FindAssetReferences(masterIDs));
		    return list.Where(x => x != null).ToArray();
	    }

		 public AssetReferenceData<EquipmentItem>[] GetRandomEquipements(EquipmentType equipmentType)
        {
            var list = new List<AssetReferenceData<EquipmentItem>>();
            if((equipmentType & EquipmentType.Headgear) != 0)
                AddRandomEquipement(list, headgears);
            if((equipmentType & EquipmentType.Armor) != 0)
                AddRandomEquipement(list, armors);
            if ((equipmentType & EquipmentType.Gloves) != 0)
                AddRandomEquipement(list, gloves);
            if ((equipmentType & EquipmentType.Weapon) != 0)
                AddRandomEquipement(list, weapon);
            if ((equipmentType & EquipmentType.Accessory) != 0)
                AddRandomEquipement(list, accessories);
            if ((equipmentType & EquipmentType.Shoes) != 0)
                AddRandomEquipement(list, shoes);
            return list.ToArray();
        }

        void AddRandomEquipement(List<AssetReferenceData<EquipmentItem>> list, AssetReferenceDataArray<EquipmentItem> dataArray)
        {
            if (dataArray.Length == 0)
                return;

            for (int i = 0; i < dataArray.Length; i++){
                list.Add(dataArray[i]);
            }
            //list.Add(dataArray[Random.Range(0, dataArray.Length)]);
        }
    }
}