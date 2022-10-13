using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXObject : FXObject {
    protected override FXManager FXManager { get { return VFXManager.Instance; } }

    public void Init(float lifeDuration)
    {
        DelayedDespawn(lifeDuration);
    }

    protected override void Init()
    {
    }
}
