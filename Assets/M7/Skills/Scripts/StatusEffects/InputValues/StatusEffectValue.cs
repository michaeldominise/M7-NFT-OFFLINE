using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public abstract class StatusEffectValue : MonoBehaviour
    {
        [ShowInInspector, DisplayAsString(false)] public virtual string DebugText => $"";

        public abstract float GetValue(StatusEffectInstance statusEffectInstance);

        public virtual float InputValue
        {
            get => 0;
#if UNITY_EDITOR
            set => UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}