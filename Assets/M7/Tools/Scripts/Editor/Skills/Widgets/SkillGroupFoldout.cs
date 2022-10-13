using M7.Tools.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace M7.Tools
{
    public class SkillGroupFoldout : VisualElement
    {
        #region UXML
        public new class UxmlFactory : UxmlFactory<SkillGroupFoldout, UxmlTraits> { }
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
                ((SkillGroupFoldout)ve).Init(headerLabelTextStr);
            }
        }
        #endregion

        private Toggle _header;
        private VisualElement _contentContainer;

        public SkillGroupFoldout()
        {
            var visualTree = AssetUtility.GetAssets<VisualTreeAsset>("SkillGroupFoldoutLayout", new string[] { }).First();
            var uxml = visualTree.Instantiate();
            hierarchy.Add(uxml);

            _header = this.Q<Toggle>("header");
            _header.RegisterValueChangedCallback(OnFoldoutValueChange);
            _contentContainer = this.Q<VisualElement>("skills-container");

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