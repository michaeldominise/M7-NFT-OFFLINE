using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace M7.Match
{
    public class LevelGoalsTable
    {
        [FormerlySerializedAs("allLevelGoalsList")]
        [TableList(IsReadOnly = true, AlwaysExpanded = true, ShowIndexLabels = true), ShowInInspector, HorizontalGroup("Main")]
        private readonly List<LevelGoalsWrapper> allLevelGoalsList;

        private static LevelGoalsData levelGoalsEditor;
        bool LevelGoalsEditorShow { get { return levelGoalsEditor != null; } }

        [ShowIf("LevelGoalsEditorShow", false)]
        [Button("Close"), VerticalGroup("Main/Editor")]
        private void Close()
        {
            levelGoalsEditor = null;
        }

        [ShowIf("LevelGoalsEditorShow", false)]
        [ShowInInspector, VerticalGroup("Main/Editor")]
        public LevelGoalsData LevelGoalsData { get { return LevelGoalsEditorShow ? levelGoalsEditor : null; } }

        [ShowIf("LevelGoalsEditorShow", false)]
        [TableList(AlwaysExpanded = true, ShowIndexLabels = true), ShowInInspector, VerticalGroup("Main/Editor")]
        private GoalData[] GoalDataEditor { get { return LevelGoalsEditorShow ? levelGoalsEditor.goals : null; } set { if(LevelGoalsEditorShow) levelGoalsEditor.goals = value; } }

        public LevelGoalsData this[int index]
        {
            get { return this.allLevelGoalsList[index].Data; }
        }

        public LevelGoalsTable(IEnumerable<LevelGoalsData> data)
        {
            this.allLevelGoalsList = data.Select(x => new LevelGoalsWrapper(x)).ToList();
        }

        private class LevelGoalsWrapper
        {
            public LevelGoalsWrapper(LevelGoalsData data)
            {
                this.Data = data;
            }

            [ShowInInspector, TableColumnWidth(50)]
            public LevelGoalsData Data { get; private set; }

            [Button("Edit"), HorizontalGroup(""), TableColumnWidth(50)]
            public void Edit()
            {
                LevelGoalsTable.levelGoalsEditor = Data;
            }

            [Button("Delete"), HorizontalGroup(""), TableColumnWidth(50)]
            public void Delete()
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Data));
            }
        }
    }
}
