using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime
{
    public abstract class UIBattle<UIPrefManagerType, TargetRefernceType, InstanceActionsType> : MonoBehaviour where UIPrefManagerType : MonoBehaviour where InstanceActionsType : InstanceActions
    {
        public UIPrefManagerType UIPrefManager { get; protected set; }
        [ShowInInspector, ReadOnly] public TargetRefernceType TargetReference { get; protected set; }
        protected abstract InstanceActionsType InstanceActions { get; }

        public virtual void Attach(UIPrefManagerType uiPrefManager, TargetRefernceType targetReference)
        {
            UIPrefManager = uiPrefManager;
            TargetReference = targetReference;
            ResetValues();
            ResetEvents();
            AttachEvents();
        }

        protected abstract void AttachEvents();
        protected abstract void ResetEvents();
        public abstract void ResetValues();
    }
}
