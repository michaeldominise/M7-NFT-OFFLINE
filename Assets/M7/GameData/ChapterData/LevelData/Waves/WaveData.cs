using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    public abstract class WaveData : DirtyData
    {
        public abstract List<SaveableCharacterData> SaveableCharacters { get; }
        public virtual void SetSaveableCharacterAtIndex(int index, SaveableCharacterData saveableCharacterData) => IsDirty = true;
    }
}
