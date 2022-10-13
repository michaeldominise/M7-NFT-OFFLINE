using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;
using M7.CDN.Addressable;
using M7.GameData.CharacterSkill;
using M7.Skill;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "CharacterObject", menuName = "Assets/M7/GameData/RpgObject/CharacterObject")]
    public class CharacterObject : RPGObject, IRPGElement
    {
#if UNITY_EDITOR
        public static CharacterObject ActiveCharacterObject { get { return (CharacterObject)Selection.activeObject; } }
#endif
        [JsonProperty] [SerializeField] RPGElement element;
        [JsonProperty] [SerializeField] CombatStats_WithModifier combatStats;
        [JsonProperty] [SerializeField] CharacterDisplayStats displayStats;
	    [JsonProperty] [SerializeField] Equipments equipments;
	    [JsonProperty] [SerializeField] float meleeXOffset;
        [JsonProperty] [SerializeField] float rangeXOffset;

        [JsonProperty] [SerializeField] AssetReferenceT<GameObject> basicAttackSkillObject;
        [JsonProperty] [SerializeField, FormerlySerializedAs("overdrive")] AssetReferenceT<GameObject> characterSkillObject;
        [JsonProperty] [SerializeField] private CharacterSkillCondition[] characterSkillConditionList;
		
        [JsonIgnore] public RPGElement Element => element;
        [JsonIgnore] public CombatStats_WithModifier CombatStats => combatStats;
        [JsonIgnore] public CharacterDisplayStats DisplayStats => displayStats;
        [JsonIgnore] public Equipments Equipments => equipments;
        [JsonIgnore] public float MeleeXOffset => meleeXOffset;
        [JsonIgnore] public float RangeXOffset => rangeXOffset;

        [JsonIgnore] public AssetReferenceT<GameObject> CharacterSkillObject => characterSkillObject;
        [JsonIgnore] public AssetReferenceT<GameObject> BasicAttackSkillObject => basicAttackSkillObject;
        [JsonIgnore] public CharacterSkillCondition[] CharacterSkillConditionList => characterSkillConditionList;
    }
}
