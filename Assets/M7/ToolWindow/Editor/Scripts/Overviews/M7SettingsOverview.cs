using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace M7.Tools
{
    public class M7SettingsOverview : BaseOverview<M7SettingsOverview, M7Settings>
    {
        public override void UpdateOverview()
        {
            base.UpdateOverview();
            M7ToolWindow.RefreshMenuTree();
        }
    }
}