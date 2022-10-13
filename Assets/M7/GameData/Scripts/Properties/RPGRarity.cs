using System.Collections;
using System.Collections.Generic;
using M7.Skill;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "RPGElement", menuName = "Assets/M7/GameData/RPGElement")]
    public class RPGRarity : BaseAssetObject
	{
        [SerializeField, FormerlySerializedAs("element")] SkillEnums.ElementFilter elementType;
		[SerializeField] Color elementColor;
		[SerializeField] Sprite displaySprite;
		
		public SkillEnums.ElementFilter ElementType => elementType;
		public Color ElementColor => elementColor;
		public Sprite DisplaySprite => displaySprite;
	}
}