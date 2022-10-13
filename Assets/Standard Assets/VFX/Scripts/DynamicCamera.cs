using UnityEngine;

using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class DynamicCamera : MonoBehaviour 
{
    [SerializeField] private Vector2 m_TargetAspectRatio = new Vector2(750f, 1280f);
/*    [SerializeField]*/ private Camera m_Camera = null;
    void Awake()
    {
        m_Camera = GetComponent<Camera>();
/*    }
    public void Update()
    { */
        float nativeWidth = Screen.width;
        float nativeHeight = Screen.height;
        float nativeAspectRatio = nativeWidth / nativeHeight;

        // Mobile target resolution for 16:9 (1280x720)

        float targetWidth = m_TargetAspectRatio.x; // 720f; //1040;
        float targetHeight = m_TargetAspectRatio.y; // 1280f; //280;
        float targetAspectRatio = targetWidth / targetHeight;

        decimal nativeAspectRatioDec = System.Convert.ToDecimal(nativeAspectRatio);
        nativeAspectRatioDec = System.Math.Round(nativeAspectRatioDec, 2);

        // Tablet target resolution for 4:3 (1280x960)

        if(nativeAspectRatioDec == 1.33m)
        {
            targetWidth = 960f;
            targetHeight = 1280f;
            targetAspectRatio = targetWidth / targetHeight;
        }

        // Mobile target resolution for 3:2 (1280x854)
        // iPhone 4 / 4s

        if(nativeAspectRatioDec == 1.5m)
        {
            targetWidth = 854f;
            targetHeight = 1280f;
            targetAspectRatio = targetWidth / targetHeight;
        }

        // Texture Size (1040 / 280)
        float ratio = nativeAspectRatio / targetAspectRatio;
        m_Camera.orthographicSize = (280f / 2f) * ratio;
    }
}
