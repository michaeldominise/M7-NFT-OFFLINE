using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFXEvent : MonoBehaviour
{
    [SerializeField] GameObject _VFXAttack;
    [SerializeField] GameObject[] _VFXOthers;

    [HideInInspector]
    [SerializeField] ParticleSystem[] atkfx;
    
    [HideInInspector]
    [SerializeField] List<ParticleSystem> otherfx;

    void Awake()
    {
        if (_VFXAttack != null)
        {
            atkfx = _VFXAttack.GetComponentsInChildren<ParticleSystem>();
            //Debug.LogError("ATK FX = " + atkfx.Length);
            foreach (var fx in atkfx)
            {
                var emitfx = fx.emission;
                emitfx.enabled = false;
            }
        }

        if (_VFXOthers != null)
        {
            for (int i = 0; i < _VFXOthers.Length; i++)
            {
                var childfx = _VFXOthers[i].GetComponentsInChildren<ParticleSystem>();
                for (int fx = 0; fx < childfx.Length; fx++)
                {
                    otherfx.Add(childfx[fx].GetComponent<ParticleSystem>());
                    //Debug.LogError("Other FX = " + otherfx.Count);
                }
            }

            if (otherfx != null)
            {
                foreach (var fx in otherfx)
                {
                    var emitfx = fx.emission;
                    emitfx.enabled = true;
                }
            }
        }
    }
    public void CharFX_Dismiss()
    {
        if (otherfx != null)
        {
            foreach (var fx in otherfx)
            {
                var emitfx = fx.emission;
                emitfx.enabled = false;
            }
        }
    }

    public void CharFX_OnAttackStart()
    {
        if (atkfx != null)
        {
            foreach (var fx in atkfx)
            {
                var emitfx = fx.emission;
                emitfx.enabled = true;
            }
        }
    }
    public void CharFX_OnAttackEnd()
    {
        if (atkfx != null)
        {
            foreach (var fx in atkfx)
            {
                var emitfx = fx.emission;
                emitfx.enabled = false;
            }
        }
    }

    public void CharFX_OnDeath()
    {
        var deathFX = GetComponent<DeathFXSpawner>();
        if (deathFX != null)
            deathFX.SpawnDeathFX();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
            GetComponent<Animator>().Play("OnCharacterAttack");
        
        if(Input.GetKeyDown(KeyCode.DownArrow))
            GetComponent<Animator>().Play("OnCharacterDeath");

        if(Input.GetKeyDown(KeyCode.LeftArrow))
            GetComponent<Animator>().Play("OnCharacterSpawned");
    }
}
