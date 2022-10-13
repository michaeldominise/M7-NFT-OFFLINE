using Fasterflect;
using M7.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;

#if SKILLS_EDITOR
//[CustomEditor(typeof(TargetManager), editorForChildClasses: true)]
//[CanEditMultipleObjects]
#endif
/// <summary>
/// This is a custom visual elemnent for the target manager
/// This draws the custom filter, sorter and fallback filter in separate boxes
/// </summary>
public class TargetManagerEditorUIE : VisualElement
{
    [SerializeField]
    private VisualTreeAsset _uxml;

    private PrefabComponentInspector _customFiltersInspector;

    public string tag;
    public TargetManager targetObject;

    public Action<GameObject, ListView> onCustomFilterAdd;
    public Action<string> onCustomFilterRemove;

    public Action<GameObject, ListView> onSorterAdd;
    public Action<string> onSorterRemove;

    public TargetManagerEditorUIE()
    {
        //return base.CreateInspectorGUI();
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/M7/Tools/Scripts/Editor/Skills/Layouts/TargetManagerEditorLayout.uxml");
        visualTree.CloneTree(this);
        
        InitializeCustomFiltersInspector(this, "custom-filters-inspector");
        InitializeFallbackFiltersInspector(this, "fallback-filters-inspector");
        InitializeSortersInspector(this, "sorter-items-inspector");

        this.Q<Foldout>("default-inspector-foldout").value = false;
        //var defaultInspector = root.Q<IMGUIContainer>("default-inspector");
        //defaultInspector.onGUIHandler = () => DrawDefaultInspector();
    }

    /// <summary>
    /// Filters and sorters list
    /// Initialize: Initialize the UI for callbacks
    /// Add: Adds the filter/sorter prefab to the skillobject prefab and unpacks it completely
    /// Remove: Remove the filter/sorter prefab in the skillobject prefab
    /// </summary>

    #region Custom Filters
    ListView _customFiltersListView;
    private void InitializeCustomFiltersInspector(VisualElement root, string elementName)
    {
        _customFiltersInspector = root.Q<PrefabComponentInspector>(elementName);
        _customFiltersInspector.templatesDataSource = () => EditorDataSourceProvider.CustomTargetFilters
            .Where(t =>
            {
                var targetFilter = t.GetComponent<ITargetFilter>();
                return IsRightTypeOfFilter(targetFilter);
            })
            .Select(t => t.name).ToArray();
        _customFiltersInspector.onNewFromTemplate = OnCustomFilterAdd;
        _customFiltersInspector.onRemove = OnCustomFilterRemove;

        _customFiltersListView = _customFiltersInspector.Q<ListView>("custom-filters-list");
        

        InitializeCustomFiltersList(_customFiltersListView);

        EditorApplication.delayCall += () =>
        {
            FitToContents(_customFiltersListView);
        };
    }

    private void InitializeCustomFiltersList(ListView list)
    {
        list.makeItem += CustomFiltersListMakeItem;
        list.bindItem += CustomFiltersListBindItem;
    }

    private VisualElement CustomFiltersListMakeItem()
    {
        VisualElement container = new VisualElement();
        container.style.flexGrow = 1;

        return container;
    }

    private void CustomFiltersListBindItem(VisualElement ve, int index)
    {
        ve.Clear();
        Editor objectEditor = Editor.CreateEditor((_customFiltersListView.itemsSource[index] as SerializedProperty).objectReferenceValue);
        if(objectEditor == null)
        {
            ve.Add(new Label("Unable to show filter."));
        } else
        {
            InspectorElement ie = new InspectorElement(objectEditor);
            ie.style.flexGrow = 1;
            ve.Add(ie);
        }
    }

    private void OnCustomFilterAdd(string templateName)
    {
        var customFilters = EditorDataSourceProvider.CustomTargetFilters;
        GameObject selectedCustomFilter = customFilters.Find(g => g.name == templateName);
        onCustomFilterAdd?.Invoke(selectedCustomFilter, _customFiltersInspector.Q<ListView>("custom-filters-list"));
        EditorApplication.delayCall += () =>
        {
            FitToContents(_customFiltersListView);
        };
    }

    private void OnCustomFilterRemove()
    {
        if(_customFiltersListView.selectedItem == null)
        {
            return;
        }
        onCustomFilterRemove?.Invoke(((_customFiltersListView.selectedItem as SerializedProperty).objectReferenceValue as Component).gameObject.name);
        EditorApplication.delayCall += () =>
        {
            FitToContents(_customFiltersListView);
        };
    }

    private void FitToContents(ListView list)
    {
        if(list.itemsSource == null)
        {
            return;
        }

        list.style.height = list.itemsSource.Count * list.itemHeight;
    }

    private bool IsRightTypeOfFilter(ITargetFilter filter)
    {
        Type targetManagerType = targetObject.GetType();
        var filterType = filter.GetType().BaseType.GetGenericArguments()[0];
        var targertManagerFilterType = targetManagerType.BaseType.GetGenericArguments()[1].BaseType.GetGenericArguments()[0];

        return filterType == targertManagerFilterType;
    }
    #endregion

    #region Fallback Filters
    private PrefabComponentInspector _fallbackFiltersInspector;
    private ListView _fallbackFiltersList;
    private void InitializeFallbackFiltersInspector(VisualElement root, string elementName)
    {
        _fallbackFiltersInspector = root.Q<PrefabComponentInspector>(elementName);
        _fallbackFiltersInspector.onNewData = OnFallbackFilterAdd;
        _fallbackFiltersInspector.onRemove = OnFallbackFilterRemove;

        _fallbackFiltersList = _fallbackFiltersInspector.Q<ListView>("fallback-filters-list");


        //InitializeCustomFiltersList(_customFiltersListView);

        EditorApplication.delayCall += () =>
        {
            FitToContents(_fallbackFiltersList);
        };
    }

    private void OnFallbackFilterAdd()
    {
        Debug.Log("Add fallbackfilter");
    }

    private void OnFallbackFilterRemove()
    {

    }
    #endregion

    #region Sorters
    private PrefabComponentInspector _sortersInspector;
    private ListView _sortersListView;

    private void InitializeSortersInspector(VisualElement root, string elementName)
    {
        _sortersInspector = root.Q<PrefabComponentInspector>(elementName);
        _sortersInspector.templatesDataSource = () => EditorDataSourceProvider.Sorters
            .Where(t =>
            {
                var targetSorter = t.GetComponent<ITargetSorter>();
                return IsRightTypeOfSorter(targetSorter);
            })
            .Select(t => t.name).ToArray();
        _sortersInspector.onNewFromTemplate = OnSorterAdd;
        _sortersInspector.onRemove = OnSorterRemove;

        _sortersListView = _sortersInspector.Q<ListView>("sorters-list");

        InitializeSortersList(_sortersListView);

        EditorApplication.delayCall += () =>
        {
            FitToContents(_sortersListView);
        };
    }

    private void InitializeSortersList(ListView list)
    {
        list.makeItem += SortersListMakeItem;
        list.bindItem += SortersListBindItem;
    }

    private void SortersListBindItem(VisualElement ve, int id)
    {
        ve.Clear();
        Editor objectEditor = Editor.CreateEditor((_sortersListView.itemsSource[id] as SerializedProperty).objectReferenceValue);
        if(objectEditor == null)
        {
            ve.Add(new Label("Unable to show sorter."));
        } else
        {
            InspectorElement ie = new InspectorElement(objectEditor);
            ie.style.flexGrow = 1;
            ve.Add(ie);
        }
    }

    private VisualElement SortersListMakeItem()
    {
        VisualElement container = new VisualElement();
        container.style.flexGrow = 1;

        return container;
    }

    private void OnSorterAdd(string sorterName)
    {
        var sorters = EditorDataSourceProvider.Sorters;
        GameObject selectedSorter = sorters.Find(g => g.name == sorterName);
        onSorterAdd?.Invoke(selectedSorter, _sortersListView);
        EditorApplication.delayCall += () =>
        {
            FitToContents(_sortersListView);
        };
    }

    private void OnSorterRemove()
    {
        if (_sortersListView.selectedItem == null)
        {
            return;
        }

        onSorterRemove?.Invoke(((_sortersListView.selectedItem as SerializedProperty).objectReferenceValue as Component).gameObject.name);
        EditorApplication.delayCall += () =>
        {
            FitToContents(_sortersListView);
        };
    }

    private bool IsRightTypeOfSorter(ITargetSorter sorter)
    {
        Type targetManagerType = targetObject.GetType();
        var sorterType = sorter.GetType().BaseType.GetGenericArguments()[0];
        var targertManagerFilterType = targetManagerType.BaseType.GetGenericArguments()[1].BaseType.GetGenericArguments()[0];

        return sorterType == targertManagerFilterType;
    }
    #endregion

    /// <summary>
    /// Factory class to show this in UIBuilder
    /// </summary>
    public new class UxmlFactory : UxmlFactory<TargetManagerEditorUIE, UxmlTraits> { }
}
