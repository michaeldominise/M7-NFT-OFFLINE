using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MintUIColorContainer : MonoBehaviour
{
    [System.Serializable]
    public class IncubatorDesigns
    {
        public Sprite icon;
        public Sprite ticket;
        public Color pBase;
        public Color pGlow;
        public Gradient pBaseGradient;
        public Gradient pGlowGradient;
    }

    public ParticleSystem particleBase;
    public ParticleSystem particleGlow;

    [Space (10)]
    public Image incubatorIconSprite;
    public Image incubatorIdContainer;

    public TMP_Text incubatorRatity;
    public TMP_Text incubatorId;

    [Space (5)]
    public List<IncubatorDesigns> incubatorDesign = new List<IncubatorDesigns> ();
}
