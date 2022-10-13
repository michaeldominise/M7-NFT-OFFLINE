using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Settings {
    public class SettingsDefaultTabHelper : MonoBehaviour {

    	public int DefaultTabIndex {
            get { return SettingsPanel.DefaultTabIndex; }
            set { SettingsPanel.DefaultTabIndex = value; }
        }
    }
}