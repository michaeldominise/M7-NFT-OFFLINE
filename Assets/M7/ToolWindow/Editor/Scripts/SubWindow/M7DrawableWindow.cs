using M7.Tools.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace M7.Tools
{
    public class M7DrawableWindow<DataType> : IM7DrawableWindow where DataType : Object
    {
        string SearchValue { get; set; }

        public IEnumerable<M7DrawableTableItem<DataType>> allData;
        System.Func<DataType, bool> filterCondiion;

        private bool showImport;

        [ShowInInspector]
        [TableList(AlwaysExpanded = true)]
        [HideIf("@selectedData != null")]
        M7DrawableTableItem<DataType>[] FilteredData { get => allData.Where(x => (filterCondiion?.Invoke(x.data) ?? true)).ToArray(); set { } }

        [ShowInInspector, InlineEditor(Expanded = true)]
        [HideIf("@selectedData == null")]
        DataType selectedData;

        public M7DrawableWindow(System.Func<DataType, bool> filterCondiion = null, bool showImport = false)
        {
            this.filterCondiion = filterCondiion;
            this.showImport = showImport;
            RefreshTable();
        }

        public void RefreshTable()
        {
            allData = AssetDatabase.FindAssets($"t:{typeof(DataType).Name}")
                .Select(guid => new M7DrawableTableItem<DataType>(AssetDatabase.LoadAssetAtPath<DataType>(AssetDatabase.GUIDToAssetPath(guid)), tableData => selectedData = tableData.data))
                .Where(x => x.data.name.ToLower().Contains(SearchValue.ToLower()))
                .OrderBy(x => x.data.name);
        }

        public virtual void ImportData()
        {
            GoogleTSVDownloader.Download(
                spreadSheetID: SheetIDs.MasterListId,
                sheetId: SheetIDs.GetFromType(typeof(DataType)),
                onDownloadComplete: (result) =>
                {
                    if (result.result == ResultState.Success)
                    {
                        EditorUtility.DisplayProgressBar("Import Data", "Translating sheet data...", 0f);
                        var data = TSVTranslator.Translate<DataType>(result.downloadedResult) as List<DataType>;
                        AssetUtility.CreateAssets(AssetPaths.PathWithType(typeof(DataType)), data);
                        EditorUtility.ClearProgressBar();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", result.error, "Ok");
                    }
                });
        }

        public void DrawToolbar()
        {
            if (selectedData == null)
            {
                SearchValue = SirenixEditorGUI.ToolbarSearchField(SearchValue);
                if (showImport && SirenixEditorGUI.ToolbarButton("Import"))
                {
                    ImportData();
                }
                if (SirenixEditorGUI.ToolbarButton("Refresh"))
                    RefreshTable();
            }
            else
            {
                if (SirenixEditorGUI.ToolbarButton(selectedData.name) || SirenixEditorGUI.ToolbarButton("x"))
                    selectedData = null;
            }
        }

        public void OnDrawBody() { }
    }

    [System.Serializable]
    public class M7DrawableTableItem<DatType> where DatType : Object
    {
        [ReadOnly] public DatType data;
        System.Action<M7DrawableTableItem<DatType>> onOpen;

        public M7DrawableTableItem(DatType data, System.Action<M7DrawableTableItem<DatType>> onOpen = null)
        {
            this.data = data;
            this.onOpen = onOpen;
        }

        [Button("Open"), TableColumnWidth(50, false)]
        public void Open() => onOpen(this);
    }
}

