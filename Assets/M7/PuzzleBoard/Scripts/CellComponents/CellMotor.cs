/*
 * TileMotor.cs
 * Author: Cristjan Lazar
 * Date: Oct 19, 2018
 */

using UnityEngine;
using UnityEngine.Events;


using DG.Tweening;
using System.Collections;
using System;
using Sirenix.OdinInspector;

namespace M7.Match {

    /// <summary>
    /// Tile component that handles movement.
    /// </summary>
    public class CellMotor : MonoBehaviour {
        [SerializeField] protected Transform target;
        [SerializeField] float acceleration = 17;
        [SerializeField] AnimationCurve impactAnimCurve;
        [SerializeField] float impactIntensity = 0.5f;
        [SerializeField] float impactDurationMultiplier = 0.5f;

        public Action ResetSpawnFlag;

        Coroutine impactCoroutine;
        Coroutine moveCoroutine;
        Vector3 newPosition;
        public bool IsMoving => moveCoroutine != null;

        private void OnDisable()
        {
            moveCoroutine = null;
            impactCoroutine = null;
        }

        [Button]
        public virtual void Move(Vector3 newPosition, bool instant = false, bool doImpact = true, float delay = 0)
        {
            this.newPosition = newPosition;
            if (moveCoroutine != null || !gameObject.activeInHierarchy)
                return;

            if (impactCoroutine != null)
                StopCoroutine(impactCoroutine);

            moveCoroutine = StartCoroutine(_Move(instant, doImpact, delay));
        }

        public IEnumerator _Move(bool instant = false, bool doImpact = true, float delay = 0)
        {
            if (target.position != newPosition)
            {
                yield return new WaitForSeconds(delay);
                if (instant)
                    target.position = newPosition;
                else
                {
                    var startTime = Time.time;
                    var originalPos = transform.position;
                    var newValue = transform.position;
                    var dir = (newPosition - target.position).normalized;
                    while (true)
                    {
                        target.position = newValue;
                        yield return null;
                        newValue = originalPos + dir * 0.5f * acceleration * Mathf.Pow(Time.time - startTime, 2);
                        if (Vector3.Distance(target.position, newValue) > Vector3.Distance(target.position, newPosition))
                            break;
                    }

                    target.position = newPosition;
                    if (doImpact)
                    {
                        impactCoroutine = StartCoroutine(DoImpactEffect(originalPos, Time.time - startTime));
                        ResetSpawnFlag?.Invoke();
                    }
                }
            }
            target.position = newPosition;
            moveCoroutine = null;
        }

        IEnumerator DoImpactEffect(Vector3 originalPos, float elapsedTime)
        {
            var targetDuration = elapsedTime * impactDurationMultiplier;
            for (var time = 0f; time < targetDuration; time += Time.deltaTime)
            {
                target.position = newPosition + (originalPos - newPosition) * impactAnimCurve.Evaluate(time / targetDuration) * impactIntensity;
                yield return null;
            }

            target.position = newPosition;
            impactCoroutine = null;
        }

        public void MoveLerp (Vector3 newPosition, float duration) => target.DOMove(newPosition, duration).Play();
    }
}

