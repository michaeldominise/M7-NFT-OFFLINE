using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

namespace M7
{
    public abstract class PopupBox : MonoBehaviour 
    {
        protected static SpawnPool SpawnPool { get { return PopupCanvas.Instance.SpawnPool; } }

        protected Action onShow;
        protected Action onHide;

        public virtual T Show<T>() where T : PopupBox
        {
            if (gameObject.activeSelf)
                return this as T;

            gameObject.SetActive(true);

            if (onShow != null)
                onShow();

            return this as T;
        }

        public virtual void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            if (onHide != null)
                onHide();

            SpawnPool.Despawn(transform);
        }

        public PopupBox AddCallbacks(Action onShow, Action onHide)
        {
            if (onShow != null)
                this.onShow += onShow;
            if (onHide != null)
                this.onHide += onHide;

            return this;
        }

        public virtual void Clear()
        {
            onShow = null;
            onHide = null;
        }

        public virtual void ResetRectTransform()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            var rectTransform = GetComponent<RectTransform>();

            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
        }
    }
}
