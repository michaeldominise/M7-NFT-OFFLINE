using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace M7.GameData
{
    public static class SortUtility
    {
        public static List<T> GetSortedItems<T>(this IEnumerable<T> items, params SortData<T>[] sortData)
        {
            if (sortData == null || sortData.Length == 0)
                return items.ToList();

            var sortedItems = sortData[0].sortType == SortData.SortType.Accending ? items.OrderBy(sortData[0].condition) : items.OrderByDescending(sortData[0].condition);
            for (var x = 1; x < sortData.Length; x++)
                sortedItems = sortData[x].sortType == SortData.SortType.Accending ? sortedItems.ThenBy(sortData[x].condition) : sortedItems.ThenByDescending(sortData[x].condition);

            return sortedItems.ToList();
        }

        public class SortData<T> : SortData
        {
            public System.Func<T, object> condition;

            public SortData(System.Func<T, object> condition, SortType sortType = SortType.Accending)
            {
                this.condition = condition;
                this.sortType = sortType;
            }
        }

        public class SortData
        {
            public enum SortType { Accending, Decending }
            public SortType sortType;

            public SortData(SortType sortType = SortType.Accending) => this.sortType = sortType;
        }
    }
}
