using System;
using UnityEditor;

# if UNITY_EDITOR
namespace M7.Tools
{
    [InitializeOnLoad]
    public class LevelPreviewPlayMode
    {
        static LevelPreviewPlayMode()
        {
            EditorApplication.playModeStateChanged += PlaymodeStateChanged;
        }

        private static void PlaymodeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                var levelPreview = UnityEngine.Object.FindObjectOfType<LevelPreview>();
                if (levelPreview != null)
                {
                    levelPreview.isInPreview = false;
                }
            }
        }
    }
}
# endif