using UnityEngine;
using UnityEngine.Events;

namespace M7.GameRuntime.Scripts.Spine
{
    public class AttackAnimationCallback : MonoBehaviour
    {
        public UnityAction Action;
        public bool IsStateDone { get; set; }
    
        public delegate void AnimationIsDone();
        public delegate void AnimatiorEnter();

        public void AnimationEnter ()
        {
           // Debug.Log (this.transform.parent.name + "Animation Enter " + IsStateDone);
            IsStateDone = false;
            //Action?.Invoke();
            //Action = null;
        }

        public void AnimationDone()
        {
            IsStateDone = true;
            //Debug.Log (this.transform.parent.name + "Animation Done " + IsStateDone);
            Action?.Invoke();
            Action = null;
        }
    }
}
