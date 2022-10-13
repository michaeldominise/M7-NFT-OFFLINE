using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VFX_xp_bar : MonoBehaviour
{
    [SerializeField] Slider _xpslider;
    [SerializeField] ParticleSystem _xpfx, _xpbarburst;
    // Start is called before the first frame update
    void Start()
    {
     _xpbarburst.Stop();
    }
    public void PlayTrail(bool active)
    {
        if (active)
        _xpfx.Play();
        else
        _xpfx.Stop();

    }
    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3 (_xpslider.value,1,1);
       if(_xpslider.value >=1) _xpbarburst.Play();
            
    }
}
