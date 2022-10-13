using M7.Skill;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if SKILLS_EDITOR
[CustomEditor(typeof(TargetFilterItemCustom_CharacterHp), editorForChildClasses: true)]
#endif
public class TargetFilterHPEditorUIE : TargetFilterItemCustomBaseEditorUIE
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = base.CreateInspectorGUI();
        var label = new Label("HP Filter");
        label.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
        root.Insert(0, label);

        return root;
    }
}

[CustomEditor(typeof(TargetFilterItemCustom_TileDistance), editorForChildClasses: true)]
public class TargetFilterTileDistanceEditorUIE : TargetFilterItemCustomBaseEditorUIE
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = base.CreateInspectorGUI();
        var label = new Label("Tile Distance");
        label.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
        root.Insert(0, label);

        return root;
    }
}