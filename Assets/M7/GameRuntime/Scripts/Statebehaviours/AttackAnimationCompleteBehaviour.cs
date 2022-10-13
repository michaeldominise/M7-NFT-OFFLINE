using M7.GameRuntime.Scripts.Spine;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Statebehaviours
{
    public class AttackAnimationCompleteBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
        {
		    animator.GetComponent<AttackAnimationCallback>().AnimationEnter();
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	    {
		    animator.GetComponent<AttackAnimationCallback>().AnimationDone();
        }
    }
}
