/*
 * TileDeath.cs
 * Author: Cristjan Lazar
 * Date: Oct 18, 2018
 */

using UnityEngine;
using UnityEngine.Events;

using PathologicalGames;
using System.Collections;
using Sirenix.OdinInspector;
using M7.Skill;

namespace M7.Match {

    /// <summary>
    /// Tile component responsible for handling tile health and despawning.
    /// </summary>
    public class CellHealth : MonoBehaviour {

        // TODO Move this to a static utility class
        private const string POOL_NAME = "TilePool";

        private static SpawnPool pool;

        [SerializeField] private MatchGridCell matchGridCell;
        [SerializeField] private Transform target;
        [SerializeField] private int health = 1;

        public int Health => health;
        public bool IsDead { get { return health < 1; } }

        public UnityEvent OnDeath;

        public UnityEvent OnDamaged;

        public void SetHealth (int value) {
            health = value;
        }

        public void AddHealth (int value) {
            health += value;
        }

        /// <summary>
        /// Deak damage to the tile. Only triggers OnDamaged event if damage > 0.
        /// </summary>
        [Button]
        public void DealDamage (int damage) {
            if (damage < 1)
                return;

            health -= damage;

            if (health < 0)
                health = 0;  


            if (IsDead && OnDeath != null)
                OnDeath.Invoke();
            else if (OnDamaged != null)
                OnDamaged.Invoke();
        }

        private void Kill () {
            DealDamage(health);
        }

        private void Despawn () {
            if (!pool.IsSpawned(target))
                return;

            pool.Despawn(target, pool.transform);

            if (OnDeath != null)
                OnDeath.Invoke();
        }

        #region Unity event methods

        private void Start () {
            pool = PoolManager.Pools[POOL_NAME];
        }

        #endregion

    }

}
