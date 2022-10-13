using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class RawImageVFX : MonoBehaviour
{
    public GameObject _ambientFX;
    public Camera _camera;
    GameObject fx;
    RawImage rawImage;
    // Use this for initialization

    void Awake()
    {
        transform.SetAsLastSibling();
        rawImage = GetComponent<RawImage>();

        if (_ambientFX != null)
        {
            if (fx == null)
            {
                //offset needed to prevent FX from overlapping on other

                fx = Instantiate(_ambientFX);
                fx.name = "ambientFX";
                fx.transform.SetParent(transform);
                fx.transform.localScale = Vector3.one;

                //offset needed to prevent FX from overlapping on other
                fx.transform.Translate(fx.transform.position + Vector3.right * 2000);
            }
        }

        if (_camera == null)
            return;
    }

    public void ShowFX(bool state)
    {
        if(fx != null)
            fx.SetActive(state);

        if(rawImage != null)
            rawImage.enabled = state;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 pos = _camera.WorldToViewportPoint(this.transform.position);

        if (pos.y < 0 || pos.y > 1)
        {
            fx.SetActive(false);
            rawImage.enabled = false;
        }
        else
        {
            fx.SetActive(true);
            rawImage.enabled = true;
        }
    }
}
