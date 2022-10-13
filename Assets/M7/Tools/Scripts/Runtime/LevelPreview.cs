using M7.GameData;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.Tools
{
    public class LevelPreview : MonoBehaviour
    {
        [ReadOnly]
        public bool isInPreview;
        [ReadOnly]
        public LevelData previewLevelData;

        private void Awake()
        {
            Instance = this;
        }

        public static LevelPreview Instance;
    }
}