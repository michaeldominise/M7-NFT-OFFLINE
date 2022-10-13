using M7.GameData;
using System;
using System.Linq;
using UnityEngine;
using M7.Skill;
using System.Collections.Generic;
using System.Collections;
using Sirenix.OdinInspector;

namespace M7.GameRuntime
{
    public class TeamManager_Battle : BaseTeamManager<CharacterInstance_Battle>, IStatusEffectInstanceController, ISkillCaster
    {
        public enum State { Initializing, Ready, EndTurn }

        [SerializeField] Transform statusEffectInstanceContainer;
        [SerializeField] StatsInstance_Team statsInstance;
        [SerializeField] UIPrefManager_CharacterStats uiPrefManager_CharacterStats;
        [SerializeField] Transform deadCharactersContainer;

        public StatsInstance_Team StatsInstance => statsInstance;
        [ShowInInspector, ReadOnly] public bool IsAlive => ActiveCharacters.FirstOrDefault(x => x.IsAlive) != null;
        Transform IStatusEffectInstanceController.StatusEffectInstanceContainer => statusEffectInstanceContainer;
        [ShowInInspector, ReadOnly] public List<CharacterInstance_Battle> AliveCharacters => RawCharacters.Where(x => x != null && x.IsAlive).ToList();
        [ShowInInspector, ReadOnly] public TeamData TeamData { get; private set; }
        [ShowInInspector, ReadOnly] public List<StatusEffectInstance> ActiveStatusEffectInstances { get; private set; } = new List<StatusEffectInstance>();
        [ShowInInspector, ReadOnly] public List<StatusEffectInstance> StatusEffectInstanceLedger { get; private set; } = new List<StatusEffectInstance>();
        [ShowInInspector, ReadOnly] public virtual State CurrentState { get; set; } = State.Initializing;
        [ShowInInspector, ReadOnly] public int WaveIndex { get; private set; } = -1;
        [ShowInInspector] public bool HasNextWave => WaveIndex + 1 < TeamData?.Waves.Count;
        [ShowInInspector] public Transform DeadCharactersContainer => deadCharactersContainer;

        public void Init(TeamData teamData, Action onFinish)
        {
            TeamData = teamData;
            LoadNextWaveTeam(BattleManager.Instance.BattleSettings.WaveTransitionDelay, onFinish);
        }

        public void LoadNextWaveTeam(float delay, Action onFinish) => StartCoroutine(_LoadNextWaveTeam(delay, onFinish));
        IEnumerator _LoadNextWaveTeam(float delay, Action onFinish)
        {
            if (TeamData.Waves.Count == WaveIndex + 1)
            {
                onFinish?.Invoke();
                yield break;
            }

            WaveIndex++;
            CurrentState = State.Initializing;

            yield return new WaitForSeconds(delay);
            
            Init(TeamData.Waves[WaveIndex], () =>
            {
                CurrentState = State.Ready;
                onFinish?.Invoke();
            });
            StartCoroutine(BattleManager.Instance.UIWaveManager.InitSequence(delay));
            BattleManager.Instance.UIWaveManager.UpdateUI(BattleManager.Instance.EnemyTeam.WaveIndex, LevelManager.LevelData.TeamData.Waves.Count);
        }

        public override void Init(WaveData waveData, Action onFinish)
        {
            base.Init(waveData, onFinish);
            statsInstance.Init(this);
        }

        public override CharacterInstance_Battle SetCharacterPosition(int index, SaveableCharacterData saveableCharacter, Action onFinish)
        {
            CharacterInstance_Battle charInstance = null;

            charInstance = base.SetCharacterPosition(index, saveableCharacter, () =>
            {
                if (charInstance != null)
                    charInstance.UIBattle_CharacterStats = uiPrefManager_CharacterStats.CreateUI(charInstance);
                onFinish?.Invoke();
            });
            return charInstance;
        }

        void IStatusEffectInstanceController.OnStatusEffectInstanceLedgerUpdate(StatusEffectInstance updatedStatusEffectInstance, IStatusEffectInstanceController.UpdateType updateType) => statsInstance.OnStatusEffectInstanceLedgerUpdate(updatedStatusEffectInstance, updateType);
        
        public override void PostInit()
        {
            StatsInstance.Init(this);
            base.PostInit();
        }

        public IEnumerator OnPreSkillCasted(SkillObject skillObject, Func<List<Component>> getTargets) { yield break; }
        public IEnumerator OnNewCasterSkillCasted(SkillObject skillObject) { yield break; }
        public ISkillCaster.SkillState CurrenSkillState { get; set; }
        
        public bool ExecuteOnSpawn { get; set; }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}