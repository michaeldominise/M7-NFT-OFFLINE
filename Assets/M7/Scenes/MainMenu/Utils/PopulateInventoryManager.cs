using M7.GameData;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using M7.GameRuntime;
using System.Threading.Tasks;

namespace M7
{
    [Serializable]
    public class PopulateInventoryManager<InstancePrefType, ObjectDataType> where InstancePrefType : InitializableInstance<ObjectDataType>
    {
        [SerializeField] protected InstancePrefType instancePref;
        [SerializeField] protected Transform container;
        [SerializeField] protected Transform scrollView;
        [SerializeField] protected int poolCount = 10;
        [SerializeField] protected float minDistanceToLoad = 15;
        [SerializeField] protected bool reverse;
        [SerializeField] protected GameObject noItemNotice;
        [SerializeField] protected GameObject loading;
        [SerializeField] protected GameObject oneItemOnlyFixer;
        [ShowInInspector, ReadOnly] public List<ObjectDataType> ObjectDataList { get; set; }
        [ShowInInspector, ReadOnly] protected List<InstancePrefType> PoolList { get; set; } = new List<InstancePrefType>();
        [ShowInInspector, ReadOnly] public List<Transform> ItemSlotList { get; set; } = new List<Transform>();
        [ShowInInspector, ReadOnly] bool InitDone { get; set; }

        protected Action<ObjectDataType> onItemClick;

        void Init()
        {
            if (InitDone)
                return;

            instancePref.gameObject.SetActive(false);
            for (var x = 0; x < poolCount; x++)
            {
                var team = UnityEngine.Object.Instantiate(instancePref, container);
                team.gameObject.SetActive(false);
                PoolList.Add(team);
            }
            InitDone = true;
        }

        public void Populate(List<ObjectDataType> saveableDataList, Action<ObjectDataType> onItemClick = null)
        { 
            Init();
            this.onItemClick = onItemClick;
            ObjectDataList = saveableDataList;

            if (ObjectDataList.Count == 0)
            {
                noItemNotice?.SetActive(true);
                oneItemOnlyFixer?.SetActive(false);
            }
            else if(ObjectDataList.Count == 1)
            {

                noItemNotice?.SetActive(false);
                oneItemOnlyFixer?.SetActive(true);
            }
            else
            {;
                noItemNotice?.SetActive(false);
                oneItemOnlyFixer?.SetActive(false);
            }


            var saveableDataListCount = saveableDataList.Count();
            for (var x = ItemSlotList.Count; x < saveableDataListCount; x++)
                CreateNewSlot(x);
            for (var i = saveableDataListCount - 1; i > ItemSlotList.Count; i--)
            {
                ItemSlotList[i].DetachChildren();
                UnityEngine.Object.Destroy(ItemSlotList[i].gameObject);
                ItemSlotList.RemoveAt(i);
            }
        }

        public void Sort(params SortUtility.SortData<ObjectDataType>[] sorters) => ObjectDataList = ObjectDataList.GetSortedItems(sorters);

        Transform CreateNewSlot(int slotIndex)
        {
            var slot = new GameObject($"Slot {slotIndex}", typeof(RectTransform)).transform;
            if (loading != null)
            {
                var lding = GameObject.Instantiate(loading, Vector3.one, Quaternion.identity);
                lding.transform.SetParent(slot);
            }

            slot.SetParent(container);
            slot.position = scrollView.position + Vector3.up * (minDistanceToLoad + 1);
            slot.localScale = Vector3.one;
            if (reverse)
                slot.SetAsFirstSibling();
            ItemSlotList.Add(slot);

            return slot;
        }

        public void Update()
        {
            if (!InitDone)
                return;

            for (int i = 0; i < ItemSlotList.Count; i++)
            {
                Transform itemSlot = ItemSlotList[i];
                if (Vector3.Distance(itemSlot.position, scrollView.position) > minDistanceToLoad)
                    continue;
                var poolIndex = (int)Mathf.Repeat(i, PoolList.Count);

                SetInstanceValues(itemSlot, i);
            }
        }

        protected virtual async void SetInstanceValues(Transform itemSlot, int index)
        {
            var poolIndex = (int)Mathf.Repeat(index, PoolList.Count);
            var objectData = ObjectDataList[index];
            if (PoolList[poolIndex].ObjectData?.Equals(objectData) ?? false)
                return;

            PoolList[poolIndex].transform.SetParent(itemSlot);
            PoolList[poolIndex].transform.SetAsLastSibling();
            PoolList[poolIndex].transform.localPosition = Vector3.zero;
            PoolList[poolIndex].transform.localScale = Vector3.one;
            PoolList[poolIndex].RequestedObjectData = objectData;
            PoolList[poolIndex].gameObject.SetActive(false);
            itemSlot.GetChild(0).gameObject.SetActive (true);

            if (PoolList[poolIndex].ObjectData != null)
            {
                await Task.Delay(2500);
                if (!PoolList[poolIndex].RequestedObjectData?.Equals(objectData) ?? false)
                    return;
            }
            
            if (itemSlot != null)
            {
                itemSlot.GetChild(0).gameObject.SetActive (false);
                PoolList[poolIndex].Init(objectData, null);
                PoolList[poolIndex].onClickInstance = onItemClick;
                PoolList[poolIndex].gameObject.SetActive(true);
            }
        }
    }
}
