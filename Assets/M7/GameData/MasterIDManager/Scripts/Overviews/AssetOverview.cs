using M7.CDN.Addressable;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace M7.GameData
{
    public class AssetOverview<T> : IUpdateOverview, IUpdateLocalizationKeys where T : Object
    {
        [ReadOnly, SerializeField] T[] _AllData = new T[0];
        public T[] AllData => _AllData;

        public virtual void UpdateOverview()
        {
#if UNITY_EDITOR
            _AllData = AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
#endif
        }

        public virtual void UpdateLocalizationKeys() { }
    }

    public interface IUpdateOverview
    {
        public void UpdateOverview();
    }

    public interface IUpdateLocalizationKeys
    {
        public void UpdateLocalizationKeys();
    }
}
