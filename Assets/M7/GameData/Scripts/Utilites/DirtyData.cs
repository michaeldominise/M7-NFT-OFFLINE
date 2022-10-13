using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using M7.CDN.Addressable;
using Newtonsoft.Json;

namespace M7.GameData
{
    [System.Serializable]
    public class DirtyData
    {
        static List<DirtyData> DirtyList { get; set; } = new List<DirtyData>();

        [JsonIgnore] public System.Action onValuesChanged { get; set; }
        [JsonIgnore] bool isDirty;

        [JsonIgnore, ShowInInspector]
        public bool IsDirty
        {
            get
            {
                if (!Application.isPlaying)
                    isDirty = false;
                return isDirty;
            }
            set
            {
                if (!Application.isPlaying)
                    isDirty = false;
                else if (isDirty != value)
                {
                    if (value && !DirtyList.Contains(this))
                        DirtyList.Add(this);
                    else if (DirtyList.Contains(this))
                        DirtyList.Remove(this);
                    isDirty = value;
                }
            }
        }

        public static void CleanDirtyList()
        {
            while (DirtyList.Count > 0)
            {
                var dirtyData = DirtyList[0];
                DirtyList.RemoveAt(0);
                dirtyData.isDirty = false;
                dirtyData.onValuesChanged?.Invoke();
            }
        }
    }
}