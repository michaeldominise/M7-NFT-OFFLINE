using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public interface IStatusEffectInstanceController
    {
        public enum UpdateType { Add, Remove }

        public Transform StatusEffectInstanceContainer { get; }
        public List<StatusEffectInstance> StatusEffectInstanceLedger { get; }
        public void OnStatusEffectInstanceLedgerUpdate(StatusEffectInstance updatedStatusEffectInstance, UpdateType updateType);
    }
}