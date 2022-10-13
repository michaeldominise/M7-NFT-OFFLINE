using M7.Skill;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

#if SKILLS_EDITOR
/// <summary>
/// Target Filter custom editor
/// This shows just the name of the target filter
/// </summary>
[CustomEditor(typeof(TargetFilterItemCustom<>), editorForChildClasses: true)]
#endif
public class TargetFilterItemCustomBaseEditorUIE : Editor
{

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        var prop = serializedObject.GetIterator();
        prop.NextVisible(true);
        do
        {
            if(prop.name == "m_Script")
            {
                continue;
            }
            var propField = new PropertyField(prop);
            propField.RegisterValueChangeCallback(OnValueChange);
            propField.style.marginLeft = 8;
            root.Add(propField);
        } while (prop.NextVisible(false));

        return root;
    }

    private void OnValueChange(SerializedPropertyChangeEvent evt)
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}