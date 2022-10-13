using DG.Tweening;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto.Prng;
using UnityEngine;
using UnityEngine.UI;
using M7.PuzzleBoard.Scripts.Booster;
using System.Linq;

namespace M7.Skill
{
    public partial class SkillQueueManager : MonoBehaviour
    {
        public static SkillQueueManager Instance => BattleManager.Instance?.SkillQueueManager;
        public enum State { Idle, Executing }

        [ShowInInspector, ReadOnly] public State CurrentState { get; private set; } = State.Idle;
        [ShowInInspector, ReadOnly] List<SkillQueueData> skillObjectQueue = new List<SkillQueueData>();
        Coroutine executeCoroutine;
        public Action executeFinishCallback;
        [ShowInInspector, ReadOnly] public ISkillCaster LastTargetReference { get; private set; }
        //SkillQueueData skillQueueData1;
        [SerializeField] public bool skip;

        public List<SkillQueueData> SkillObjectQueueData => skillObjectQueue;

        public Action<SkillQueueData> OnSkillQueueDataFinished;

        public int skillRoutineListCount;
        public void Init()
        {
        }

        public void WaitUntilIdle(Action callback)
        {
            if (CurrentState == State.Executing)
                executeFinishCallback += callback;
            else
            {
                callback?.Invoke();
            }
        }

        public void AddSkillsToQueue(IEnumerable<SkillQueueData> skillObjects, bool waitToFinishCurrent = true, Action onFinished = null, bool isForced = false, Action onStarted = null, float intervalDelay = 0)
           => StartCoroutine(_AddSkillsToQueue(skillObjects, waitToFinishCurrent, onFinished, isForced, onStarted, intervalDelay));

        IEnumerator _AddSkillsToQueue(IEnumerable<SkillQueueData> skillObjects, bool waitToFinishCurrent = true, Action onFinished = null, bool isForced = false, Action onStarted = null, float intervalDelay = 0)
        {
            var i = 0;
            foreach (var skillObject in skillObjects)
            {
                AddSkillToQueue(skillObject.caster, skillObject.skillObject, waitToFinishCurrent, i + 1 == skillObjects.Count() ? onFinished : null, isForced, i == 0 ? onStarted : null);
                yield return new WaitForSeconds(intervalDelay);
                i++;
            }
        }

        public SkillQueueData AddSkillToQueue(ISkillCaster caster, SkillObject skillObject, bool waitToFinishCurrent = true, Action onFinished = null, bool isForced = false, Action onStarted = null, bool firstInQueue = false)
            => AddSkillToQueue(new SkillQueueData(caster, skillObject, waitToFinishCurrent, onFinished, isForced, onStarted), firstInQueue);

        public SkillQueueData AddSkillToQueue(SkillQueueData skillQueueData, bool firstInQueue = false)
        {
            if (skillQueueData.caster.CurrenSkillState.HasFlag(ISkillCaster.SkillState.Executed | ISkillCaster.SkillState.Executing))
            {
                skillQueueData.onFinished?.Invoke();
                return null;
            }

            if (firstInQueue)
                skillObjectQueue.Insert(0, skillQueueData);
            else
                skillObjectQueue.Add(skillQueueData);
            CurrentState = State.Executing;

            if (executeCoroutine == null)
                executeCoroutine = StartCoroutine(StartQueue());

            return skillQueueData;
        }

        IEnumerator StartQueue()
        {
            while (skillObjectQueue.Count > 0 && (skillObjectQueue[0].isForced || BattleManager.Instance.PlayerTeam.IsAlive && BattleManager.Instance.EnemyTeam.IsAlive))
            {
                if (skip)
                    break;

                var skillQueueData = skillObjectQueue[0];

                if (skillQueueData.waitToFinishCurrent)
                    yield return StartQueue(skillQueueData);
                else
                {
                    StartCoroutine(StartQueue(skillQueueData));
                    skillRoutineListCount++;
                    skillQueueData.onFinished += () => skillRoutineListCount--;
                }

                skillObjectQueue.Remove(skillQueueData);
                yield return new WaitWhile(() => skillObjectQueue.Count == 0 && skillRoutineListCount > 0);
            }

            LastTargetReference = null;
            executeCoroutine = null;
            CurrentState = State.Idle;
            executeFinishCallback?.Invoke();
            executeFinishCallback = null;
            skillObjectQueue.Clear();
        }

        IEnumerator StartQueue(SkillQueueData skillQueueData)
        {
            skillQueueData.onStarted?.Invoke();
            ConditionalDataValues dataValues = new ConditionalDataValues();

            //if (LastTargetReference != skillQueueData.caster && LastTargetReference != null)
            //    yield return LastTargetReference.OnNewCasterSkillCasted(skillQueueData.skillObject);

            List<Func<List<Component>>> targetManagerData = null;
            skillQueueData.skillObject.GetTargetManagerData(skillQueueData.caster as Component, data => targetManagerData = data);

            yield return new WaitWhile(() => targetManagerData == null);
            yield return ExecuteSkillObject(skillQueueData, dataValues, targetManagerData);
            LastTargetReference = skillQueueData.caster;

            //yield return LastTargetReference.OnNewCasterSkillCasted(skillQueueData.skillObject);

            skillQueueData.onFinished?.Invoke();
        }

        IEnumerator ExecuteSkillObject(SkillQueueData skillQueueData, ConditionalDataValues dataValues, List<Func<List<Component>>> targetManagerData)
        {
            Debug.Log("ExecuteSkillObject");
            if (skillQueueData?.skillObject == null)
                yield break;
            var skillCaster = skillQueueData.caster as Component;
            if (skillCaster == null)
            {
                Debug.LogError($"[ExecuteSkillObject] {skillQueueData.caster} is not a component.");
                yield break;
            }
            var proceedNextSkillCard = false;
            
            SkillManager.ExecuteSkill(skillCaster, skillQueueData.skillObject, dataValues, targetManagerData, () => proceedNextSkillCard = true);


            yield return new WaitUntil(() => proceedNextSkillCard);
        }
    }

    [Serializable]
    public class SkillQueueData
    {
        public ISkillCaster caster;
        public SkillObject skillObject;
        public Action onStarted;
        public Action onFinished;
        public bool waitToFinishCurrent;
        public bool isForced;

        public SkillQueueData(ISkillCaster caster, SkillObject skillObject, bool waitToFinishCurrent = false, Action onFinished = null, bool isForced = false, Action onStarted = null)
        {
            this.caster = caster;
            this.skillObject = skillObject;
            this.onFinished = onFinished;
            this.waitToFinishCurrent = waitToFinishCurrent;
            this.isForced = isForced;
            this.onStarted = onStarted;
        }
    }
}