using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using M7.GameData;
using System;
using System.Security.Principal;
using Sirenix.OdinInspector;
using M7.Skill;
using M7.FX;
using DG.Tweening;
using M7.GameRuntime.Scripts.Managers.BattleScreen;

namespace M7.GameRuntime
{
    public class CharacterSelectionTargetManager : MonoBehaviour
    {
        public static CharacterSelectionTargetManager Instance => BattleManager.Instance?.CharacterSelectionTargetManager;

        [SerializeField] CanvasGroup overlay;

        [SerializeField] private TargetCursor _targetCursor;

        public List<CharacterInstance_Battle> CurrentSelectedTargets { get; set; } = new List<CharacterInstance_Battle>();
        [ShowInInspector, ReadOnly] CharacterInstance_Battle HoveredCharacter;
        [ShowInInspector, ReadOnly] public CharacterInstance_Battle PreSelectedCharacter;
        Coroutine selectTargetCoroutine;
        public bool cancelTarget;
        public bool IsActive => selectTargetCoroutine != null;

        public void StartSelectTargets(Component caster, TargetManager_CharacterInstance targetManager_CharacterInstance, Action onFinish)
        {
            #if UNITY_WEBGL
            // activate target cursor
            _targetCursor.SetCursor();
            #endif
            
            BattleMessageManager.Instance.Show("Select target.", -1);
            onFinish += () =>
            {
                #if UNITY_WEBGL
                // deactivate target cursor
                _targetCursor.UnsetCursor();
                #endif
                BattleMessageManager.Instance.Hide();
            };

            if (selectTargetCoroutine != null || targetManager_CharacterInstance == null)
            {
                onFinish?.Invoke();
                return;
            }

            if (TurnManager.Instance.CurrentState == TurnManager.State.EnemyTurn && LevelManager.GameMode == LevelData.GameModeType.Adventure)
            {
                CurrentSelectedTargets = targetManager_CharacterInstance.GetTargets(caster).Select(x => x as CharacterInstance_Battle).ToList();
                onFinish?.Invoke();
                return;
            }
            else
                selectTargetCoroutine = StartCoroutine(_StartSelectTargets(caster, targetManager_CharacterInstance, onFinish));
        }

        IEnumerator _StartSelectTargets(Component caster, TargetManager_CharacterInstance targetManager_CharacterInstance, Action onFinish)
        {
            CurrentSelectedTargets = null;
            HoveredCharacter = null;
            PreSelectedCharacter = null;

            var filteredTargets = targetManager_CharacterInstance.GetTargets(caster, true).Select(x => x as CharacterInstance_Battle).ToList();
            SetUISelection(filteredTargets, true);
            while (true)
            {
                    if (PreSelectedCharacter || cancelTarget)
                        break;
                    if (HoveredCharacter && filteredTargets.Contains(HoveredCharacter))
                    {
                        CurrentSelectedTargets = GetNearbyCharacters(HoveredCharacter, filteredTargets, targetManager_CharacterInstance.TargetCount);
                        // CurrentSelectedTargets.ForEach(x => x.AnimateCast());
                        CurrentSelectedTargets.ForEach(x => x.ShowTarget());
                    }

                    yield return null;
            }

            SetUISelection(filteredTargets, false);
            if (PreSelectedCharacter && filteredTargets.Contains(PreSelectedCharacter))
                CurrentSelectedTargets = GetNearbyCharacters(PreSelectedCharacter, filteredTargets, targetManager_CharacterInstance.TargetCount);
            
            selectTargetCoroutine = null;
            onFinish?.Invoke();
        }

        public IEnumerator CancelTarget()
        {
            cancelTarget = true;
            
            CurrentSelectedTargets?.ForEach(target => target?.HideTarget());
            CurrentSelectedTargets = null;
            HoveredCharacter = null;
            PreSelectedCharacter = null;

            yield return new WaitUntil(() => cancelTarget = true);

            cancelTarget = false;
            BattleMessageManager.Instance.Hide();
            _targetCursor.UnsetCursor();
        }
        public void SetUISelection(List<CharacterInstance_Battle> filteredTargets, bool isShow)
        {
            BattleCameraHandler.Instance.SelectionCamera.gameObject.SetActive(isShow);
            if (isShow)
            {
                filteredTargets.ForEach(x => ParticleWorldManager.SetLayerRecursively(x.gameObject, BattleCameraHandler.Instance.selectionLayer));
                overlay.gameObject.SetActive(true);
                overlay.DOFade(1, 0.5f);
            }
            else
            {
                overlay.alpha = 0;
                overlay.gameObject.SetActive(false);
                filteredTargets.ForEach(x => ParticleWorldManager.SetLayerRecursively(x.gameObject, BattleCameraHandler.Instance.worldLayer));
            }
        }

        public List<CharacterInstance_Battle> GetNearbyCharacters(CharacterInstance_Battle characterInstance, List<CharacterInstance_Battle> filteredTargets, int targetCount)
        {
            var findIndex = filteredTargets.FindIndex(x => x == characterInstance);
            var result = new List<CharacterInstance_Battle>();
            for (var x = 0; x < filteredTargets.Count && x < targetCount && findIndex >= 0; x++)
            {
                var target = filteredTargets[(int)Mathf.Repeat(x + findIndex, filteredTargets.Count)];
                result.Add(target);
            }
            return result;
        }

        public void SetPreSelectedCharacter(CharacterInstance_Battle characterInstance_Battle)
        {
            PreSelectedCharacter = characterInstance_Battle;
            _targetCursor.UnsetCursor();
        }
        public void SetHoveredCharacter(CharacterInstance_Battle characterInstance_Battle) => HoveredCharacter = characterInstance_Battle;

        public void UnsetHoveredCharacter(CharacterInstance_Battle characterInstanceBattle)
        {
            HoveredCharacter = null;
            CurrentSelectedTargets?.ForEach(target => target?.HideTarget());
            
            //if(BattleManager.Instance.CardManager.CurrentState == CardManager.InHandState.Selected)
            //    _targetCursor.SetCursor();
        }
    }
}

