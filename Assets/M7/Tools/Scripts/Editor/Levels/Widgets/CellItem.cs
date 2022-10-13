using M7.Match;
using M7.Tools.Utility;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace M7.Tools
{
    /// <summary>
    /// Cell Item custom visual element
    /// This handles the display logic of each cell item
    /// e.g. setting the image, tint
    /// 
    /// The cell consists of 2 visual elements inside its root/container
    /// The <see cref="_image"/> is the normal cell image.
    /// For base cells, this is just the cube that we tint
    /// The <see cref="_subImage"/> is commonly used by the base cells.
    /// This is the element icon. For non-base cells, this can just be null
    /// </summary>
    public class CellItem : VisualElement
    {
        private VisualElement _image;
        private VisualElement _subImage;

        private CellType _cellType;

        public CellType CellType
        {
            get => _cellType;
            set
            {
                _cellType = value;
                SetImage(_cellType.Sprite == null ? _cellType.SubSprite : _cellType.Sprite);
                SetImageTint(_cellType.BaseColor);
                SetSubImage(_cellType.IconSprite ? _cellType.IconSprite.texture : null);
                tooltip = _cellType.TileName;
            }
        }

        public CellItem()
        {
            var root = new VisualElement();
            var visualTree = AssetUtility.GetAssets<VisualTreeAsset>("tileitemlayout", new string[] { }).First();
            visualTree.CloneTree(root);
            hierarchy.Add(root);

            _image = this.Q<VisualElement>("image");
            _subImage = this.Q<VisualElement>("sub-image");
        }

        public void SetImage(Sprite sprite)
        {
            if(sprite == null)
            {
                return;
            }

            SetImage(sprite.texture);
        }

        public void SetImage(Texture2D texture)
        {
            if(texture == null)
            {
                return;
            }

            _image.style.backgroundImage = texture;
        }

        public void SetImageTint(Color color)
        {
            _image.style.unityBackgroundImageTintColor = color;
        }

        public void SetSubImage(Texture2D texture)
        {
            _subImage.style.backgroundImage = texture;
        }

        public new class UxmlFactory : UxmlFactory<CellItem, UxmlTraits> { }
    }
}