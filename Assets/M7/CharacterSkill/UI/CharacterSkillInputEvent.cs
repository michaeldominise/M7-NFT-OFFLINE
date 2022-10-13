using UnityEngine;

namespace M7.GameRuntime.Scripts.UI.OverDrive
{
    public class CharacterSkillInputEvent : MonoBehaviour
    {
        public delegate void Click();

        public event Click OnClick;
        
        public bool IsClickable { get; set; }

        private void OnMouseUpAsButton()
        {
            //test
            print($"Clickable {IsClickable}");
            if (!IsClickable) return;

            OnClick?.Invoke();
        }

        private void OnDisable()
        {
            OnClick = null;
        }
    }
}
