using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorTools.UIElements
{
    public class ComboBox : VisualElement
    {
        public Action onAddEmptyOption;
        public Action<GameObject> onItemSelect;
        public Action<IEnumerable<int>> onSelectedItemIndicesChange;
        public Action<VisualElement, int> onBindItem;
        public Func<VisualElement> onMakeItem;

        public ListView listView => _internalListView;
        public Button addOptionButton => _internalAddOptionButton;

        private ListView _internalListView;
        private Button _internalAddOptionButton;

        public new class UxmlFactory : UxmlFactory<ComboBox, UxmlTraits> { }
        public ComboBox()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/M7/Tools/Scripts/Editor/Skills/CustomLayouts/ComboBoxLayout.uxml");
            visualTree.CloneTree(this);

            _internalListView = this.Q<ListView>("options-list");

            //_listContainer = this.Q<VisualElement>("list-view");
        }
    }
}