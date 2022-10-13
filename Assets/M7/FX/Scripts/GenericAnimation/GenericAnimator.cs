using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAnimator : MonoBehaviour {


    public System.Action onAllAnimationsFinished;



    bool isOnFinish_Init;
    int finishedAnims;

    [SerializeField] GenericAnimation[] _animations;

    public virtual void Refresh()
    {
        finishedAnims = 0;
    }

    void InitOnFinish()
    {

        if (isOnFinish_Init)
            return;
        
        if (!isOnFinish_Init)
            isOnFinish_Init = true;

        foreach (GenericAnimation anim in _animations)
            anim.onEndAnim += OnAnimFinish;
    }
    void OnAnimFinish()
    {
        finishedAnims++;
        //Debug.Log("[GenericAnimator] OnAnimFinish " + finishedAnims + "/" + _animations.Length);

        if (finishedAnims == _animations.Length)
            if(onAllAnimationsFinished != null)
                onAllAnimationsFinished();
    }


    public virtual void PlayAnimations()
    {

        InitOnFinish();

        foreach (GenericAnimation anim in _animations)
            anim.PlayAnim();
    }

    public void StopAnimations()
    {
        foreach (GenericAnimation anim in _animations)
            anim.StopAnim();
    }
}
