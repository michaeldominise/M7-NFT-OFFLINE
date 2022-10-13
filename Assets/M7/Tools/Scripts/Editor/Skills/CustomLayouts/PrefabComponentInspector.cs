using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class PrefabComponentInspector : VisualElement
{
    #region UXML
    public new class UxmlFactory : UxmlFactory<PrefabComponentInspector, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private UxmlStringAttributeDescription _headerLabelText = new UxmlStringAttributeDescription { name = "header-label", defaultValue = "Header" };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            var headerLabelTextStr = _headerLabelText.GetValueFromBag(bag, cc);
            ((PrefabComponentInspector)ve).Init(headerLabelTextStr);
        }
    }
    #endregion

    public Action onNewData;
    public Action<string> onNewFromTemplate;
    public Action onRemove;
    public Func<string[]> templatesDataSource;

    private Label _headerLabel;
    private VisualElement _contentView;
    private ToolbarMenu _addMenu;
    private Button _deleteButton;

    private string _headerText;
    private string _tag;

    public PrefabComponentInspector()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/M7/Tools/Scripts/Editor/Skills/CustomLayouts/PrefabComponentInspectorLayout.uxml");
        visualTree.CloneTree(this);

        _headerLabel = this.Q<Label>("header-label");
        _deleteButton = this.Q<Button>("delete-button");
        _deleteButton.clicked += OnDeleteButtonClick;
        _addMenu = this.Q<ToolbarMenu>("add-menu");
        _addMenu.RegisterCallback<ClickEvent>(OnAddMenuButtonClick);
        _contentView = this.Q<VisualElement>("content-view");
        //SetupMenu(_addMenu);
    }

    private void OnAddMenuButtonClick(ClickEvent evt)
    {
        if (templatesDataSource == null)
        {
            onNewData?.Invoke();
        }
        else
        {
            SearchablePopup.Show(_addMenu.worldBound, templatesDataSource == null ? new string[] { } : templatesDataSource.Invoke(), 0, OnTemplateSelected);
        }
    }

    private void OnDeleteButtonClick()
    {
        onRemove?.Invoke();
    }

    public void Init(string headerText)
    {
        _headerText = headerText;
        _headerLabel.text = _headerText;
    }

    protected virtual void OnTemplateSelected(int index)
    {
        var templates = templatesDataSource == null ? new string[] { } : templatesDataSource.Invoke();
        onNewFromTemplate?.Invoke(templates[index]);
    }

    public override VisualElement contentContainer => _contentView;
}
