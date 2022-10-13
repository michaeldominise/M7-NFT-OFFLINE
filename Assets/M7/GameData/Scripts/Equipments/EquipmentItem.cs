using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using M7.Skill;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "EquipmentItem", menuName = "Assets/M7/GameData/EquipmentItem")]
    public class EquipmentItem : BaseAssetObject
    {
	    public enum Type
	    {
		    Accessory,
		    Armor,
		    Gloves,
		    Shoes,
		    Headgear,
		    Weapon,
	    }
	    [SerializeField] string displayName;
	    [SerializeField] RPGElement element;
	    [SerializeField] AssetReferenceT<Sprite> iconSprite;
	    [SerializeField] AssetReferenceT<GameObject> skillObject;
	    [SerializeField] List<EquipmentItem_BodyPartData> bodyParts;
	    [SerializeField] CombatStats additionalStats;
	    [SerializeField] Type equipmentType;
	    [SerializeField] bool isSubSpine;

		public string DisplayName => displayName;
	    public AssetReferenceT<Sprite> IconSprite => iconSprite;
	    public AssetReferenceT<GameObject> SkillObject => skillObject;
	    public RPGElement Element => element;
	    public List<EquipmentItem_BodyPartData> BodyParts => bodyParts;
	    public CombatStats AdditionalStats => additionalStats;
	    public Type EquipmentType => equipmentType;
		public bool IsSubSpine => isSubSpine;
	}
}
