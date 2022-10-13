using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using M7.FX;
using M7.GameData;
using M7.GameRuntime;
using M7.Match;
using M7.Skill;
using UnityEngine;

public class ParticleAttractorManager : MonoBehaviour
{
    public static ParticleAttractorManager Instance { get; private set; }

    [SerializeField] GameObject particlePref;
    [SerializeField] CellType omniTile;
    [SerializeField] Vector2 randomDelayRange = new Vector2(0, 0.5f);
    [SerializeField] float initialDelay = 0.5f;
    [SerializeField] float particleInterval => BattleSceneSettings.Instance.ChargeToHeroIntervalPerOrb;
    int coroutineQueueCount;

    public bool IsAnimating => coroutineQueueCount > 0;
    public static Action<MatchGridCell> OnParticleCharged;

    void Awake () {
        Instance = this;

    }

    public void Spawn(Dictionary<SkillEnums.ElementFilter, List<MatchGridCell>> cellElementGroup, Action onFinish)
    {
        var characterInstances = BattleManager.Instance.IsGameDone ? BattleManager.Instance.PlayerTeam.AliveCharacters : BattleManager.Instance.ActiveTeam.AliveCharacters;

        foreach (var cellElementlist in cellElementGroup)
        {
            for (int i = 0; i < cellElementlist.Value.Count; i++)
            {
                var addToCollection = true;
                var charElementGroup = characterInstances.Where(x => x.Element.ElementType == cellElementlist.Key);
                foreach (var characterInstance in charElementGroup)
                {
                    MatchGridCell cell = cellElementlist.Value[i];
                    if (!cell.ShowParticleAttactor)
                        continue;
                    var chargeValue = characterInstance.StatsInstance.Attack + Mathf.Lerp(0, characterInstance.StatsInstanceBattle.MatchBoardDamage, (i + 1f) / cellElementlist.Value.Count);
                    StartCoroutine(SpawnParticle(cell, characterInstance, initialDelay + i * particleInterval, chargeValue, i, () => 
                        {
                            if (addToCollection)
                            {
                                BattleManager.Instance.GaianiteCollectionManager.AddCollection();
                                addToCollection = false;
                            }
                            if(coroutineQueueCount == 0)
                                onFinish?.Invoke();
                        }));
                }
            }
        }
    }


    IEnumerator SpawnParticle(MatchGridCell matchGridTCell, CharacterInstance_Battle characterInstance, float delay, float addValue, int index, Action onFinish)
    {
        coroutineQueueCount++;
        var tileWorldPos = matchGridTCell.transform.position - Vector3.up * 0.75f;
        var charWorldPos = characterInstance.MainSpineInstance.SpineOffsetManager.BodyOffset.position;
        var color = matchGridTCell.CellTypeContainer.CellType.BaseColor;
        yield return new WaitForSeconds(delay);

        var particle = ParticleWorldManager.Instance.SpawnVFX<ParticleAttractor>(particlePref, tileWorldPos, ParticleWorldManager.CameraType.Puzzle, ParticleWorldManager.CameraType.Particle);
        particle?.Init(color, ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(charWorldPos, ParticleWorldManager.CameraType.World), BattleSceneSettings.Instance.ChargeToHeroDurationPerOrb);

        yield return new WaitForSeconds(BattleSceneSettings.Instance.ChargeToHeroDurationPerOrb / 2);
        MasterAudio.PlaySound("RPG - AttackCharge");
        yield return new WaitForSeconds(BattleSceneSettings.Instance.ChargeToHeroDurationPerOrb / 2);
        OnParticleCharged?.Invoke(matchGridTCell);
        if (!BattleManager.Instance.IsGameDone && (BattleManager.Instance.IsPlayerTeamObject(characterInstance) || LevelManager.LevelData.GameMode != M7.GameData.LevelData.GameModeType.Adventure))
            characterInstance.UIBattle_CharacterStats.AttackChargeText(addValue, Mathf.Floor((index + 1) / 2f));

        coroutineQueueCount--;
        onFinish?.Invoke();
    }
}
