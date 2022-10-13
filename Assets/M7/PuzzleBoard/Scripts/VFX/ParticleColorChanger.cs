using M7.Skill;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleColorChanger : MonoBehaviour
{
    [System.Serializable]
    public class colorChange
    {
        public SkillEnums.ElementFilter elementType;
        public ParticleSystem.MinMaxGradient _colorMode;
    }
    public colorChange[] colorChangeList;
    int colorCode = 0;
    ParticleSystem[] colored_ParticleSystem { get { return GetComponentsInChildren<ParticleSystem>(true); } }

    public void ApplyColorScheme(SkillEnums.ElementFilter elementType)
    {
        if (colored_ParticleSystem != null)
        {
            var colorChange = colorChangeList.FirstOrDefault(x => x.elementType == elementType);

            foreach (var particle in colored_ParticleSystem)
            {
                var col = particle.main;
                col.startColor = colorChange._colorMode;
            }
        }
    }
}
