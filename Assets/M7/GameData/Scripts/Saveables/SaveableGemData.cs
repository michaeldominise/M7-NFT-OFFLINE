using System;

namespace M7.GameData
{
    [Serializable]
    public class SaveableGemData : BaseSaveableData<GemObject>
    {
        public SaveableGemData() : base() { }
        public SaveableGemData(string masterID, string instanceID) : base(masterID, instanceID) { }
    }
}