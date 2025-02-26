using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    [System.Serializable]
    public class WaveData_Player : WaveData
    {
        [JsonProperty][SerializeField] List<string> instanceIDList;
        [JsonIgnore] public override List<SaveableCharacterData> SaveableCharacters => instanceIDList.Select(x => PlayerDatabase.Inventories.Characters.FindItem(x)).ToList();

        public override void SetSaveableCharacterAtIndex(int index, SaveableCharacterData saveableCharacterData)
        {
            instanceIDList[index] = saveableCharacterData?.InstanceID ?? "";
            base.SetSaveableCharacterAtIndex(index, saveableCharacterData);
        }
    }
}
