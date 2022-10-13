using System;
using UnityEngine;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace M7.Match
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class MatchMatrixAttribute : Attribute {}

    public sealed class MatchMatrixAttributeDrawer : OdinAttributeDrawer<MatchMatrixAttribute, CellType[]>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect r = (Rect)EditorGUILayout.BeginVertical(label.text);
            if (GUI.Button(r, GUIContent.none))
                Debug.Log("Go here");
            GUILayout.Label("I'm inside the button");
            GUILayout.Label("So am I");
            EditorGUILayout.EndVertical();
            GUIHelper.RequestRepaint();
        }
    }
}
