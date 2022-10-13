using M7.Skill;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameData
{
    [System.Serializable]
    public class SaveableIncubatorData : BaseSaveableData<IncubatorObject>
    {
        public SaveableIncubatorData() : base() { }
        public SaveableIncubatorData(string masterID, string instanceID) : base(masterID, instanceID) { }
    }
}