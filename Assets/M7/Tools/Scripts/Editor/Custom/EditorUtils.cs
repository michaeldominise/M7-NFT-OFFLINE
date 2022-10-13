using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace M7.Tools
{
    public static class EditorUtils
    {
        public static void ShowObjectPicker<T>(this T initialValue, Action<T> OnSelectorClosed, Action<T> OnSelectionChanged) where T : UnityEngine.Object
        {
            ShowObjectPicker<T>(OnSelectorClosed, OnSelectionChanged, initialValue);
        }

        /// <summary>
        /// Shows an object picker from editor assembly
        /// The object picker is internal only in Unity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OnSelectorClosed"></param>
        /// <param name="OnSelectionChanged"></param>
        /// <param name="initialValueOrNull"></param>
        public static void ShowObjectPicker<T>(Action<T> OnSelectorClosed, Action<T> OnSelectionChanged, T initialValueOrNull = null) where T : UnityEngine.Object
        {
            var hiddenType = typeof(Editor).Assembly.GetType("UnityEditor.ObjectSelector");
            var ps = hiddenType.GetProperties(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
            PropertyInfo piGet = hiddenType.GetProperty("get", BindingFlags.Public | BindingFlags.Static);
            var os = piGet.GetValue(null);

            MethodInfo miShow = hiddenType.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[]
            {
            typeof(T),
            typeof(    System.Type),
            typeof(UnityEngine.Object),
            typeof(bool),
            typeof(List<int>),
            typeof(Action<T>),
            typeof(Action<T>)
            }, new ParameterModifier[0]);
            //Action<UnityEngine.Object> onSelectorClosed = o => { Debug.Log( "selector closed"+o.name ); };
            //Action<UnityEngine.Object> onSelectedUpdated = o => { Debug.Log( "selector updated"+o.name ); };
            miShow.Invoke(os, new object[]
                {
                initialValueOrNull,
                typeof(T),
                null,
                false,
                null,
                OnSelectorClosed,
                OnSelectionChanged,
                }
            );
        }

        /// <summary>
        /// Trims an absolute path and only takes the relative path of the project
        /// </summary>
        /// <param name="absolutePath">e.g. C:/Documents/M7/Assets/Data</param>
        /// <returns>Trimmed part: e.g. Assets/Data</returns>
        public static string GetAssetPathFromAbsolutePath(string absolutePath)
        {
            if(absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }

            return absolutePath;
        }
    }
}