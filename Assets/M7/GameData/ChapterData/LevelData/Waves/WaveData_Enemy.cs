using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    [System.Serializable]
    public class WaveData_Enemy : WaveData
    {
        [JsonProperty][SerializeField] List<SaveableCharacterData_Enemy> saveableCharacters = new List<SaveableCharacterData_Enemy>();
        [JsonIgnore] public override List<SaveableCharacterData> SaveableCharacters => saveableCharacters.Select(x => x as SaveableCharacterData).ToList();

        public override void SetSaveableCharacterAtIndex(int index, SaveableCharacterData saveableCharacterData)
        {
            saveableCharacters[index] = saveableCharacterData as SaveableCharacterData_Enemy;
            base.SetSaveableCharacterAtIndex(index, saveableCharacterData);
        }
    }
}
