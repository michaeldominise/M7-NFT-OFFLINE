using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameRuntime
{
    public class UIFollowWorldObject : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] Camera thisCamera { get; set; }
        [ShowInInspector, ReadOnly] Camera targetCamera { get; set; }
        [ShowInInspector, ReadOnly] Transform targetObject { get; set; }

        private void Awake()
        {
            thisCamera = thisCamera ?? Camera.main;
            targetCamera = thisCamera ?? Camera.main;
        }

        public void SetValues(Transform targetObject, Camera thisCamera = null, Camera targetCamera = null)
        {
            this.targetObject = targetObject ?? this.targetObject;
            this.thisCamera = thisCamera ?? Camera.main;
            this.targetCamera = targetCamera ?? Camera.main;
        }


        private void LateUpdate()
        {
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            if (!targetObject)
                return;

            if(thisCamera == targetCamera)
                transform.position = targetObject.position;
            else
            {
                var targetScreenPoint = targetCamera.WorldToScreenPoint(targetObject.position, Camera.MonoOrStereoscopicEye.Mono);
                transform.position = thisCamera.ScreenToWorldPoint(targetScreenPoint, Camera.MonoOrStereoscopicEye.Mono);
            }
        }
    }
}
