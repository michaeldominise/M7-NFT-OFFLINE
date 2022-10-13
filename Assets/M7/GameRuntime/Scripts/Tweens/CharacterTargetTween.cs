using DG.Tweening;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Tweens
{
    // Tween for target icon when character is a target
    public class CharacterTargetTween : MonoBehaviour
    {
        private readonly Vector3 _localScale = new Vector3(0.5f, 0.5f, 1);
        private void OnEnable()
        {
            print($"target local scale {_localScale}");
            const float scalePercentage = 0.75f;
            var localScale = transform.localScale;
            transform.DOScale(new Vector3(scalePercentage * localScale.x, scalePercentage * localScale.y, localScale.z), 1).
                SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            DOTween.Kill(transform);
            transform.localScale = _localScale;
        }
    }
}
