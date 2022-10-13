using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace M7
{
    [CreateAssetMenu(fileName = "HelperTextData", menuName = "Assets/M7/SceneTransition/HelperTextData")]
    public class HelperTextData : ScriptableObject
    {
        public string[] Tips;

#if UNITY_EDITOR
        public TextAsset loadingScreenTipsList;

        [Button]
        public void Populate()
        {
            if (loadingScreenTipsList != null)
            {
                string[] tipsList = loadingScreenTipsList.text.Split('\n');
                Tips = tipsList;
            }
            else
            {
                Debug.LogError("TextAsset is null");
            }
        }
#endif
    }
}
