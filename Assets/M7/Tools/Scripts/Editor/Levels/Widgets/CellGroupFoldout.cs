using M7.Tools.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace M7.Tools
{
    /// <summary>
    /// Cell Group Foldout Custom Visual Element
    /// This element is used to group cells with the same directory
    /// The grouping is done outside this class
    /// This class just handles the display logic
    /// </summary>
    public class CellGroupFoldout : VisualElement
    {
        #region UXML
        /// <summary>
        /// Factory class for the visual element to show in UI Builder
        /// </summary>
        public new class UxmlFactory : UxmlFactory<CellGroupFoldout, UxmlTraits> { }
        /// <summary>
        /// Initialize the header attribute in UI Builder's inspector
        /// </summary>
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
                ((CellGroupFoldout)ve).Init(headerLabelTextStr);
            }
        }
        #endregion

        private Toggle _header;
        private VisualElement _contentContainer;

        /// <summary>
        /// Constructor to initialize the UIs and register callbacks
        /// </summary>
        public CellGroupFoldout()
        {
            var visualTree = AssetUtility.GetAssets<VisualTreeAsset>("tilegroupfoldoutlayout", new string[] { }).First();
            var uxml = visualTree.Instantiate();
            hierarchy.Add(uxml);

            _header = this.Q<Toggle>("header");
            _header.RegisterValueChangedCallback(OnFoldoutValueChange);
            _contentContainer = this.Q<VisualElement>("tiles-container");

            ToggleFoldout(_header.value);
        }

        public string Header
        {
            get => _header.text;
            set
            {
                _header.text = value;
            }
        }

        private void OnFoldoutValueChange(ChangeEvent<bool> evt)
        {
            ToggleFoldout(evt.newValue);
        }

        public void Init(string headerText)
        {
            if (_header == null)
            {
                return;
            }

            _header.text = headerText;
        }

        public void ToggleFoldout(bool show)
        {
            _contentContainer.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public override VisualElement contentContainer => _contentContainer;
    }
}