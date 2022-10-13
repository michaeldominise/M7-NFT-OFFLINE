using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameData
{
    [System.Serializable]
    public class SaveableCharacterData_Enemy : SaveableCharacterData
    {
        [JsonProperty] [SerializeField] int attackTurn = 3;

        [JsonIgnore] public int AttackTurn => Mathf.Max(attackTurn, 1);

        public SaveableCharacterData_Enemy(string masterID, string instanceID, int level, string[] equipments) : base(masterID, instanceID, level, equipments) { }

#if UNITY_EDITOR
        [JsonIgnore]
        [ShowInInspector, PropertyOrder(-1)] public CharacterObject characterObject
        {
            get => MasterIDManager.RPGObjectReference.FindAssetReference(masterID)?.Asset as CharacterObject;
            set => masterID = value?.MasterID;
        }
#endif
    }
}