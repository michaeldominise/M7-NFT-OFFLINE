using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class GameEventManager : MonoBehaviour
    {
        //public static GameEventManager Instance => BattleManager.Instance?.GameEventManager;

        //public CharacterQueueMananger castedCharacterQueueManager { get { return CharacterQueueMananger.Instance; } }

        [ShowInInspector] public Component playerCharacter;
        [ShowInInspector] public Component enemyCharacter;

        public Component PlayerCharacter => playerCharacter;
        public Component EnemyCharacter => enemyCharacter;

        [SerializeField]
        private float skillDuration;
        public float SkillDuration => skillDuration;

        [SerializeField]
        private float finalDurationResult;
        public float FinalDurationResult => finalDurationResult;

        //Start and Target Position for Lerping Spine of PlayerCharacter
        [SerializeField]
        Vector3 startPos;
        [SerializeField]
        Vector3 targetPos;

        public void SetProperty(Component caster, Component target, float animationDuration)
        {
            playerCharacter = caster; //Set Player
            startPos = PlayerCharacter.transform.position; //Set start position by PlayerCharacter

            if (target.GetComponent<CharacterInstance_Battle>() != null) //Check if target is a DecreaseSkillPoint Skill_Object. Just checking if the target has CharacterInstance_Battle
            {
                skillDuration = animationDuration; // Set skillDuration from animationDuration parameter
                //finalDurationResult = castedCharacterQueueManager.InitialTimeQueue / 2; //Set final duration result and divide by initialTimeQueue by 2

                enemyCharacter = target; //Set enemyCharacter as target parameter
                targetPos = EnemyCharacter.transform.position + -EnemyCharacter.transform.right; //Set target position by the EnemyCharacterPosition

                StartCoroutine(MoveToPosition(targetPos, FinalDurationResult)); //Init StartCoroutine MoveToPosition lerp function
            }
        }

        IEnumerator MoveToPosition(Vector3 targetPosition, float lerpDuration)
        {
            float time = 0; //Set float time to 0

            while (time < lerpDuration) //While? then do lerp
            {
                if (startPos != targetPosition)
                {
                    PlayerCharacter.transform.position = Vector3.Lerp(startPos, targetPosition, time / lerpDuration);
                    PlayerCharacter.GetComponent<CharacterInstance_Battle>().MainSpineInstance.MoveAnimation(true, 1); //Inititialize MoveAnimation of the playercharacter to true and set animation clip speed by lerpDuration/FinalDUration Result
                }
                time += Time.deltaTime;
                yield return null;
            }

            PlayerCharacter.transform.position = targetPosition; //Set PlayerCharacter by the target position

            PlayerCharacter.GetComponent<CharacterInstance_Battle>().MoveAnimation(false, 1); //Inititialize MoveAnimation of the playercharacter to false and set animation clip speed by lerpDuration/FinalDUration Result
            PlayerCharacter.GetComponent<CharacterInstance_Battle>().SetTriggerAnimation("IsAttack"); //Initialize attack animation after moving

            //---This lines of code initialize the get hitAnimation with delay first---//
            yield return new WaitForSeconds(0.5f); // THIS COULD BE AN ACTION EVENT OR TIME BASED ON THE SKILL EXACT ATTACK FRAME
            EnemyCharacter.GetComponent<CharacterInstance_Battle>().SetTriggerAnimation("IsHit"); //Do get hit animation after yield return
            //---This lines of code initialize the get hitAnimation with delay first---//

            yield return new WaitForSeconds(SkillDuration); //Yield by skill duration 
            StartCoroutine(BackToPosition(startPos, FinalDurationResult)); //then execute backToPosition Lerp Function after yield skillduration
        }


        IEnumerator BackToPosition(Vector3 targetPosition, float lerpDuration)
        {
            float time = 0; //Set float time to 0
            Vector3 curPos = PlayerCharacter.transform.position; //Set currentPosition of the playercharacter

            while (time < lerpDuration) //While? then do lerp
            {
                PlayerCharacter.transform.position = Vector3.Lerp(curPos, targetPosition, time / lerpDuration);
                time += Time.deltaTime;
                PlayerCharacter.GetComponent<CharacterInstance_Battle>().MoveAnimation(true, -1); //Inititialize MoveAnimation of the playercharacter to true and set animation clip speed by lerpDuration/FinalDUration Result

                yield return null;
            }
            PlayerCharacter.transform.position = targetPosition; //Set playercharacter position back to the startPos

            PlayerCharacter.GetComponent<CharacterInstance_Battle>().MoveAnimation(false, -1);//Inititialize MoveAnimation of the playercharacter to false and set animation clip speed by lerpDuration/FinalDUration Result
        }


    }
}