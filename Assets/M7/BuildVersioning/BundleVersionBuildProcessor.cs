#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BundleVersionBuildProcessor : IPreprocessBuildWithReport
{
    const string TextFilePath = "Assets/M7/BuildVersioning/BundleVersionBuildText.txt";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (!Directory.Exists(TextFilePath))
            Directory.CreateDirectory(Path.GetDirectoryName(TextFilePath));
#if UNITY_ANDROID
        File.WriteAllText(TextFilePath, PlayerSettings.Android.bundleVersionCode.ToString());
#elif UNITY_IOS
        File.WriteAllText(TextFilePath, PlayerSettings.iOS.buildNumber.ToString());
#endif             
    }
}
#endif