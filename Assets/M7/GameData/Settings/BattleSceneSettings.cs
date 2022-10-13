using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using M7.Tools;
using M7.Match;
using Sirenix.OdinInspector;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "BattleSceneSettings", menuName = "Assets/M7/GameData/Settings/BattleSceneSettings")]
    public class BattleSceneSettings : M7Settings
    {
        static BattleSceneSettings _Instance;
        public static BattleSceneSettings Instance
        {
            get
            {
                _Instance = _Instance ?? BattleManager.Instance?.BattleSettings;
#if UNITY_EDITOR
                if (_Instance == null)
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets("t:BattleSceneSettings");
                    if (guids.Length > 0)
                    {
                        var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                        _Instance = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleSceneSettings>(assetPath);
                    }
                }
#endif
                return _Instance;
            }
        }

        [SerializeField] int initialTeamSkillPoints = 3;
        [SerializeField] int maxTeamSkillPoints = 3;
        [SerializeField] int adventureTurnCoundownDuration = 30;
        [SerializeField] int pvpTurnCoundownDuration = 30;

        [Title("Intervals and Timing")]
        [SerializeField] float puzzleBoardOverlayDuration = 0.5f;//
        [SerializeField] float waveTransitionDelay = 1;//
        [SerializeField] float waveTransitionDuration = 3; //
        [SerializeField] float chargeToHeroIntervalPerOrb = 1;
        [SerializeField] float chargeToHeroDurationPerOrb = 1; //
        [SerializeField] float heroToEnemyAttackDelay = 1;
        [SerializeField] float enemyToHeroAttackDelay = 1;
        [SerializeField] float attackIntervalPerCharacter = 1;
        [SerializeField] float heroToEnemyTurnDelay = 1;
        [SerializeField] float enemyToHeroTurnDelay = 1;
        [SerializeField] float enemyTurnCountdownDelay = 1;

        [Title("Victory Screen")]
        [SerializeField] float victoryShowDelay = 1;
        [SerializeField] float victoryToRocketDelay = 1;
        [SerializeField] float movesToRocketTransformInterval = 1;
        [SerializeField] float movesToRocketTransformDuration = 1; 
        [SerializeField] float rocketStartExplosionDelay = 1;
        [SerializeField] float resultScreenShowDelay = 1;
          
        public int InitialTeamSkillPoints => initialTeamSkillPoints;
        public int MaxTeamSkillPoints => maxTeamSkillPoints;
        public int AdventureTurnCoundownDuration => adventureTurnCoundownDuration;
        public int PvpTurnCoundownDuration => pvpTurnCoundownDuration;
        public float PuzzleBoardOverlayDuration => puzzleBoardOverlayDuration;
        public float WaveTransitionDelay => waveTransitionDelay;
        public float WaveTransitionDuration => waveTransitionDuration;
        public float ChargeToHeroIntervalPerOrb => chargeToHeroIntervalPerOrb;
        public float ChargeToHeroDurationPerOrb => chargeToHeroDurationPerOrb;
        public float HeroToEnemyAttackDelay => heroToEnemyAttackDelay;
        public float EnemyToHeroAttackDelay => enemyToHeroAttackDelay;
        public float AttackIntervalPerCharacter => attackIntervalPerCharacter;
        public float HeroToEnemyTurnDelay => heroToEnemyTurnDelay;
        public float EnemyToHeroTurnDelay => enemyToHeroTurnDelay;
        public float EnemyTurnCountdownDelay => enemyTurnCountdownDelay;
        public float VictoryShowDelay => victoryShowDelay;
        public float VictoryToRocketDelay => victoryToRocketDelay;
        public float MovesToRocketTransformInterval => movesToRocketTransformInterval;
        public float MovesToRocketTransformDuration => movesToRocketTransformDuration;
        public float RocketStartExplosionDelay => rocketStartExplosionDelay;
        public float ResultScreenShowDelay => resultScreenShowDelay;
    }
}
