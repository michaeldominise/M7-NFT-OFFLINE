using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using M7.GameData;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace M7.Tools
{
    public class M7ToolWindow : OdinMenuEditorWindow
    {
        public static M7ToolWindow Instance { get; private set; }

        const string CHARACTERS = "Characters";
        const string CHAPTERS = "Chapters";
        const string LEVELS = "Levels";
        const string LEVELS_ADVENTURE = LEVELS + "/Adventure";
        const string LEVELS_PVP = LEVELS + "/Pvp";
        const string EQUIPABLES = "Equipables";
        const string SKILLS = "Skills";
        const string LEVEL_EDITOR = "Level Editor";
        const string CURRENCIES = "Currencies";
        const string SETTINGS = "Settings";
        const string ANIMATION = "Animation";
        const string ANIMATION_CONTROLLER_SETTER = ANIMATION + "/Controller Setter";

        public IM7DrawableWindow SelectedDrawableToolbar;
        OdinMenuTree tree;

        [MenuItem("Tools/M7ToolWindow")]
        private static void Open()
        {
            var window = GetWindow<M7ToolWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            Instance = this;
            tree = new OdinMenuTree();
            tree.DefaultMenuStyle.IconSize = 28.00f;
            RefreshMenuTree();
            return tree;
        }

        public static void RefreshMenuTree()
        {
            if (Instance == null)
                return;

            Instance.tree = new OdinMenuTree();
            Instance.tree.Add(CHARACTERS, new M7DrawableWindow<CharacterObject>());
            Instance.tree.Add(CHAPTERS, new M7DrawableWindow<ChapterData>(showImport: true));
            Instance.tree.Add(LEVELS, new M7DrawableWindow<LevelData>());
            Instance.tree.Add(LEVELS_ADVENTURE, new StageInfoDrawableWindow(x => x.GameMode == LevelData.GameModeType.Adventure, true));
            Instance.tree.Add(LEVELS_PVP, new M7DrawableWindow<LevelData>(x => x.GameMode == LevelData.GameModeType.PVP));
            Instance.tree.Add(EQUIPABLES, new M7DrawableWindow<EquipmentItem>());
            Instance.tree.Add(CURRENCIES, new M7DrawableWindow<CurrencyObject>());
            Instance.tree.Add(SKILLS, null);
            Instance.tree.Add(LEVEL_EDITOR, null);
            Instance.tree.Add(ANIMATION, null);
            Instance.tree.Add(ANIMATION_CONTROLLER_SETTER, new AnimationControllerSetter());
            Instance.tree.Add(SETTINGS, new M7DrawableWindow<M7Settings>());
        }

        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = MenuTree.Selection.FirstOrDefault();
            var selectedInstanceValue = selected.Value as IM7DrawableWindow;
            if (selectedInstanceValue == null)
                return;

            var toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            if (selected != null && selectedInstanceValue != null)
                selectedInstanceValue.DrawToolbar();
            SirenixEditorGUI.EndHorizontalToolbar();

            if (selected != null && selectedInstanceValue != null)
                selectedInstanceValue.OnDrawBody();
        }
    }
}