#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using M7.GameData;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace M7.ToolDownload
{

    public class ContentBaseLayout
    {
        public string spreadSheetId { get; set; }
        public string sheetId { get; set; }

        [ResponsiveButtonGroup("Base")]
        public virtual void UpdateTable()
        {
        }

        [ResponsiveButtonGroup("Base")]
        public virtual void UploadTable()
        {
        }

        [ResponsiveButtonGroup("Base")]
        public virtual void CreateScriptableAsset()
        {
            
        }
    }
}
#endif
