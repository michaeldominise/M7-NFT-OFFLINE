#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M7.GameData;
using M7.Skill;
using M7.ToolDownload;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;


[Serializable]
public class SkillOjectCapsule
{
    public string Hero;
    public string Gear;
    public string Element;
    public string Gear_Name;
    public string Rarity;
    public string Skill_Name;
    public string Skill_Illustration;
    public string Card_Type;
    public int Cost;
    public int ATK;
    public int DEF;
    public string M7_Description;

}

public class SkillObjectList
{
    public SkillOjectCapsule[] SkillObject;
}
public class SkillObjectData : SkillObjectDataLayout
{
    protected string OBJECT_DATA_PATH { get { return "Assets/M7/Skills/Prefab/Skills/Skills/HeroSkills/SkillObject_{0}.prefab"; } }
    static readonly string SkillObject_Bundle_ID = "934037209";
    static readonly string skill_Object = "";
    public SkillObjectData()
    {
        sheetId = SkillObject_Bundle_ID;
        SkillOjectCapsules = new SkillOjectCapsule[0];

    }


    public override void UpdateTable()
    {
        GoogleContentDownloader.updateSKills(
            sheetId,
            Table =>
            {
                SkillOjectCapsules = Table.SkillObject.Where(x => !x.Skill_Name.IsNullOrWhitespace()).ToArray();

            });
        if (Elements.IsNullOrEmpty())
        {
            Elements = AssetDatabase.FindAssets("Element", new[] {"Assets/M7/GameData/ScriptableObjects/"})
                .Select(v => AssetDatabase.LoadAssetAtPath<RPGElement>(AssetDatabase.GUIDToAssetPath(v))).ToList();
            Elements = Elements.Where(s => s != null).Distinct().ToList();
        }
    }

 /*   public override void CreateScriptableAsset()
    {
        if (SkillOjectCapsules != null)
        {
            foreach (var skillObject in SkillOjectCapsules)
            {
                string path = string.Format(OBJECT_DATA_PATH, skillObject.Skill_Name);
                var component = AssetDatabase.FindAssets("SkillObject_"+skillObject.Skill_Name, new[] {"Assets/M7/Skills/Prefab/Skills/Skills/HeroSkills/"}).Select(v=>AssetDatabase.LoadAssetAtPath<SkillObject>(AssetDatabase.GUIDToAssetPath(v))).FirstOrDefault();
                if (component != null)
                {
                    component.Init(skillObject, Elements);
                }
                else
                {
                    GameObject gameObject = MonoBehaviour.Instantiate(ReferenceSkillObject);
                    gameObject.name = skillObject.Skill_Name;
                    component = gameObject.GetComponent<SkillObject>();
                    component.Init(skillObject, Elements);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, path, InteractionMode.UserAction);
                    EditorUtility.FocusProjectWindow();
                    EditorUtility.SetDirty(gameObject);
                    Selection.activeObject = gameObject;
                    MonoBehaviour.DestroyImmediate(gameObject);
                }
            }
        }
    }*/
    

  
    public override void UploadTable()
    {
        SkillOjectCapsule data = new SkillOjectCapsule();
   //     data.BundleList = BundleDataTable;
    //    UploadContent(ARENA_PLAYFAB_KEY, data, PlayfabContentUtility.TitleType.InternalTitleData);
    }
}
#endif
