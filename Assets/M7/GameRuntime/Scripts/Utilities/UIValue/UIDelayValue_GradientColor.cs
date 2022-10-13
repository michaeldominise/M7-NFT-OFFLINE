using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime
{
    public class UIDelayValue_GradientColor : UIDelayValue
    {
        [SerializeField] Gradient colorGradient;
        [SerializeField] Graphic targetGraphics;

        protected override void InvokeUpdateEvents()
        {
            base.InvokeUpdateEvents();
            targetGraphics.CrossFadeColor(colorGradient.Evaluate(currentValue), 0, false, false);
        }
    }
}