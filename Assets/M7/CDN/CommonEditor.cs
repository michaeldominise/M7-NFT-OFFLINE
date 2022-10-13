
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public static partial class CommonEditor
{
    [MenuItem("CDN/Delete All PlayerPrefs")]
    public static void DeleteAllPlayerPrefs()
    {
        if(EditorUtility.DisplayDialog("Delete All PlayerPrefs", "Are you sure you want to delete all player prefs?", "Yeah Baby!", "Nope Nevermind!"))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    [MenuItem("CDN/Clear Cache")]
    public static void ClearCache()
    {
        if (Caching.ClearCache())
        {
            Debug.Log("Cleared Cache!");
        }
    }
}

#endif
