using M7.GameData;
using M7.Skill;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace M7.GameRuntime
{
    public class WaveTransitionManager : MonoBehaviour
    {
        public static WaveTransitionManager Instance => BattleManager.Instance?.WaveTransitionManager;

        [SerializeField] float nextWaveDistance = 10;
        [SerializeField] Transform cameraContainer;
        [ShowInInspector, ReadOnly] Vector3 OffsetPosition { get; set; }

        public float NextWaveDistance => nextWaveDistance;
        TeamManager_Battle PlayerTeam => BattleManager.Instance.PlayerTeam;
        TeamManager_Battle EnemyTeam => BattleManager.Instance.EnemyTeam;

        public void Init()
        {
            OffsetPosition = cameraContainer.transform.position - PlayerTeam.transform.position;
        }

        public void NewWaveTransition(bool isMovingPlayerTeam, Action onFinish)
        {
            var startTime = Time.time;
            var movingTeam = isMovingPlayerTeam ? PlayerTeam : EnemyTeam;
            var destinationTeam = isMovingPlayerTeam ? EnemyTeam : PlayerTeam;
            destinationTeam.transform.position = cameraContainer.position + Vector3.right * nextWaveDistance - new Vector3(-OffsetPosition.x, OffsetPosition.y, OffsetPosition.z);

            var aliveCharacters = movingTeam.AliveCharacters;
            aliveCharacters.ForEach(x =>
            {
                if (x == null)
                    return;
                x.UIBattle_CharacterStats.ObjectFollower.enabled = false;
                x.MoveAnimation(true, 1);
            });

            movingTeam.transform.DOLocalMove(cameraContainer.position + Vector3.right * nextWaveDistance - new Vector3(OffsetPosition.x, OffsetPosition.y, OffsetPosition.z), BattleSceneSettings.Instance.WaveTransitionDuration).SetEase(Ease.Linear);
            cameraContainer.DOLocalMove(cameraContainer.position + (isMovingPlayerTeam ? Vector3.right : Vector3.left) * nextWaveDistance, BattleSceneSettings.Instance.WaveTransitionDuration).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    aliveCharacters.ForEach(x =>
                    {
                        if (x == null)
                            return;
                        x.UIBattle_CharacterStats.ObjectFollower.enabled = true;
                        x.MoveAnimation(false, 1);
                    });
                    onFinish?.Invoke();
                });
            
        }
    }
}