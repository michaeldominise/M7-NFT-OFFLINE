using M7.Skill;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class CharacterAnimationEventManager : MonoBehaviour
    {
        public static CharacterAnimationEventManager Instance => BattleManager.Instance?.CharacterAnimationEventManager;

        //public CharacterQueueMananger castedCharacterQueueManager { get { return CharacterQueueMananger.Instance; } }

        [SerializeField] float lerpDuration = 1; // Can be set manually or dyanamicaly
        [SerializeField] float meleeXOffset = 0.5f;
        [SerializeField] float rangeXOffset = 2f;


        public IEnumerator MoveToDestination(CharacterInstance_Battle movingCharacter, CharacterInstance_Battle targetCharacter, SkillEnums.SkillTransitionType fromAnimation, SkillEnums.SkillTransitionType toAnimation)
        {
            var destinationPos = Vector3.zero;
            var flip = false;

            switch (toAnimation)
            {
                case SkillEnums.SkillTransitionType.None:
                    destinationPos = movingCharacter.transform.position + movingCharacter.SubSpineOffset;
                    flip = true;
                    break;
                case SkillEnums.SkillTransitionType.MoveToTarget:
                    if (!targetCharacter)
                        yield break;
                    destinationPos = targetCharacter.transform.TransformPoint(Vector3.right * (meleeXOffset + targetCharacter.CharacterObject.MeleeXOffset)) + Vector3.back;
                    break;
                case SkillEnums.SkillTransitionType.InPlace:
                    switch(fromAnimation)
                    {
                        case SkillEnums.SkillTransitionType.MoveToTarget:
                            if (!targetCharacter)
                                yield break;
                            destinationPos = targetCharacter.transform.TransformPoint(Vector3.right * (rangeXOffset + targetCharacter.CharacterObject.MeleeXOffset)) + Vector3.back;
                            flip = true;
                            break;
                        default:
                            destinationPos = movingCharacter.ActiveSpineContainer.position;
                            break;
                    }
                    break;
                case SkillEnums.SkillTransitionType.BattleCenter:
                    switch (fromAnimation)
                    {
                        case SkillEnums.SkillTransitionType.MoveToTarget:
                            if (!targetCharacter)
                                yield break;
                            flip = true;
                            break;
                    }
                    destinationPos = BattleCameraHandler.Instance.BattleWorldCamera.BattleCenter.position;
                    break;
            }

            var startPos = movingCharacter.ActiveSpineContainer.position;
            if (startPos == destinationPos)
                yield break;

            if (flip) movingCharacter.ActiveSpineContainer.localScale = new Vector3(-1, 1, 1);

            movingCharacter.MoveAttackAnimation(true, 1.3f);
            var startTime = Time.time;
            while (Time.time - startTime < 0.5f)
            {
                movingCharacter.ActiveSpineContainer.position = Vector3.Lerp(startPos, destinationPos, (Time.time - startTime) / 0.5f);
                yield return null;
            }

            movingCharacter.ActiveSpineContainer.position = destinationPos;
            movingCharacter.MoveAttackAnimation(false, 1.3f); 
            if (flip) movingCharacter.ActiveSpineContainer.localScale = Vector3.one;
        }
    }
}