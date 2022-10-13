using Fasterflect;
using M7.FX.VFX.Scripts;
using M7.Skill;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Skill Effects editor visual element
/// </summary>
public class SkillData_MultipleEffectsEditorUIE : VisualElement
{
    private SkillData_MultipleEffect _skillData;
    private SerializedObject _serializedObject;
    private SerializedProperty _serializedProperty;
    private int _skillDataIndex;
    private string _prefabPath;
    private string _effectId;

    public Func<PageController> pageController;

    public SkillData_MultipleEffect SkillData
    {
        get => _skillData;
        set
        {
            _skillData = value;
        }
    }

    public SkillData_MultipleEffectsEditorUIE()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/M7/Tools/Scripts/Editor/Skills/Layouts/SkillData_MultipleEffectsDrawerLayout.uxml");
        visualTree.CloneTree(this);


        //DrawTargetManager(_root, property);
    }

    /// <summary>
    /// Sets the serialized property of the skill effects so that we can use it in the visual element
    /// </summary>
    /// <param name="serializedProperty"></param>
    /// <param name="index"></param>
    public void Setup(SerializedProperty serializedProperty, int index)
    {
        _serializedProperty = serializedProperty;
        _skillDataIndex = index;

        GetInternalData();
        Rebind();
    }

    /// <summary>
    /// Get the data of the skill effects through reflection
    /// </summary>
    private void GetInternalData()
    {
        var assetPath = AssetDatabase.GetAssetPath((_serializedProperty.serializedObject.targetObject as SkillObject).gameObject);
        _prefabPath = assetPath;
        var sko = (_serializedProperty.serializedObject.targetObject as SkillObject);
        _effectId = (string)sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("effectId");
    }

    /// <summary>
    /// Rebinds all UIs
    /// This is always done when a new skill is selected so that the the new skill object property
    /// is binded to the UI.
    /// Binding is typically just saying that let Unity handle the property change notifs
    /// </summary>
    private void Rebind()
    {
        DrawTargetManager();
        DrawVFXInspector();
        DrawStatusEffectsInspector();
    }

    #region Target Manager 
    /// <summary>
    /// Draws the target manager
    /// Shows just a summary of the target manager if there is
    /// Double clicking it will open the target manager in another page
    /// </summary>
    private void DrawTargetManager()
    {
        var container = this.Q<PrefabComponentInspector>("target-manager-inspector");
        container.templatesDataSource = () => EditorDataSourceProvider.TargetManagerPrefabs.Select(t => t.name).ToArray();
        container.onNewFromTemplate = OnNewTargetManager;
        container.onRemove = OnTargetManagerRemove;

        var sko = (_serializedProperty.serializedObject.targetObject as SkillObject);
        var tm = sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("targetManager") as TargetManager;

        container.Clear();
        if (tm != null)
        {
            var masterDetailItem = new MasterDetailItemPrototype(
                drawFunction: () => DebugElements.Get(tm));
            masterDetailItem.onOpenItem = () =>
            {
                TargetManagerEditorUIE targetManagerView = new TargetManagerEditorUIE();
                targetManagerView.Bind(new SerializedObject(tm));
                pageController?.Invoke().PushItem("Target Manager", targetManagerView);

                targetManagerView.onCustomFilterAdd = (go, list) =>
                {
                    AddCustomFilter(go);
                    list.Rebuild();
                };
                targetManagerView.onCustomFilterRemove = (name) =>
                {
                    var nameTokens = name.Split('|');
                    RemoveCustomFilter(nameTokens[1]);
                };
                targetManagerView.onSorterAdd = (go, list) =>
                {
                    AddSorter(go);
                    list.Rebuild();
                };
                targetManagerView.onSorterRemove = (name) =>
                {
                    var nameTokens = name.Split('|');
                    RemoveSorter(nameTokens[1]);
                };


            };
            

            //InspectorElement ie = new InspectorElement(tmEditor);
            //ie.style.flexGrow = 1;
            container.Add(masterDetailItem);
        }
    }

    /// <summary>
    /// Add custom filter to target manager
    /// This gets the custom filter prefab and adds it to the SkillObject prefab and unpacks it completely
    /// </summary>
    /// <param name="filter"></param>
    private void AddCustomFilter(GameObject filter)
    {
        string parentPath = $"SkillEffects/{_effectId}/TargetManager/CustomFilters";
        M7PrefabUtility.AppendObject(_prefabPath, parentPath, filter,
            onObjectAppended: (root, newObject) =>
            {
                var sko = root.GetComponent<SkillObject>();
                newObject.name = $"{newObject.name}|{Guid.NewGuid().Shorten()}";

                var tm = sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("targetManager") as TargetManager;
                var lst = tm.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("targetFilterItemCustomList");
                lst.CallMethod("Add", newObject.GetComponent<ITargetFilter>());
            });
    }

    /// <summary>
    /// Removes custom filter from target manager
    /// This also removes the custom filter gameobject inside the skillobject prefab
    /// </summary>
    private void RemoveCustomFilter(string name)
    {
        M7PrefabUtility.RemoveChildWithNameContain(_prefabPath, name,
            onChildRemoved: (root) =>
            {
                var sko = root.GetComponent<SkillObject>();

                var tm = sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("targetManager") as TargetManager;
                var lst = tm.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("targetFilterItemCustomList");
                var size = (int)lst.GetPropertyValue("Count");

                int index = -1;
                var lstArray = lst.CallMethod("ToArray");
                for (int i = 0; i < size; i++)
                {
                    var e = lstArray.GetElement(i) as UnityEngine.Object;
                    if (e == null)
                    {
                        index = i;
                    }
                }
                lst.CallMethod("RemoveAt", index);
            });
    }

    /// <summary>
    /// Add sorter to target manager
    /// This also removes the sorter gameobject inside the skillobject prefab
    /// </summary>
    private void AddSorter(GameObject sorter)
    {
        string parentPath = $"SkillEffects/{_effectId}/TargetManager/Sorters";
        M7PrefabUtility.AppendObject(_prefabPath, parentPath, sorter,
            onObjectAppended: (root, newObject) =>
            {
                var sko = root.GetComponent<SkillObject>();
                newObject.name = $"{newObject.name}|{Guid.NewGuid().Shorten()}";

                var tm = sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("targetManager") as TargetManager;
                var lst = tm.GetFieldValue("sorter").GetFieldValue("sorterItems");
                lst.CallMethod("Add", newObject.GetComponent<ITargetSorter>());
            });
    }

    /// <summary>
    /// Remove sorter from target manager
    /// This gets the custom filter prefab and adds it to the SkillObject prefab and unpacks it completely
    /// </summary>
    private void RemoveSorter(string name)
    {
        M7PrefabUtility.RemoveChildWithNameContain(_prefabPath, name,
            onChildRemoved: (root) =>
            {
                var sko = root.GetComponent<SkillObject>();
                var tm = sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("targetManager") as TargetManager;
                var lst = tm.GetFieldValue("sorter").GetFieldValue("sorterItems");
                var size = (int)lst.GetPropertyValue("Count");

                int index = -1;
                var lstArray = lst.CallMethod("ToArray");
                for (int i = 0; i < size; i++)
                {
                    var e = lstArray.GetElement(i) as UnityEngine.Object;
                    if (e == null)
                    {
                        index = i;
                    }
                }
                lst.CallMethod("RemoveAt", index);
            });
    }

    /// <summary>
    /// Adds the target manager prefab to the skillobject prefab and unpacks it completely
    /// This also adds custom filter and sorter headers
    /// </summary>
    /// <param name="templateName"></param>
    private void OnNewTargetManager(string templateName)
    {
        if (EditorUtility.DisplayDialog("Warning", "This will replace the current initial target manager. All filters and sorters will be removed. Are you sure you want to continue?", "Yes", "No"))
        {
            GameObject newObject = EditorDataSourceProvider.TargetManagerPrefabs.Find(g => g.name == templateName);
            M7PrefabUtility.AddOrReplaceChild(newObject, _prefabPath, _effectId, "TargetManager",
                onChildAdd: (root, newObject) =>
                {
                    var so = root.GetComponent<SkillObject>();
                    if (newObject.transform.Find("CustomFilters") == null)
                    {
                        var customFiltersObject = new GameObject();
                        customFiltersObject.transform.SetParent(newObject.transform);
                        customFiltersObject.name = "CustomFilters";
                    }
                    if (newObject.transform.Find("FallbackFilters") == null)
                    {
                        var fallbackFiltersObject = new GameObject();
                        fallbackFiltersObject.transform.SetParent(newObject.transform);
                        fallbackFiltersObject.name = "FallbackFilters";
                    }
                    if (newObject.transform.Find("Sorters") == null)
                    {
                        var sortersObject = new GameObject();
                        sortersObject.transform.SetParent(newObject.transform);
                        sortersObject.name = "Sorters";
                    }
                    so.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).SetFieldValue("targetManager", newObject.GetComponent<TargetManager>());
                });

            EditorApplication.delayCall += () =>
            {
                DrawTargetManager();
            };
        }
    }

    private void OnTargetManagerRemove()
    {

    }
    #endregion

    #region VFX
    /// <summary>
    /// Draws vfx inspector
    /// Shows just a summary of the vfx if there is
    /// Double clicking it will open the vfx editor in another page
    /// </summary>
    private void DrawVFXInspector()
    {
        var container = this.Q<PrefabComponentInspector>("vfx-inspector");
        container.templatesDataSource = () => EditorDataSourceProvider.GetVFXs().Select(t => t.name).ToArray();
        container.onNewFromTemplate = OnNewVFX;
        container.onRemove = OnVFXRemove;


        var sko = (_serializedProperty.serializedObject.targetObject as SkillObject);
        var vfx = sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("vfx") as VFXSkillSystem;

        container.Clear();
        if (vfx != null)
        {
            var masterDetailItem = new MasterDetailItemPrototype(
                drawFunction: () => DebugElements.Get(vfx));
            masterDetailItem.onOpenItem = () =>
            {
                Editor objectEditor = Editor.CreateEditor(vfx);

                if (objectEditor == null)
                {
                    container.Add(new Label("Unable to show status effect."));
                }
                else
                {
                    InspectorElement ie = new InspectorElement(objectEditor);

                    pageController?.Invoke().PushItem($"VFX", ie);
                }
            };

            container.Add(masterDetailItem);
        }
    }

    /// <summary>
    /// Remove the vfx in the skillobject
    /// </summary>
    private void OnVFXRemove()
    {
        M7PrefabUtility.OpenPrefab(_prefabPath,
            onObjectOpen: (root) =>
            {
                var sko = root.GetComponent<SkillObject>();
                sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).SetFieldValue("vfx", null);
            });
    }

    /// <summary>
    /// Shows the vfx summary in the editor
    /// </summary>
    /// <param name="vfxName"></param>
    private void OnNewVFX(string vfxName)
    {
        var selectedVFX = EditorDataSourceProvider.GetVFXs().Find(g => g.name == vfxName);
        var vfx = selectedVFX.GetComponent<VFXSkillSystem>();

        M7PrefabUtility.OpenPrefab(_prefabPath,
            onObjectOpen: (root) =>
            {
                var sko = root.GetComponent<SkillObject>();
                sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).SetFieldValue("vfx", vfx);
            });
    }
    #endregion

    #region Status Effects
    private ListView _statusEffectsList;
    /// <summary>
    /// Draws the status effect
    /// Shows just a summary of the status effects if there is
    /// Double clicking it will open the status effect in another page
    /// </summary>
    private void DrawStatusEffectsInspector()
    {
        var container = this.Q<PrefabComponentInspector>("status-effects");
        var sko = (_serializedProperty.serializedObject.targetObject as SkillObject);

        _statusEffectsList = this.Q<ListView>("status-effects-list");
        _statusEffectsList.BindProperty(_serializedProperty.FindPropertyRelative("statusEffects"));
        _statusEffectsList.makeItem += StatusEffectMakeItem;
        _statusEffectsList.bindItem += StatusEffectBindItem;

        container.templatesDataSource = () => EditorDataSourceProvider.GetStatusEffects().Select(t => t.name).ToArray();
        container.onNewFromTemplate = OnNewStatusEffect;
        container.onRemove = OnStatusEffectRemove;

        EditorApplication.delayCall += () =>
        {
            FitToContents(_statusEffectsList);
        };
    }

    /// <summary>
    /// Adds the a status effect gameobject to the skillobject prefab
    /// </summary>
    /// <param name="effectName"></param>
    private void OnNewStatusEffect(string effectName)
    {
        var selectedStatusEffect = EditorDataSourceProvider.GetStatusEffects().Find(g => g.name == effectName);
        string parentPath = $"SkillEffects/{_effectId}/StatusEffects";

        M7PrefabUtility.OpenPrefab(_prefabPath,
            onObjectOpen: (root) =>
            {
                var skillEffects = root.transform.Find("SkillEffects");
                var skillEffectsId = root.transform.Find($"SkillEffects/{_effectId}");
                var skillEffectsStatusEffect = root.transform.Find($"SkillEffects/{_effectId}/StatusEffects");

                if (skillEffects == null)
                {
                    var go = new GameObject();
                    go.name = "SkillEffects";
                    go.transform.SetParent(root.transform);
                    skillEffects = go.transform;
                }

                if (skillEffectsId == null)
                {
                    var go = new GameObject();
                    go.name = $"{_effectId}";
                    go.transform.SetParent(skillEffects);
                    skillEffectsId = go.transform;
                }

                if (skillEffectsStatusEffect == null)
                {
                    var go = new GameObject();
                    go.name = "StatusEffects";
                    go.transform.SetParent(skillEffectsId);
                    skillEffectsStatusEffect = go.transform;
                }
            });

        M7PrefabUtility.AppendObject(_prefabPath, parentPath, selectedStatusEffect,
            onObjectAppended: (root, newObject) =>
            {
                var sko = root.GetComponent<SkillObject>();
                newObject.name = $"{newObject.name}|{Guid.NewGuid().Shorten()}";

                var lst = sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("statusEffects");
                lst.CallMethod("Add", newObject.GetComponent<StatusEffect>());
            });

        EditorApplication.delayCall += () =>
        {
            FitToContents(_statusEffectsList);
        };
    }

    /// <summary>
    /// Remove status effect gameobject in skillobject prefab
    /// </summary>
    private void OnStatusEffectRemove()
    {
        var selectedStatusEffect = _statusEffectsList.selectedItem;
        if (selectedStatusEffect == null)
        {
            return;
        }

        var name = (selectedStatusEffect as SerializedProperty).objectReferenceValue.name;
        var nameTokens = name.Split('|');

        M7PrefabUtility.RemoveChildWithNameContain(_prefabPath, nameTokens[1],
            onChildRemoved: (root) =>
            {
                var sko = root.GetComponent<SkillObject>();

                var lst = sko.GetFieldValue("dataList").CallMethod("ToArray").GetElement(_skillDataIndex).GetFieldValue("statusEffects");
                lst.CallMethod("RemoveAt", _statusEffectsList.selectedIndex);
            });

        EditorApplication.delayCall += () =>
        {
            FitToContents(_statusEffectsList);
        };
    }

    /// <summary>
    /// Bind the status effect object to the visual element
    /// for serialization
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="id"></param>
    private void StatusEffectBindItem(VisualElement ve, int id)
    {
        var md = (ve as MasterDetailItemPrototype);
        var statusEffect = (_statusEffectsList.itemsSource[id] as SerializedProperty).objectReferenceValue as StatusEffect;
        md.DrawCustom(() =>
        {
            return DebugElements.StatusEffect(statusEffect, id);
        });

        md.onOpenItem = () =>
        {
            Editor objectEditor = Editor.CreateEditor(statusEffect);

            if (objectEditor == null)
            {
                ve.Add(new Label("Unable to show status effect."));
            }
            else
            {
                InspectorElement ie = new InspectorElement(objectEditor);

                pageController?.Invoke().PushItem($"Status Effect #{id}", ie);
            }
        };
        
    }

    /// <summary>
    /// Status effect list draw function
    /// </summary>
    /// <returns></returns>
    private VisualElement StatusEffectMakeItem()
    {
        var masterDetailItem = new MasterDetailItemPrototype();
        return masterDetailItem;
    }

    #endregion

    #region Misc
    /// <summary>
    /// Fit list contents inside the list
    /// Mostly used in Unity 2020
    /// </summary>
    /// <param name="list"></param>
    private void FitToContents(ListView list)
    {
        if (list.itemsSource == null)
        {
            return;
        }

        list.style.height = list.itemsSource.Count * list.itemHeight;
    }
    #endregion

    #region Reflection

    #endregion
}
