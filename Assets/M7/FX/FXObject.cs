using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXObject : MonoBehaviour {
    virtual protected FXManager FXManager { get; private set; }
    [SerializeField] bool initOnStart = true;

    public virtual void Start()
    {
        if(initOnStart)
            Init();
    }

    protected virtual void Init()
    { 
    }

    public virtual void Despawn()
    {
        FXManager._Despawn(this);
    }

    public virtual void DelayedDespawn(float delay)
    {
        StartCoroutine(_DelayedDespawn(delay));
    }

    IEnumerator _DelayedDespawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        Despawn();
    }
}
