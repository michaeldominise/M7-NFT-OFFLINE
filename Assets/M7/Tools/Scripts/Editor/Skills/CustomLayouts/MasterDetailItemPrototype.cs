using M7.Tools.Utility;
using System;
using System.Linq;
using UnityEngine.UIElements;

public class MasterDetailItemPrototype : VisualElement
{
    public Action onOpenItem;

    private VisualElement _container;
    private Label _label;
    private Button _openButton;

    public string LabelText
    {
        get => _label.text;
        set
        {
            _label.text = value;
        }
    }

    public MasterDetailItemPrototype(Func<VisualElement> drawFunction = null)
    {
        var visualTree = AssetUtility.GetAssets<VisualTreeAsset>("masterdetailitemprototypelayout", new string[] { }).First();
        var uxml = visualTree.Instantiate();
        hierarchy.Add(uxml);

        _container = this.Q<VisualElement>("content-container");
        _container.RegisterCallback<MouseDownEvent>(OnDoubleClickLabel);

        _label = this.Q<Label>("item-label");
        
        if(drawFunction != null)
        {
            _label.style.visibility = Visibility.Hidden;
            _label.style.height = 0;
            _container.Add(drawFunction.Invoke());
        }

        _openButton = this.Q<Button>("open-button");
        _openButton.clicked += OpenItem;
    }

    private void OnDoubleClickLabel(MouseDownEvent evt)
    {
        if(evt.clickCount == 2)
        {
            OpenItem();
        }
    }

    private void OpenItem()
    {
        onOpenItem?.Invoke();
    }

    public void DrawCustom(Func<VisualElement> drawFunction)
    {
        _label.style.visibility = Visibility.Hidden;
        _label.style.height = 0;
        _container.Clear();
        _container.Add(drawFunction.Invoke());
    }
}
