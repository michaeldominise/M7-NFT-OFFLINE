using UnityEditor;
using M7.Skill;
using UnityEngine;
using UnityEngine.UIElements;

#if SKILLS_EDITOR
[CustomEditor(typeof(TargetSorterItem_CharacterCurrentHpPercentage), editorForChildClasses: true)]
#endif
public class TargetSorterItemEditorUIE : TargetSorterItemBaseEditorUIE
{
    public override VisualElement CreateInspectorGUI()
    {
        var root = base.CreateInspectorGUI();
        var label = new Label("Sort by HP Percentage");
        label.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
        root.Insert(0, label);

        return root;
    }
}
