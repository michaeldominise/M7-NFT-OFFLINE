using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix;
using Sirenix.OdinInspector;
using System;


[System.Serializable]
public class EquipmentList
{
	public string heroId;
	public List<string> equipments = new List<string>();
}

[CreateAssetMenu(fileName = "MinteDatabase", menuName = "Assets/M7/GameData/MinterDatabase")]
public class BEquipmentData : ScriptableObject
{
	public List<EquipmentList> equipmentList = new List<EquipmentList>();
}
