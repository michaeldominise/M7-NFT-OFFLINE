using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

public class Box : VisualElement
{
    #region UXML
    public new class UxmlFactory : UxmlFactory<Box, UxmlTraits> { }
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
            ((Box)ve).Init(headerLabelTextStr);
        }
    }
    #endregion

    private Label _headerLabel;
    private VisualElement _boxContent;

    private string _headerText;

    public Box()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/M7/Tools/Scripts/Editor/Skills/CustomLayouts/Box.uxml");
        var uxml = visualTree.Instantiate();
        hierarchy.Add(uxml);

        _headerLabel = this.Q<Label>("header-label");
        _boxContent = this.Q<VisualElement>("box-content");
    }

    public Box(string headerText)
    {
        Init(headerText);
    }

    public void Init(string headerText)
    {
        _headerText = headerText;
        _headerLabel.text = _headerText;
    }

    public override VisualElement contentContainer => _boxContent;
}
