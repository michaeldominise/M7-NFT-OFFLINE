/*
 * TweenCycleBase.cs
 * Author: Cristjan Lazar
 */

using UnityEngine;
using UnityEngine.Events;

using DG.Tweening;

public abstract class TweenCycleBase : MonoBehaviour {

    [SerializeField] protected float duration = 1f;

    [SerializeField] protected float interval = 0f;

    [SerializeField] protected float offsetTime = 0f;

    [SerializeField] protected Ease ease = Ease.Linear;

    [SerializeField] protected int loops = -1;

    [SerializeField] protected LoopType loopType = LoopType.Yoyo;

    [SerializeField] protected bool initOnStart = true;

    [SerializeField] protected bool playOnStart = true;

    public UnityEvent OnStart;

    public UnityEvent OnPlay;

    public UnityEvent OnPause;

    public UnityEvent OnComplete;

    public UnityEvent OnRewind;

    public Sequence seq { get; private set; }

    protected abstract Tweener InitTween ();

    protected abstract void Reset ();

    public void Initialize () {
        seq = DOTween.Sequence()
                     .Append(InitTween())
                     .SetEase(ease)
                     .AppendInterval(interval)
                     .SetLoops(loops, loopType)
                     .OnStart(OnStart.Invoke)
                     .OnPlay(OnPlay.Invoke)
                     .OnPause(OnPause.Invoke)
                     .OnComplete(OnComplete.Invoke)
                     .OnRewind(OnRewind.Invoke)
                     .SetAutoKill(false)
                     .Pause();

        seq.Goto(offsetTime, false);
    }

    public void Play () {
        seq?.PlayForward();
    }

    public void PlayReverse () {
        seq?.PlayBackwards();
    }

    public void Pause () {
        seq?.Pause();
    }

    public void Restart () {
        seq?.Restart();
    }

    public void Rewind () {
        seq?.Rewind();
    }

    public void Stop () {
        if (seq == null)
            return;

        seq.Rewind();
        seq.Kill();
    }

    #region Unity event methods
    void Start () {
        if (initOnStart)
            Initialize();

        if (playOnStart)
            Play();
    }

    void OnDestroy () {
        seq?.Kill();
    }
    #endregion

}

