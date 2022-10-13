/*
 * IntList.cs
 * Author: Cristjan Lazar
 * Date: OCT 31, 2018
 */

using System.Collections.Generic;
using UnityEngine;

namespace M7.Match {

    [CreateAssetMenu(menuName = "BGGamesCore/Match/DataContainers/IntList")]
    public class IntList : ScriptableObject {

        private List<int> value = new List<int>();

        public List<int> Value { get { return this.value; } set { this.value = value; } }

    }

}

