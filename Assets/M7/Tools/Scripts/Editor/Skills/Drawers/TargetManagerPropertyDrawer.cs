using M7.Skill;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomPropertyDrawer(typeof(TargetManager))]
public class TargetManagerPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Tools/Skills/Layouts/TargetManagerEditorLayout.uxml");
        var drawer = visualTree.CloneTree(property.propertyPath);
        container.Add(drawer);

        return container;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}
