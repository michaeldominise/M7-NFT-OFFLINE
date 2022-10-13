using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace M7.Match
{
    public class CellBagTable
    {
        [FormerlySerializedAs("allTileBags")]
        [TableList(IsReadOnly = true, AlwaysExpanded = true, ShowIndexLabels = true), ShowInInspector, HorizontalGroup("Main")]
        private readonly List<TileBagWrapper> allTileBags;

        private static CellBag tileBagEditor;
        bool TileBagEditorShow { get { return tileBagEditor != null; } }

        [ShowIf("TileBagEditorShow", false)]
        [Button("Close"), VerticalGroup("Main/Editor")]
        private void Close()
        {
            tileBagEditor = null;
        }

        [ShowIf("TileBagEditorShow", false)]
        [ShowInInspector, VerticalGroup("Main/Editor")]
        public CellBag TileBag { get { return TileBagEditorShow ? tileBagEditor : null; } }

        [ShowIf("TileBagEditorShow", false)]
        [TableList(AlwaysExpanded = true, ShowIndexLabels = true), ShowInInspector, VerticalGroup("Main/Editor")]
        private TileBagItem[] TileBagEditor { get { return TileBagEditorShow ? tileBagEditor.cellBagItems : null; } set { if(TileBagEditorShow) tileBagEditor.cellBagItems = value; } }

        public CellBag this[int index]
        {
            get { return this.allTileBags[index].Data; }
        }

        public CellBagTable(IEnumerable<CellBag> data)
        {
            this.allTileBags = data.Select(x => new TileBagWrapper(x)).ToList();
        }

        private class TileBagWrapper
        {
            public TileBagWrapper(CellBag data)
            {
                this.Data = data;
            }

            [ShowInInspector, TableColumnWidth(50)]
            public CellBag Data { get; private set; }

            [Button("Edit"), HorizontalGroup("")]
            public void Edit()
            {
                CellBagTable.tileBagEditor = Data;
            }

            [Button("Delete"), HorizontalGroup("")]
            public void Delete()
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Data));
            }
        }
    }
}
