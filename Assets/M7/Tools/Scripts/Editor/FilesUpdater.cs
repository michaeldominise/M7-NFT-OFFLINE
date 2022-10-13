using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.OdinInspector;
using System.IO;

public class FilesUpdater : OdinEditorWindow
{
    [SerializeField, TableList] UpdaterData[] importDetails;
    [SerializeField, TableList] SearchOption searchOption = SearchOption.AllDirectories;

    [MenuItem("Tools/FilesUpdater")]
    private static void OpenWindow()
    {
        GetWindow<FilesUpdater>().Show();
    }

    [Button]
    void UpdateFiles()
    {
        foreach (var importDetail in importDetails)
        {
            if (!Directory.Exists(importDetail.FromDirectory))
                continue;

            var filePaths = Directory.GetFiles(importDetail.FromDirectory, "*.*", searchOption);
            foreach (var filePath in filePaths)
            {
                var newFilePath = filePath.Replace(importDetail.FromDirectory, importDetail.DestinationDirectory);
                var newDirPath = Path.GetDirectoryName(newFilePath);
                CreateFolder(newDirPath);
                File.Copy(filePath, newFilePath, true);

                var newAsset = AssetDatabase.LoadAssetAtPath<Object>(newFilePath);
                if(newAsset)
                    EditorUtility.SetDirty(newAsset);
            }
        }
        AssetDatabase.SaveAssets();
    }

    void CreateFolder(string dirPath)
    {
        var parentDir = Path.GetDirectoryName(dirPath);
        if (!AssetDatabase.IsValidFolder(parentDir))
            CreateFolder(parentDir);

        if (!AssetDatabase.IsValidFolder(dirPath))
        {
            AssetDatabase.CreateFolder(parentDir, Path.GetFileName(dirPath));
            AssetDatabase.SaveAssets();
        }
    }

    [System.Serializable]
    public class UpdaterData
    {
        [SerializeField, HideInInspector] string fromDirectory;
        [SerializeField, HideInInspector] string destinationDirectory;

        [ShowInInspector] public string FromDirectory { get => fromDirectory; set => fromDirectory = value.Replace("\"", ""); }
        [ShowInInspector] public string DestinationDirectory { get => destinationDirectory; set => destinationDirectory = value.Replace("\"", ""); }
    }
}
