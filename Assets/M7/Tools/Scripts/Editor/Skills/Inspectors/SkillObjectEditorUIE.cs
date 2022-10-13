using M7.Skill;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Fasterflect;
using System.Collections.Generic;

#if SKILLS_EDITOR
[CanEditMultipleObjects]
#endif
public class SkillObjectEditorUIE : Editor
{
    [SerializeField]
    private VisualTreeAsset _uxml;

    private PrefabComponentInspector _initialTargetManagerInspector;
    private ToolbarBreadcrumbs _breadCrumbs;

    private SkillObject _targetObject;

    private VisualElement _root;
    private string _prefabPath;

    private VisualElement _contentContainer;

    private const string topLevelKey = "top-level";
    private const string targetManagerViewKey = "target-manager-view";

    private PageController _pageController;

    /// <summary>
    /// Gets the target object whenever a skillobject is selected
    /// </summary>
    private void OnEnable()
    {
        _targetObject = target as SkillObject;
        if(_targetObject == null)
        {
            return;
        }
        var assetPath = AssetDatabase.GetAssetPath(_targetObject.gameObject);
        _prefabPath = assetPath;
    }

    /// <summary>
    /// Setups the inspector UI of skillobject
    /// </summary>
    /// <returns></returns>
    public override VisualElement CreateInspectorGUI()
    {
        //return base.CreateInspectorGUI();
        _root = new VisualElement();
        _uxml.CloneTree(_root);

        _contentContainer = _root.Q<VisualElement>("content-container");
        _breadCrumbs = _root.Q<ToolbarBreadcrumbs>("skill-breadcrumbs");

        _pageController = new PageController(
            initialText: $"[{_targetObject.MasterID}] {_targetObject.name}",
            breadcrumbs: _breadCrumbs,
            contentContainer: _contentContainer);

        EditorApplication.delayCall += () =>
        {
            DrawInitialTargetManagerInspector(_root);
        };
        InitializeSkillEffects(_root);

        _root.Q<Foldout>("default-inspector-foldout").value = false;
        var defaultInspector = _root.Q<IMGUIContainer>("default-inspector");
        defaultInspector.onGUIHandler = () => DrawDefaultInspector();

        _root.Q<ObjectField>("skill-icon").RegisterValueChangedCallback(OnSkillIconChange);

        var iconSprite = serializedObject.FindProperty("displayStats.skill_Image").objectReferenceValue as Sprite;
        if (iconSprite != null)
        {
            ChangeIconPreview(iconSprite.texture);
        }

        return _root;
    }

    private void OnSkillIconChange(ChangeEvent<UnityEngine.Object> evt)
    {
        ChangeIconPreview((evt.newValue as Sprite).texture);
    }

    private void ChangeIconPreview(Texture2D texture2D)
    {
        _root.Q<VisualElement>("skill-icon").style.backgroundImage = new StyleBackground(texture2D);
    }

    #region Initial Target Manager
    /// <summary>
    /// Draws initial target manager
    /// This only shows the summary of target manager
    /// double click it to open the target manager in a new page
    /// </summary>
    /// <param name="root"></param>
    private void DrawInitialTargetManagerInspector(VisualElement root)
    {
        var content = root.Q<PrefabComponentInspector>("initial-target-manager-inspector-box");
        content.templatesDataSource = () => EditorDataSourceProvider.TargetManagerPrefabs.Select(t => t.name).ToArray();
        content.onNewFromTemplate = OnInitialTargetManagerAdd;
        content.onRemove = OnInitialTargetManagerRemove;
        content.Clear();

        //if (_targetObject.SelectionTargetManager != null)
        //{
        //    var masterDetailItem = new MasterDetailItemPrototype(
        //        drawFunction: () => DebugElements.Get(_targetObject.SelectionTargetManager));
        //    masterDetailItem.onOpenItem = () =>
        //    {
        //        TargetManagerEditorUIE targetManagerView = new TargetManagerEditorUIE();
        //        targetManagerView.targetObject = _targetObject.SelectionTargetManager;
        //        targetManagerView.Bind(new SerializedObject(_targetObject.SelectionTargetManager));
        //        _pageController.PushItem("Initial Manager", targetManagerView);

        //        targetManagerView.onCustomFilterAdd = (go, list) =>
        //        {
        //            AddFilterInternal(go, $"{_targetObject.SelectionTargetManager.gameObject.name}/CustomFilters");
        //            list.Rebuild();
        //        };

        //        targetManagerView.onCustomFilterRemove = (name) =>
        //        {
        //            RemoveFilterInternal(name);
        //        };

        //        targetManagerView.onSorterAdd = (go, list) =>
        //        {
        //            AddSorterInternal(go, $"{_targetObject.SelectionTargetManager.gameObject.name}/Sorters");
        //            list.Rebuild();
        //        };

        //        targetManagerView.onSorterRemove = (name) =>
        //        {
        //            RemoveSorterInternal(name);
        //        };
        //    };
        //    content.Add(masterDetailItem);
        //}
    }

    /// <summary>
    /// Adds target manager prefab to skillobject prefab
    /// and unpacks it completely
    /// </summary>
    /// <param name="templateName"></param>
    private void OnInitialTargetManagerAdd(string templateName)
    {
        if(EditorUtility.DisplayDialog("Warning", "This will replace the current initial target manager. All filters and sorters will be removed. Are you sure you want to continue?", "Yes", "No"))
        {
            M7PrefabUtility.RemoveAllChildrenWithName(_prefabPath, "InitialTargetManager");
            GameObject newObject = EditorDataSourceProvider.TargetManagerPrefabs.Find(g => g.name == templateName);
            M7PrefabUtility.AddOrReplaceChild(newObject, _prefabPath, InspectorTag.SkillEffectsInitialTargetManager,
                onChildAdd: (root, newGO) =>
                {
                    var so = root.GetComponent<SkillObject>();
                    //so.SelectionTargetManager = newGO.GetComponent<TargetManager>();

                    newGO.name = "InitialTargetManager";

                    if(newGO.transform.Find("CustomFilters") == null)
                    {
                        var customFiltersObject = new GameObject();
                        customFiltersObject.transform.SetParent(newGO.transform);
                        customFiltersObject.name = "CustomFilters";
                    }
                    if (newGO.transform.Find("FallbackFilters") == null)
                    {
                        var fallbackFiltersObject = new GameObject();
                        fallbackFiltersObject.transform.SetParent(newGO.transform);
                        fallbackFiltersObject.name = "FallbackFilters";
                    }
                    if (newGO.transform.Find("Sorters") == null)
                    {
                        var sortersObject = new GameObject();
                        sortersObject.transform.SetParent(newGO.transform);
                        sortersObject.name = "Sorters";
                    }
                });
            DrawInitialTargetManagerInspector(_root);
        }
    }

    /// <summary>
    /// Remove the the initial target manager gameobject inside the skillobject prefab
    /// Redraws initial target manager
    /// </summary>
    private void OnInitialTargetManagerRemove()
    {
        M7PrefabUtility.RemoveAllChildrenWithName(_prefabPath, "InitialTargetManager");
        DrawInitialTargetManagerInspector(_root);
    }
    #endregion

    /// <summary>
    /// Adds <paramref name="filter"/> prefab to skillobject prefab and unpacks it completely
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="parentPath"></param>
    private void AddFilterInternal(GameObject filter, string parentPath)
    {
        M7PrefabUtility.AppendObject(_prefabPath, parentPath, filter,
            onObjectAppended: (root, newObject) =>
            {
                var sko = root.GetComponent<SkillObject>();
                newObject.name = $"{newObject.name}|{Guid.NewGuid().Shorten()}";
                
                var lst = sko.GetFieldValue("initialTargetManager").GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("targetFilterItemCustomList");
                var tfilter = newObject.GetComponent<ITargetFilter>();
                lst.CallMethod("Add", newObject.GetComponent<ITargetFilter>());
            });
    }

    /// <summary>
    /// Remove the filter gameobject inside the skillobject prefab
    /// </summary>
    /// <param name="name"></param>
    private void RemoveFilterInternal(string name)
    {
        M7PrefabUtility.RemoveChildWithNameContain(_prefabPath, name,
            onChildRemoved: (root) =>
            {
                var sko = root.GetComponent<SkillObject>();
                var itm = sko.GetFieldValue("initialTargetManager");
                var lst = sko.GetFieldValue("initialTargetManager").GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("targetFilterItemCustomList");
                var size = (int)lst.GetPropertyValue("Count");

                int index = -1;
                var lstArray = lst.CallMethod("ToArray");
                for(int i = 0; i < size; i++)
                {
                    var e = lstArray.GetElement(i) as UnityEngine.Object;
                    if(e == null)
                    {
                        index = i;
                    }
                }
                lst.CallMethod("RemoveAt", index);
            });
    }

    /// <summary>
    /// Adds the sorter prefab to skillobject prefab and unpacks it completely
    /// </summary>
    /// <param name="sorter"></param>
    /// <param name="parentPath"></param>
    private void AddSorterInternal(GameObject sorter, string parentPath)
    {
        M7PrefabUtility.AppendObject(_prefabPath, parentPath, sorter,
            onObjectAppended: (root, newObject) =>
            {
                var sko = root.GetComponent<SkillObject>();
                newObject.name = $"{newObject.name}|{Guid.NewGuid().Shorten()}";

                var lst = sko.GetFieldValue("initialTargetManager").GetFieldValue("sorter").GetFieldValue("sorterItems");
                lst.CallMethod("Add", newObject.GetComponent<ITargetSorter>());
            });
    }

    /// <summary>
    /// Remove the sorter in the skillobject prefab
    /// </summary>
    /// <param name="name"></param>
    private void RemoveSorterInternal(string name)
    {
        M7PrefabUtility.RemoveChildWithNameContain(_prefabPath, name,
            onChildRemoved: (root) =>
            {
                var sko = root.GetComponent<SkillObject>();
                var lst = sko.GetFieldValue("initialTargetManager").GetFieldValue("sorter").GetFieldValue("sorterItems");
                var size = (int)lst.GetPropertyValue("Count");

                int index = -1;
                var lstArray = lst.CallMethod("ToArray");
                for(int i = 0; i < size; i++)
                {
                    var e = lstArray.GetElement(i) as UnityEngine.Object;
                    if(e == null)
                    {
                        index = i;
                    }
                }
                lst.CallMethod("RemoveAt", index);
            });
    }

    #region Multiple Effects
    private void InitializeSkillEffects(VisualElement root)
    {
        var multipleEffectsInspector = root.Q<PrefabComponentInspector>("multiple-effects-inspector");
        var listview = multipleEffectsInspector.Q<ListView>("data-list");
        listview.itemsSource = SkillEffects;
        listview.bindItem += (ve, id) =>
        {
            var md = (ve as MasterDetailItemPrototype);
            md.DrawCustom(() =>
            {
                return DebugElements.SkillEffect(SkillEffects[id], id);
            });

            md.onOpenItem = () =>
            {
                SkillData_MultipleEffectsEditorUIE skillEffectView = new SkillData_MultipleEffectsEditorUIE();
                var prop = serializedObject.FindProperty("dataList").GetArrayElementAtIndex(id);
                skillEffectView.Setup(prop, id);
                skillEffectView.pageController = () => _pageController;
                _pageController.PushItem($"Skill Effect #{id}", skillEffectView);
            };
        };

        listview.makeItem += () =>
        {
            var masterDetailItem = new MasterDetailItemPrototype();
            return masterDetailItem;
        };
        multipleEffectsInspector.onNewData = () =>
        {
            M7PrefabUtility.OpenPrefab(_prefabPath,
                onObjectOpen: (root) =>
                {
                    string effectId = Guid.NewGuid().Shorten();
                    SkillData_MultipleEffect effect = new SkillData_MultipleEffect();
                    effect.effectId = effectId;
                    var sko = root.GetComponent<SkillObject>();
                    var lst = sko.GetFieldValue("dataList");
                    lst.CallMethod("Add", effect);

                    if(root.transform.Find("SkillEffects") == null)
                    {
                        var ske = new GameObject();
                        ske.name = "SkillEffects";
                        ske.transform.SetParent(root.transform);
                    }

                    var skeParent = root.transform.Find("SkillEffects");
                    var newSke = new GameObject();
                    newSke.name = effectId;
                    newSke.transform.SetParent(skeParent);

                    var vfx = new GameObject();
                    vfx.name = "VFX";
                    vfx.transform.SetParent(newSke.transform);
                });

            EditorApplication.delayCall += () =>
            {
                FitToContents(listview);
            };
        };

        multipleEffectsInspector.onRemove = () =>
        {
            var selectedIndex = listview.selectedIndex;
            var effectId = (string)_targetObject.GetFieldValue("dataList").CallMethod("ToArray").GetElement(selectedIndex).GetFieldValue("effectId");
            M7PrefabUtility.OpenPrefab(_prefabPath,
                onObjectOpen: (root) =>
                {
                    var sko = root.GetComponent<SkillObject>();
                    var lst = sko.GetFieldValue("dataList");
                    lst.CallMethod("RemoveAt", selectedIndex);

                    var objectToRemove = root.transform.FindRecursive(effectId);
                    if(objectToRemove != null)
                    {
                        DestroyImmediate(objectToRemove.gameObject);
                    }
                });
            EditorApplication.delayCall += () =>
            {
                FitToContents(listview);
            };
        };

        EditorApplication.delayCall += () =>
        {
            FitToContents(listview);
        };
    }
    #endregion

    /// <summary>
    /// Fit the list to the contents
    /// Usually for Unity 2020
    /// </summary>
    /// <param name="list"></param>
    private void FitToContents(ListView list)
    {
        list.style.height = list.itemsSource.Count * list.itemHeight;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnInitialTargetManagerSelect(int index)
    {
        

        //foreach(var t in targets)
        //{
        //    Debug.Log((t as SkillObject).transform.childCount);
        //    var so = t as SkillObject;
        //    GameObject go = new GameObject();
        //    go.name = "yey";
        //    M7PrefabUtility.AddOrReplaceChild(go, $"{EditorPath.SkillObjectsLocation}{so.gameObject.name}.prefab", InspectorTag.SingleTargetTargetManager);
        //}
        
    }

    #region Reflection
    private TargetManager InitialTargetManager
    {
        get
        {
            return _targetObject.GetFieldValue("initialTargetManager") as TargetManager;
        }
        set
        {
            _targetObject.SetFieldValue("initialTargetManager", value);
        }
    }

    private List<SkillData_MultipleEffect> SkillEffects
    {
        get
        {
            return _targetObject.GetFieldValue("dataList") as List<SkillData_MultipleEffect>;
        }
    }
    #endregion
}
