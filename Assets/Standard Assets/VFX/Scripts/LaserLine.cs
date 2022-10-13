using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour
{

    public Transform laserEnd;
    [SerializeField] bool _isWorld = false;
    private LineRenderer lr;
    Vector3 startPos, endPos;

    // Use this for initialization
    void Start()
    {
        lr = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isWorld)
        {
            startPos = transform.position;
            endPos = laserEnd.position;
        }
        else
        {
            startPos = transform.localPosition;
            endPos = laserEnd.localPosition;
        }

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }
}
