using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime.Scripts.UI.OverDrive
{
    [RequireComponent(typeof(Image))]
    public class CustomizedFill : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private enum FillType
        {
            Horizontal, Vertical
        }
        
        [SerializeField] private float start, end;
        [SerializeField] private FillType fillType;
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        [Button]
        public void Fill(float value)
        {
            //var fill = (end - start) * Mathf.Clamp(value, 0, 1);

            //if (fillType == FillType.Horizontal)
            //{
            //    _rectTransform.sizeDelta = new Vector2(fill + start, _rectTransform.sizeDelta.y);
            //    return;
            //}
            
            //_rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x , fill + start);
        }
    }
}
