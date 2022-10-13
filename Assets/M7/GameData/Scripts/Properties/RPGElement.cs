using System.Collections;
using System.Collections.Generic;
using M7.Skill;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "RPGElement", menuName = "Assets/M7/GameData/RPGElement")]
    public class RPGElement : BaseAssetObject
	{
		[SerializeField] SkillEnums.ElementFilter elementType;
		[SerializeField] Color elementColor;
		[SerializeField] Sprite displaySprite;
		[SerializeField] Sprite displayGemSprite;
		[SerializeField] GameObject vfxElement;

		 public SkillEnums.ElementFilter ElementType => elementType;
		 public Color ElementColor => elementColor;
		 public Sprite DisplaySprite => displaySprite;
		 public Sprite DisplayGemSprite => displayGemSprite;
		 public GameObject VfxElement => vfxElement;
	}
}