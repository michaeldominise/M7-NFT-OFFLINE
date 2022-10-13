using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VFX_GeneratorMaterialAssigner : MonoBehaviour
{
    //[SerializeField] VFXCurrency GeneratorType = VFXCurrency.Gaianite;
    [SerializeField] Material Gaianite, EnhancementKit, PowerUpChip;

    Material GeneratorMaterialType(string type)
    {
        Material mat = null;
        switch (type)
        {
            case "EnhancementKit":
                mat = EnhancementKit;
                break;
            case "Gaianite":
                mat = Gaianite;
                break;
            case "PowerUp":
                mat = PowerUpChip;
                break;
        }

        return mat;
    }
    ParticleSystemRenderer psr(GameObject obj) { return GetComponent<ParticleSystemRenderer>(); }
    ParticleSystem ps { get { return GetComponent<ParticleSystem>(); } }
    public float duration { get { return ps.duration; } }

    private void Awake()
    {
        ps.playOnAwake = false;
    }
    public void Play(string GeneratorType)
    {
        psr(gameObject).material = GeneratorMaterialType(GeneratorType);
        ps.Play();
    }

}
