/*
 * MatchGridLevelDataContainer.cs
 * Author: Cristjan Lazar
 * Date: Oct 30 ,2018
 */

using UnityEngine;

namespace M7.Match {

    /// <summary>
    /// Level data holder. Useful as a singular reference.
    /// </summary>
    public class MatchGridLevelDataContainer : MonoBehaviour {

        [SerializeField] private MatchGridEditorSavedLevelData levelData;

        public MatchGridEditorSavedLevelData LevelData { get { return levelData; } }

        public void Initialize (MatchGridEditorSavedLevelData newLevelData) {
            levelData = newLevelData;
        }

    }

}


