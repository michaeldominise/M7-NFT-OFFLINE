/*
 * TileTypeDrawer.cs
 * Author: Cristjan Lazar
 * Date: Oct 12, 2018
 */

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace M7.Match {

    public class TileTypeDrawer : OdinValueDrawer<CellType> {

        protected override void DrawPropertyLayout(GUIContent label) {
            var rect = EditorGUILayout.GetControlRect(label != null, 45);

            if (label != null)
                rect.xMin = EditorGUI.PrefixLabel(rect.AlignCenterY(15), label).xMin;
            else
                rect = EditorGUI.IndentedRect(rect);

            CellType tileType = this.ValueEntry.SmartValue;
            Texture texture = null;

            if (tileType)
            {
                if(tileType.Sprite != null)
                    texture = GUIHelper.GetAssetThumbnail(tileType.Sprite.texture, typeof(CellType), true);
                else if(tileType.SubSprite != null)
                    texture = GUIHelper.GetAssetThumbnail(tileType.SubSprite.texture, typeof(CellType), true);
                GUI.Label(rect.AddXMin(50).AlignMiddle(16), EditorGUI.showMixedValue ? "-" : tileType.TileName);
            }

            this.ValueEntry.WeakSmartValue = SirenixEditorFields
                .UnityPreviewObjectField(rect.AlignLeft(45), tileType, texture, this.ValueEntry.BaseValueType);
        }

    }

}
#endif