/*
 * GridDimension.cs
 * Author: Cristjan Lazar
 * Date: Oct 18, 2018
 */

using UnityEngine;

namespace M7.Match {

    /// <summary>
    /// Grid dimensions in columns and rows.
    /// </summary>
    [System.Serializable]
    public struct GridDimensions {
        [SerializeField] private int columns;
        [SerializeField] private int rows;

        public int Columns { get { return columns; } }
        public int Rows { get { return rows;  } }

        public GridDimensions (int columns, int rows) {
            this.columns = columns;
            this.rows = rows;
        }

        public static GridDimensions DEFAULT = new GridDimensions(8,8);
    }

}

