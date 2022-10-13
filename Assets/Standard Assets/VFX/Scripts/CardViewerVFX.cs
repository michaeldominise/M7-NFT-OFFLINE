using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class CardViewerVFX : MonoBehaviour
{
    public Camera _camera;
    public GameObject _FXPanel;
    public Transform _parent;
    public List<GameObject> _childFX;

    [RangeAttribute(0, 1)]
    public float _fade = 0;

    Vector3 initPos, initScale;
    Quaternion initRot;

    // Use this for initialization

    void Awake()
    {
        _fade = 0;
        Setup();
    }

    void Setup()
    {
        _parent = transform.parent;
        _camera = GameObject.FindGameObjectWithTag("SafeAreaCamera").GetComponent<Camera>();
        _FXPanel = GameObject.FindGameObjectWithTag("FX");

        for (int i = 0; i < transform.childCount; i++)
        {
            _childFX.Add(transform.GetChild(i).gameObject);
        }
    }

    void Reset(bool state)
    {
        foreach (var child in _childFX)
        {
            if (_childFX != null)
                child.SetActive(state);
        }
    }

    void ShowFX(Transform value)
    {
        transform.SetParent(value);
        transform.position = new Vector2(0, transform.position.y);
        transform.rotation = Quaternion.identity;

        //prevent negative scale
        Vector3 absScale = new Vector3(Mathf.Abs(transform.localScale.x),
                                        Mathf.Abs(transform.localScale.y),
                                        Mathf.Abs(transform.localScale.z));
        transform.localScale = absScale;
    }

    void FXFadeInOut()
    {
        foreach (var alpha in _childFX)
        {
            var pFade = alpha.GetComponent<ParticleSystem>().main.startColor;
            Color fadeCol = pFade.color;
            fadeCol.a -= _fade;
            pFade = fadeCol;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_camera != null && _parent != null)
        {
            Vector2 pos = _camera.WorldToViewportPoint(_parent.position);

            if (_parent != null || _FXPanel != null)
            {
                if (pos.x < 0 || pos.x > 1)
                {
                    ShowFX(_parent);
                    Reset(false);
                }
                else
                {
                    ShowFX(_FXPanel.transform);
                    Reset(true);
                }
            }
        }
    }
}
