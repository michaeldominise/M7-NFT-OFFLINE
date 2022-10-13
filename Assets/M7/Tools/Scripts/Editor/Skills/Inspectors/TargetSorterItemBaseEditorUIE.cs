using M7.Skill;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

#if SKILLS_EDITOR
[CustomEditor(typeof(TargetSorterItem<>), editorForChildClasses: true)]
#endif
public class TargetSorterItemBaseEditorUIE : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        var prop = serializedObject.GetIterator();
        prop.NextVisible(true);
        do
        {
            if (prop.name == "m_Script")
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
