using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFXSystem : MonoBehaviour
{
    ParticleSystem[] pfx;
    protected virtual void Start()
    {
        pfx = GetComponentsInChildren<ParticleSystem>();
        if (pfx != null)
            foreach (var fx in pfx)
            {
                var vfx = fx.emission;
                if (vfx.enabled == false)
                    vfx.enabled = true;
            }
    }

    public virtual void EmitParticles(bool state = false)
    {
        if (pfx != null)
            foreach (var fx in pfx)
            {
                if (state == true)
                    fx.Play();
                else
                    fx.Stop();
            }
    }
}

