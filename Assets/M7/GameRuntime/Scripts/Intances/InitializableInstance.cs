using M7.CDN;
using M7.CDN.Addressable;
using M7.GameData;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameRuntime
{
    [System.Serializable]
    public abstract class InitializableInstance<ObjectDataType> : MonoBehaviour
    {
        public Action<ObjectDataType> onClickInstance;
        public virtual ObjectDataType ObjectData { get; protected set; }
        public virtual ObjectDataType RequestedObjectData { get; set; }
        
        public virtual void Init(ObjectDataType objectData, Action onFinish)
        {
            if(this.ObjectData?.Equals(objectData) ?? false)
                return;

            ObjectData = objectData;
            onFinish?.Invoke();
        }
    }
}