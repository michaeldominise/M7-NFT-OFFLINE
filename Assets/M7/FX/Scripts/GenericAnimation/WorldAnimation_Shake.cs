using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class WorldAnimation_Shake : WorldAnimation 
{

    [SerializeField] protected Transform _targetTransform;
    [SerializeField] protected float speed = 1;
    [SerializeField] protected float intensity = 1;


    [SerializeField] AnimationCurve _shakeStrengthCurve_x;
    [SerializeField] AnimationCurve _shakeStrengthCurve_y;
    [SerializeField] AnimationCurve _shakeStrengthCurve_z;

    float curvedShakeStrengthX(float lerpval) { return intensity * _shakeStrengthCurve_x.Evaluate(lerpval); }
    float curvedShakeStrengthY(float lerpval) { return intensity * _shakeStrengthCurve_y.Evaluate(lerpval); }
    float curvedShakeStrengthZ(float lerpval) { return intensity * _shakeStrengthCurve_z.Evaluate(lerpval); }

    protected Vector3 _basePosition;
    protected Vector3 _randSphereOffset(float lerpStep)
    {

        return new Vector3((Mathf.PerlinNoise(Time.time * speed + 1f, Time.time * speed * 0.2f) - 0.5f) * curvedShakeStrengthX(lerpStep),
                           (Mathf.PerlinNoise(Time.time * speed + 2f, Time.time * speed * 0.4f) - 0.5f) * curvedShakeStrengthY(lerpStep),
                           (Mathf.PerlinNoise(Time.time * speed + 4f, Time.time * speed * 0.8f) - 0.5f) * curvedShakeStrengthZ(lerpStep));
        
    }

    protected Vector3 _randomPosition(float lerpStep)  { return _basePosition + _randSphereOffset(lerpStep);  }

    [Button]
    public override void PlayAnim(float delay = 0)
    {
        _basePosition = _targetTransform.localPosition;

        base.PlayAnim(delay);
        PlayAnimCoroutine(ShakeAnimCoroutine(delay));
    }

    // listener
    public void PlayAnim()
    {
        print("Camera Shake End");
        _basePosition = _targetTransform.localPosition;

        base.PlayAnim(0);
        PlayAnimCoroutine(ShakeAnimCoroutine(0));
    }
    
    protected virtual IEnumerator ShakeAnimCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        float lerpStep = 0;
        float elapsedTime = 0;

        while (lerpStep < 1)
        {
            lerpStep = elapsedTime / _animDuration;

            if (lerpStep <= 0)
                lerpStep = 0;

            _targetTransform.localPosition = _randomPosition(lerpStep);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        _targetTransform.localPosition = _basePosition;

        if (onEndAnim != null)
            onEndAnim();

    }
}
