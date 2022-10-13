using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public abstract class UIValueAnimationEffect : MonoBehaviour
    {
        public abstract void OnValueUpdate(float value);
    }
}