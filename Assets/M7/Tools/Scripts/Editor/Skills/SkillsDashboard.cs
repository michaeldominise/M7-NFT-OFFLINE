using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using EditorTools.UIElements;
using M7.Skill;
using System.IO;
using System.Collections.Generic;
using M7.Tools.Utility;
using Fasterflect;
using System;
using M7.Tools;

public class SkillsDashboard : EditorWindow
{
    private VisualElement detailsContainer;
    private ComboBox skillsComboBox;
    private VisualElement _skillsListContainer;

    private ToolbarBreadcrumbs _breadCrumbs;

    private ToolbarMenu _newSkillObjectMenu;
    private ToolbarButton _importButton;

    private ToolbarSearchField _searchField;

    private List<GameObject> _skillObjectPrefabs = new List<GameObject>();
    private List<string> _excludeDirectories = new List<string>()
    {
        "Assets/M7/Skills/Prefab/Skills/Skills/OldHeroSkills",
    };

    #region Internal
    private Dictionary<string, List<SkillObject>> _skillsDictionary = new Dictionary<string, List<SkillObject>>();
    #endregion

    [MenuItem("M7/Skills Dashboard")]
    public static void ShowExample()
    {
        SkillsDashboard wnd = GetWindow<SkillsDashboard>();
        wnd.titleContent = new GUIContent("Skills Dashboard");
    }

    private void OnEnable()
    {
        _skillObjectPrefabs = EditorDataSourceProvider.GetSkillObjects();
    }

    private void OnDisable()
    {
        _skillObjectPrefabs.Clear();
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/M7/Tools/Scripts/Editor/Skills/SkillsDashboardLayout.uxml");
        visualTree.CloneTree(rootVisualElement);

        detailsContainer = rootVisualElement.Q<VisualElement>("skill-details-container");
        skillsComboBox = rootVisualElement.Q<ComboBox>("skills-combobox");
        _skillsListContainer = rootVisualElement.Q<VisualElement>("skills-list-container");
        InitializeList();

        _newSkillObjectMenu = rootVisualElement.Q<ToolbarMenu>("new-skillobject-menu");
        _newSkillObjectMenu.menu.AppendAction("Add Empty", AddEmptySkillObject);
        _newSkillObjectMenu.menu.AppendAction("Add From Template", OnAddSkillObjectFromTemplateMenuSelect);

        _importButton = rootVisualElement.Q<ToolbarButton>("import-button");
        _importButton.clicked += ImportSkills;

        _searchField = rootVisualElement.Q<ToolbarSearchField>("search-field");
        _searchField.RegisterValueChangedCallback(SearchSkill);
    }

    private void InitializeList()
    {
        _skillsListContainer.Clear();
        List<string> directoryPaths = AssetDatabase.GetSubFolders("Assets/M7/Skills/Prefab/Skills/Skills").ToList();
        directoryPaths.RemoveAll(s => _excludeDirectories.Contains(s));
        foreach(string directory in directoryPaths)
        {
            SkillGroupFoldout groupFoldout = new SkillGroupFoldout();
            groupFoldout.Header = Path.GetFileName(directory);

            var skillObjects = AssetUtility.GetAssets<SkillObject>(directory, "*.prefab", SearchOption.AllDirectories);

            foreach(SkillObject skillObject in skillObjects)
            {
                var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/M7/Tools/Scripts/Editor/Skills/CustomLayouts/SkillObjectItemPrototype.uxml");
                VisualElement skillListItemLayout = visualTree.Instantiate();
                skillListItemLayout.AddToClassList("skill-item");
                skillListItemLayout.userData = skillObject;
                if(skillObject == null)
                {
                    continue;
                }

                skillListItemLayout.RegisterCallback<ClickEvent>(OnSkillItemClick);

                skillListItemLayout.Q<Label>("skill-item-name").text = skillObject.DisplayStats.DisplayName;
                groupFoldout.Add(skillListItemLayout);
            }
            _skillsListContainer.Add(groupFoldout);
        }
    }

    private void OnSkillItemClick(ClickEvent evt)
    {
        OnSkillItemSelectInternal(evt.target as VisualElement);
    }

    private void OnSkillItemSelectInternal(VisualElement target)
    {
        var querybles = rootVisualElement.Query(className: "skill-item-selected");
        querybles.ForEach((ve) =>
        {
            ve.RemoveFromClassList("skill-item-selected");
        });

        target.AddToClassList("skill-item-selected");
        SelectObject(target.userData as SkillObject);
    }

    private void SelectObject(SkillObject skillObject)
    {
        detailsContainer.Clear();
        Editor objectEditor = Editor.CreateEditor(skillObject);
        if (objectEditor != null)
        {
            InspectorElement ie = new InspectorElement(objectEditor);
            ie.style.flexGrow = 1;
            detailsContainer.Add(ie);
        }
    }

    private void AddEmptySkillObject(DropdownMenuAction menuAction)
    {
        var newObject = new GameObject();
        newObject.name = "NewSkillObject";
        var skillObject = newObject.AddComponent<SkillObject>();
        var newId = GUID.Generate().ToString();
        skillObject.MasterID = newId;
        skillObject.DisplayStats.DisplayName = "New Skill Object";
        string newObjectPath = M7PrefabUtility.CreateNew(newObject, "NewSkillObject.prefab", EditorPath.SkillObjectsLocation);
        DestroyImmediate(newObject);        

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(newObjectPath);
        _skillObjectPrefabs.Add(prefab);

        if (skillsComboBox != null)
        {
            skillsComboBox.listView.itemsSource = _skillObjectPrefabs;
            skillsComboBox.listView.Rebuild();
            skillsComboBox.listView.ClearSelection();
            int newObjectIndex = _skillObjectPrefabs.IndexOf(prefab);
            skillsComboBox.listView.AddToSelection(newObjectIndex);
        }

        InitializeList();        
    }

    private VisualElement FindSkillEntryWithId(string id)
    {
        var query = _skillsListContainer.Query<VisualElement>(className: "skill-item");
        VisualElement skillEntry = null;

        query.ForEach(ve =>
        {
            var skillObject = ve.userData as SkillObject;
            if(skillObject.MasterID == id)
            {
                skillEntry = ve;
                return;
            }
        });

        return skillEntry;
    }

    private void OnAddSkillObjectFromTemplateMenuSelect(DropdownMenuAction menuAction)
    {
        SearchablePopup.Show(_newSkillObjectMenu.worldBound, EditorDataSourceProvider.GetSkillObjects().Select(s => s.GetComponent<SkillObject>().DisplayStats.DisplayName).ToArray(), 0, AddSkillObjectFromTemplate);
    }

    private void AddSkillObjectFromTemplate(int index)
    {
        var selectedTemplate = EditorDataSourceProvider.GetSkillObjects()[index];
        string tempaltePath = AssetDatabase.GetAssetPath(selectedTemplate);
        string newObjectPath = Path.Combine(EditorPath.SkillObjectsLocation, $"Copy of {selectedTemplate.name}.prefab");
        AssetDatabase.CopyAsset(tempaltePath, newObjectPath);
        AssetDatabase.Refresh();

        skillsComboBox.listView.Rebuild();

        var newObject = AssetDatabase.LoadAssetAtPath<GameObject>(newObjectPath);
        _skillObjectPrefabs.Add(newObject);
        var so = newObject.GetComponent<SkillObject>();
        so.DisplayStats.DisplayName = $"Copy of {so.DisplayStats.DisplayName}";

        skillsComboBox.listView.Rebuild();
        skillsComboBox.listView.ClearSelection();
        int newObjectIndex = _skillObjectPrefabs.IndexOf(newObject);
        skillsComboBox.listView.AddToSelection(newObjectIndex);
    }

    private void ImportSkills()
    {
        GoogleTSVDownloader.Download(
            spreadSheetID: SheetIDs.MasterListId,
            sheetId: SheetIDs.HeroSkillsId,
            onDownloadComplete: (result) =>
            {
                if (result.result == ResultState.Success)
                {
                    EditorUtility.DisplayProgressBar("Import Data", "Translating sheet data...", 0f);
                    var stringsList = TSVTranslator.TranslateToStringsList(result.downloadedResult);
                    SkillObjectFactory.Create(stringsList, AssetPaths.HeroSkillsPath);
                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", result.error, "Ok");
                }
            });

        GoogleTSVDownloader.Download(
            spreadSheetID: SheetIDs.MasterListId,
            sheetId: SheetIDs.EnemySkillsId,
            onDownloadComplete: (result) =>
            {
                if (result.result == ResultState.Success)
                {
                    EditorUtility.DisplayProgressBar("Import Data", "Translating sheet data...", 0f);
                    var stringsList = TSVTranslator.TranslateToStringsList(result.downloadedResult);
                    SkillObjectFactory.Create(stringsList, AssetPaths.EnemySkillsPath);
                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", result.error, "Ok");
                }
            });
    }

    private void SearchSkill(ChangeEvent<string> evt)
    {
        var query = _skillsListContainer.Query(className: "skill-item");
        query.ForEach(item =>
        {
            var skill = item.userData as SkillObject;
            bool show = skill.DisplayStats.DisplayName.Contains(evt.newValue, StringComparison.InvariantCultureIgnoreCase);
            item.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        });
    }
}

public class SkillObjectFactory
{
    // Assets/M7/Skills/Prefab/Skills/Skills/ReferenceSkillObject.prefab
    public static void Create(List<List<string>> stringsList, string path)
    {
        var prefab = AssetUtility.GetAssets<GameObject>("ReferenceSkillObject", new[] { "Assets/M7/Skills/Prefab/Skills/Skills" })[0];
        var skillsList = AssetUtility.GetAssets<GameObject>("", new[] { path });

        foreach(List<string> strings in stringsList)
        {
            var existingSkill = skillsList == null ? null : skillsList.Where(sk => sk != null).Select(sk => sk.GetComponent<SkillObject>()).Where(sk => sk.MasterID == strings[0]).ToList();
            if(existingSkill == null || existingSkill.Count == 0)
            {
                Debug.Log($"New skill: {strings[0]}");
                var newSkill = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                newSkill.name = strings[0];
                var skillObject = newSkill.GetComponent<SkillObject>();
                skillObject.MasterID = strings[0];
                skillObject.DisplayStats.DisplayName = strings[1];
                skillObject.DisplayStats.Description = strings[2];
                //if (strings[3] != "")
                //{
                //    skillObject.SetFieldValue("skillTag", Enum.Parse(typeof(SkillTags), strings[3], true));
                //}

                PrefabUtility.SaveAsPrefabAsset(newSkill, Path.Combine(path, $"{newSkill.name}.prefab"));
                GameObject.DestroyImmediate(newSkill);
            } else
            {
                Debug.Log($"Existing skill: {existingSkill[0].MasterID}");
                var skill = existingSkill[0];
                skill.DisplayStats.DisplayName = strings[1];
                skill.DisplayStats.Description = strings[2];
                //if (strings[3] != "")
                //{
                //    skill.SetFieldValue("skillTag", Enum.Parse(typeof(SkillTags), strings[3], true));
                //}
            }            
        }
    }
}