using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "GemObject", menuName = "Assets/M7/GameData/RpgObject/GemObject")]
    public class GemObject : RPGObject, IRPGElement
    {
#if UNITY_EDITOR
        public static GemObject ActiveGemObject { get { return (GemObject)Selection.activeObject; } }
#endif
        [SerializeField] RPGElement element;
        [SerializeField] RpgObjectDisplayStats displayStats;

        public RPGElement Element => element;
        public RpgObjectDisplayStats DisplayStats => displayStats;

    }
}
