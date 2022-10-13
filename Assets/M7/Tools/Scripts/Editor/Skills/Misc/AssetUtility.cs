using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace M7.Tools.Utility
{
    public class AssetUtility
    {
        /// <summary>
        /// Gets all assets of <typeparamref name="T"/> in the <paramref name="searchPath"/> with <paramref name="searchFilter"/>
        /// This is just to make the code shorter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchFilter"></param>
        /// <param name="searchPath"></param>
        /// <returns></returns>
        public static List<T> GetAssets<T>(string searchFilter, string[] searchPath) where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(searchFilter, searchPath);
            foreach (string guid in guids)
            {
                string paths = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(paths);
                assets.Add(asset);
            }

            return assets;
        }

        /// <summary>
        /// Delete asset in the asset database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        public static void DeleteAsset<T>(T go) where T : Object
        {
            string assetPath = AssetDatabase.GetAssetPath(go);
            AssetDatabase.DeleteAsset(assetPath);
        }

        /// <summary>
        /// Get asset path of <paramref name="obj"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetAssetPath<T>(T obj) where T : Object
        {
            return AssetDatabase.GetAssetPath(obj);
        }

        public static List<T> GetAssets<T>(string path, string searchFilter, SearchOption option = SearchOption.TopDirectoryOnly) where T : Object
        {
            List<T> assets = new List<T>();
            string[] files = Directory.GetFiles(path, searchFilter, option);
            foreach (var file in files)
            {
                var asset = (T)AssetDatabase.LoadAssetAtPath(file, typeof(T));
                assets.Add(asset);
            }

            return assets;
        }

        /// <summary>
        /// Creates an asset with <paramref name="path"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="assets"></param>
        /// <param name="taskName"></param>
        public static void CreateAssets<T>(string path, List<T> assets, string taskName = "Create Assets") where T : Object
        {
            EditorUtility.DisplayProgressBar(taskName, $"Creating assets...[0/{assets.Count}]", 0f);
            for(int i = 0; i < assets.Count; i++)
            {
                try
                {
                    CreateAsset(path, assets[i]);
                    EditorUtility.DisplayProgressBar(taskName, $"Creating assets...[{i}/{assets.Count}]", (float)i / assets.Count);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"{e.Message}");
                }
            }

            EditorUtility.DisplayProgressBar(taskName, "Saving assets...", 0f);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
        }

        public static void CreateAsset<T>(string path, T asset, bool save = false, bool refresh = false) where T : Object
        {
            string assetPath = Path.Combine(path, asset.name + ".asset");
            AssetDatabase.CreateAsset(asset, assetPath);

            if (save)
            {
                AssetDatabase.SaveAssets();
            }

            if (refresh)
            {
                AssetDatabase.Refresh();
            }
        }
    }
}