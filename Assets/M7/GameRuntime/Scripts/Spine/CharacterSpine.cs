using M7.GameRuntime.Scripts.Spine;
using UnityEngine;

namespace M7.GameRuntime
{
    public class CharacterSpine : MonoBehaviour
    {
        [SerializeField] SpineSkinGenerator spineSkinGenerator;
        [SerializeField] Animator animator;
        [SerializeField] SpineOffsetManager spineOffsetManager;

        [SerializeField] GameObject shadowPref;
        [SerializeField] GameObject shadowObject;
        [SerializeField] Vector3 shadowSize;
        [SerializeField] Vector3 shadowOffset;
        [SerializeField] AttackAnimationCallback attackAnimationCallback;
        
        public SpineSkinGenerator SpineSkinGenerator => spineSkinGenerator;
	    public Animator Animator => Animator;
        public SpineOffsetManager SpineOffsetManager => spineOffsetManager;
        public GameObject ShadowObject => shadowObject;

        public AttackAnimationCallback AttackAnimationCallback => attackAnimationCallback;
        
        public void SetTriggerAnimation(string animParameter)
        {
            animator.SetTrigger(animParameter);
        }
        public void SetBoolAnimation(string animParameter, bool value) => animator.SetBool(animParameter, value);
        
        public void MoveAnimation(bool state, float speed)
        {
            //Debug.Log ("Move : " + state);
            animator.SetFloat("Speed", speed);
	        SetBoolAnimation("IsMoving", state);
        }

        public void SetShadowSettings(Transform parent)
        {
            shadowObject = Instantiate(shadowPref, parent);
            shadowObject.transform.localScale = shadowSize;
            shadowObject.transform.localPosition = shadowOffset;
        }

#if UNITY_EDITOR
        // [Button]
        // public void SetAnimationDuration()
        // {
        //     var clips = animator.runtimeAnimatorController.animationClips;
        //     foreach (var clip in clips)
        //     {
        //         var animationDictionary = GetAnimationDictionary(clip.name);
        //         
        //         if(animationDictionary != null)
        //             _dictAnimationDuration[animationDictionary.]
        //     }
        // }
        //
        // private Dictionary<SkillEnums.SkillAnimationType, float> GetAnimationDictionary(string clipName)
        // {
        //     var dict = Dictionary<SkillEnums.SkillAnimationType, float>;
        //
        //     switch (clipName)
        //     {
        //         case "attack1":
        //             break;
        //     }
        // }

        #endif
    }
}