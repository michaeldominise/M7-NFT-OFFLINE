using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.CDN;
using System.Linq;
using M7.CDN.Addressable;
using M7;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace M7
{
    [CreateAssetMenu(menuName = "Assets/M7/SceneTransition/SceneTransitionAddressableSettings")]
    public class SceneTransitionAddressableSettings : ScriptableObject
    {
        public AssetReferenceDataArray<Object> sceneList;
    }
}
