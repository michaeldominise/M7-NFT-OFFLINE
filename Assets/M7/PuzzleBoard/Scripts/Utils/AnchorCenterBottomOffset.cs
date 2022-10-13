using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[ExecuteInEditMode]
public class AnchorCenterBottomOffset : MonoBehaviour {
    public enum MatchType { MatchWidth, MatchHeight }

    [SerializeField] Camera targetCam;
    [SerializeField] Vector2 bottomCenterOffset = new Vector2(0, 540);

    //[SerializeField] Vector2 referenceResolution = new Vector2(900, 1800);
    [SerializeField] Vector2 referenceResolution = new Vector2(900, 1800);
    [SerializeField] MatchType matchType;

    Vector3 bottomCenterOffsetVect3;
    Vector3 targetPos;

    float ratioResolution
    {
        get
        {
            return matchType == MatchType.MatchWidth ? (targetCam.pixelWidth / referenceResolution.x) : (targetCam.pixelHeight / referenceResolution.y);
        }
    }

    private void Awake()
    {
        UpdateArea();
    }

#if UNITY_EDITOR
    private void Update()
    {
        UpdateArea();
    }
#endif

    //[Button]
    //void ResetArea()
    //{
    //    targetCam = GetComponent<Camera>();
    //    targetCam.rect = Rect.zero;
    //}

    [Button]
    void UpdateArea()
    {
        if (targetCam == null)
            return;



        bottomCenterOffsetVect3.x = bottomCenterOffset.x + targetCam.pixelWidth / 2f;
        bottomCenterOffsetVect3.y = Screen.safeArea.yMin + bottomCenterOffset.y * ratioResolution;
        bottomCenterOffsetVect3.z = targetCam.transform.InverseTransformPoint(transform.position).z;
        targetPos = targetCam.ScreenToWorldPoint(bottomCenterOffsetVect3);
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }
}
