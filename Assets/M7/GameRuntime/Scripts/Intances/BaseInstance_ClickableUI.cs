using M7.CDN.Addressable;
using M7.GameData;
using M7.Skill;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using M7.GameData.Scripts.RPGObjects.Boosters;
using UnityEngine.EventSystems;

namespace M7.GameRuntime
{
    [System.Serializable]
    public abstract class BaseInstance_ClickableUI<BaseSaveableDataType> : BaseRPGObjectInstance<BaseSaveableDataType>, IPointerClickHandler where BaseSaveableDataType : BaseSaveableData  
    {
        public void OnPointerClick(PointerEventData pointerEventData) => onClickInstance?.Invoke(SaveableData);
    }
}