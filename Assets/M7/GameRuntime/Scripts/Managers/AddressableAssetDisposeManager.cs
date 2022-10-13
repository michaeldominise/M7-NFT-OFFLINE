using M7.CDN;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace M7.GameRuntime
{
    public class AddressableAssetDisposeManager : MonoBehaviour
    {
        static AddressableAssetDisposeManager Instance => GameManager.Instance.AddressableAssetDisposeManager;

        [ShowInInspector] public Dictionary<IDisposableAssetReference, DisposeTrigger> disposableAssetReferenceList { get; private set; } = new Dictionary<IDisposableAssetReference, DisposeTrigger>();

        public static void DisposeAssetReference(IDisposableAssetReference disposableAssetReference)
        {
            if (disposableAssetReference == null)
                return;
            Instance.disposableAssetReferenceList.Remove(disposableAssetReference);
            disposableAssetReference.ReleaseAsset();
        }

        public static void DisposeAssetReference(DisposeTrigger disposeTrigger)
        {
            var keyValuePairList = Instance.disposableAssetReferenceList.Where(keyValuePair => keyValuePair.Value.Contains(disposeTrigger));
            foreach (var keyValuePair in keyValuePairList)
                DisposeAssetReference(keyValuePair.Key);
        }

        public static void AddDisposableAssetReference(IDisposableAssetReference disposableAssetReference)
        {
            Instance.disposableAssetReferenceList[disposableAssetReference] = WindowEventManager.WindowEventString;
        }
    }

    [System.Serializable]
    public struct DisposeTrigger
    {
        [SerializeField] string triggerID;

        public DisposeTrigger(string value) => triggerID = value;
        public static implicit operator string(DisposeTrigger dt) => dt.triggerID;
        public static implicit operator DisposeTrigger(string value) => new DisposeTrigger(value);
        public bool Contains(string value) => triggerID.StartsWith(value);
        public override string ToString() => triggerID;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}