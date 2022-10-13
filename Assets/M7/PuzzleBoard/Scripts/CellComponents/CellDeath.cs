/*
 * TileDeath.cs
 * Author: Cristjan Lazar
 * Date: Oct 30, 2018
 */

using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Serialization;

using DG.Tweening;
using PathologicalGames;
using M7.GameRuntime;

namespace M7.Match {

    /// <summary>
    /// On tile death animation handler.
    /// </summary>
    public class CellDeath : MonoBehaviour {

        [SerializeField, FormerlySerializedAs("matchGridTile")] MatchGridCell matchGridCell;
        [SerializeField] Transform target;
        [SerializeField] private float hideDelay = 1;
        //[SerializeField] private float destroyDelay = 1;
        Coroutine playCoroutine;

        MatchGrid ActiveGrid => PuzzleBoardManager.Instance.ActiveGrid;

        //Tweener tweener;

        //public bool IsPlaying { get { return tweener.IsPlaying(); } }
        public bool IsPlaying { get { return playCoroutine != null; } }

        public void Rewind () {
            //tweener.Rewind();
            target.gameObject.SetActive(true);
        }

        public void Play () {
            //tweener.Play();
            if (playCoroutine != null)
                StopCoroutine(playCoroutine);

            if (gameObject.activeInHierarchy)
                playCoroutine = StartCoroutine(PlayDelay());
            else
                Die();
        }
        
        IEnumerator PlayDelay()
        {
            yield return new WaitForSeconds(hideDelay);
            target.gameObject.SetActive(false);
            Die();
        }

        public void Die(float delay = 0)
        {
            MatchGridCellSpawner.Instance.Despawn(matchGridCell, delay);
            playCoroutine = null;
            if (matchGridCell.CellTypeContainer.CellType.ElementType == Skill.SkillEnums.ElementFilter.Special || matchGridCell.CellTypeContainer.CellType.ElementType == Skill.SkillEnums.ElementFilter.Special || matchGridCell.CellTypeContainer.CellType.ElementType == Skill.SkillEnums.ElementFilter.Blockers)
                return;
            BattleManager.Instance.GaianiteCollectionManager.ActivateTile(matchGridCell);
        }

        public void Pause () {
            //tweener.Pause();
        }

        #region Unity event methods
        private void Awake() {
    //        tweener = transform.DOScale(0f, duration)
				//.SetEase(Ease.Linear)
    //            .SetAutoKill(false)
    //            .Pause();
        }
        #endregion

    }

}
