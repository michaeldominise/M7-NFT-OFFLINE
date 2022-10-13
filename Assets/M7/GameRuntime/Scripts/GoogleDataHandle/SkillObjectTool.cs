#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class SkillObjectTool : OdinEditorWindow
{
    const string MAIN_CATEGORY = "Main Category";
    const string Skill_Object = "Skill Object";
    [MenuItem("Tools/Murasaki7/Admin/SkillObject")]
    
    private static void Open()
    {
        var window = GetWindow<SkillObjectTool>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(1200, 600);
        window.Show();
    }
    
    [TabGroup(MAIN_CATEGORY, Skill_Object)]
    public SkillObjectData SkillObjectBundle = new SkillObjectData();

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif