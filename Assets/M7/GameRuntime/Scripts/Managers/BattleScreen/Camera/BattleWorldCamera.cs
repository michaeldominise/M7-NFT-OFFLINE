using System.Collections;
using System.Collections.Generic;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameRuntime
{
    public class BattleWorldCamera : MonoBehaviour
    {
        public static BattleWorldCamera Instance => BattleCameraHandler.Instance.BattleWorldCamera;

        public System.Action onWaypointMoveFinished;

        [SerializeField] AnimationCurve moveLerpCurve;
        [SerializeField] AnimationCurve rotLerpCurve;


        [SerializeField] GenericAnimation _shakeAnim;
        [SerializeField] GenericAnimation _shakeAnimStrong;
        [SerializeField] Transform battleCenter;

        public Transform BattleCenter => battleCenter;

        Coroutine moveLerpCoroutine;
        Coroutine rotationLerpCoroutine;


        public void StopShake() { _shakeAnim.StopAnim(); _shakeAnimStrong.StopAnim(); }
        public void ShakeCamera(float delay = 0) { _shakeAnim.PlayAnim(delay); }
        public void ShakeCamera_Strong(float delay = 0) { _shakeAnimStrong.PlayAnim(delay); }

        [Button]
        public void ResetToOrigPos(float duration = 1)
        {
            if (moveLerpCoroutine != null)
                StopCoroutine(moveLerpCoroutine);
            moveLerpCoroutine = StartCoroutine(_MoveLerp(transform.TransformPoint(Vector3.zero), duration));
        }

        [Button]
        public void ResetToOrigRot(float duration = 1)
        {
            if (rotationLerpCoroutine != null)
                StopCoroutine(rotationLerpCoroutine);
            rotationLerpCoroutine = StartCoroutine(_RotLerp(transform.parent.rotation * transform.rotation, duration));
        }

        IEnumerator _MoveLerp(Vector3 targetPos, float duration = 1, System.Action callback = null)
        {
            var currentPos = transform.position;
            var startTime = Time.time;

            while (startTime + duration > Time.time)
            {
                transform.position = Vector3.Lerp(currentPos, targetPos, moveLerpCurve.Evaluate((Time.time - startTime) / duration));
                yield return null;
            }

            transform.position = targetPos;

            if (callback != null)
                callback();
        }


        IEnumerator _RotLerp(Quaternion targetRotation, float duration = 1)
        {
            var currentRot = transform.rotation;
            var startTime = Time.time;

            while (startTime + duration > Time.time)
            {
                transform.rotation = Quaternion.Lerp(currentRot, targetRotation, rotLerpCurve.Evaluate((Time.time - startTime) / duration));
                yield return null;
            }
            transform.rotation = targetRotation;
        }

        IEnumerator _LookLerp(Vector3 targetForward, float duration = 1)
        {
            var currentRot = transform.forward;
            var startTime = Time.time;

            while (startTime + duration > Time.time)
            {
                transform.forward = Vector3.Lerp(currentRot, targetForward, rotLerpCurve.Evaluate((Time.time - startTime) / duration));
                yield return null;
            }
            transform.forward = targetForward;
        }
    }
}