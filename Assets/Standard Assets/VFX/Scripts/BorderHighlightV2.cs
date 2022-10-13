using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BorderHighlightV2 : MonoBehaviour
{
    [SerializeField] Transform[] _path;
    [SerializeField] GameObject _fx;

    [SerializeField] float _duration = 1;
    Vector3[] _waypoint;
    // Start is called before the first frame update
    void Start()
    {
        _waypoint = new Vector3[_path.Length];

        if (_path != null)
            for (int i = 0; i < _path.Length; i++)
            {
                _waypoint[i] = _path[i].localPosition;
            }

        _fx.transform.localPosition = _waypoint[0];

        _fx.transform.DOLocalPath(_waypoint, _duration, PathType.Linear, PathMode.Ignore, 0).
        SetLoops(-1).
        SetEase(Ease.Linear).
        SetOptions(true);
    }

    void OnDisable()
    {
        _fx.transform.DOPause();
    }

    void OnEnable()
    {
        _fx.transform.DOPlay();
    }

    void Update()
    {

    }
}
