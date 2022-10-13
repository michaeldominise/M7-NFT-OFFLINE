/*
 * MatchCell.cs
 * Author: Cristjan Lazar
 * Date: Oct 10, 2018
 */

using System;
using System.Collections;
using M7.Match.PlaymakerActions;
using Gamelogic.Grids;
using Sirenix.OdinInspector;
using UnityEngine;
using M7.Skill;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using M7.PuzzleBoard.Scripts.Booster;
using M7.PuzzleBoard.Scripts.SpecialTiles;
using M7.GameRuntime;
using UnityEngine.Events;
using UnityEngine.Serialization;
using DG.Tweening;

namespace M7.Match
{
    /// <summary>
    /// Component wrangler for MatchGrid cells.
    /// </summary>
    public class MatchGridCell : MonoBehaviour, IStatusEffectInstanceController, ISkillCaster
    {
        public enum CellState { Active, Locked, Damaged, Dead }

        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected SpriteRenderer subSpriteRenderer;
        [SerializeField, FormerlySerializedAs("specialTileComboIndicator")] protected ParticleSystem specialTileComboIndicator;
        [SerializeField] protected SpriteRenderer iconSpriteRenderer;
        [SerializeField] protected SpriteRenderer glowSpriteRenderer;
        [SerializeField, FormerlySerializedAs("tileTypeContainer")] protected CellTypeContainer cellTypeContainer;
        [SerializeField, FormerlySerializedAs("tileMotor")] protected CellMotor cellMotor;
        [SerializeField, FormerlySerializedAs("tileHealth")] protected CellHealth cellHealth;
        [SerializeField, FormerlySerializedAs("tileDeath")] protected CellDeath cellDeath;
        [SerializeField] protected Transform statusEffectInstanceContainer;
        public Transform container;
        
        [SerializeField] private bool isSpawned;
        [SerializeField] private bool isInOmniGroup;
        public bool IsSpawned => isSpawned;

        RectPoint currentRectPoint;
        public Transform StatusEffectInstanceContainer => statusEffectInstanceContainer;
        public List<StatusEffectInstance> ActiveStatusEffectInstances { get; } = new List<StatusEffectInstance>();
        public List<StatusEffectInstance> StatusEffectInstanceLedger { get; } = new List<StatusEffectInstance>();

        public RectPoint CurrentRectPoint
        {
            get => currentRectPoint;
            set
            {
                currentRectPoint = value;
                rectPointLabel.text = currentRectPoint.ToString();
                name = $"{cellTypeContainer.CellType.TileName} {currentRectPoint}";
            }
        }
        [ShowInInspector] public Vector2Int CurrentRectPointDisplay => new Vector2Int(CurrentRectPoint.X, CurrentRectPoint.Y);

        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public SpriteRenderer SubSpriteRenderer => subSpriteRenderer;
        public SpriteRenderer IconSpriteRenderer => iconSpriteRenderer;
        public CellTypeContainer CellTypeContainer => cellTypeContainer;
        public CellMotor CellMotor => cellMotor;
        public CellHealth CellHealth => cellHealth;
        public CellDeath CellDeath => cellDeath;

        [SerializeField] TextMesh rectPointLabel;
        [SerializeField] string id;
        public static Func<string, bool> OnAnyPicked;
        public string Id { get { return id; } set { id = value; } }
        
        CellState currentCellState;

        [ShowInInspector, ReadOnly]
        public CellState CurrentCellState
        {
            get { return currentCellState; }
            set
            {
                if (currentCellState == value)
                    return;

                currentCellState = value;

                onTileStateChanged?.Invoke(currentCellState);
                OnCellStateChangedMethod(currentCellState);
            }
        }

        public Action<CellState> onTileStateChanged;

        public bool IsInteractible
        {
            get
            {
                return CellTypeContainer.CellType.IsTouchable &&
                                        !cellMotor.IsMoving &&
                                        !cellHealth.IsDead &&
                                        (SecondaryCell?.CellTypeContainer.CellType.IsMatchable ?? true);
            }
        }

        public CellType LastTileType { get; set; }
        public SkillObject TouchSkillObject => CellTypeContainer.CellType.TouchSkillOject;
        [ShowInInspector, ReadOnly] public bool ShowParticleAttactor { get; set; } = true;
        [ShowInInspector, ReadOnly] public bool PlayDeathAnim { get; set; } = true;
        public bool IsSkillExecuted { get; set; }
        [ShowInInspector, ReadOnly] public MatchGridCell SecondaryCell => CellTypeContainer.CellType != null && CellTypeContainer.CellType.TileGridLocation == SkillEnums.CellGridLocation.Main ? PuzzleBoardManager.Instance?.SecondaryGrid.Grid[CurrentRectPoint] : null;
        public static List<MatchGridCell> matchGridToExecute = new List<MatchGridCell>();

        public void ShowDisplay(bool value) => container.gameObject.SetActive(value);

        public void SelectTile(bool isSelected)
        {
            if (isSelected)
            {
                transform.SetAsLastSibling();
                container.localPosition = Vector3.forward * -0.1f;
            }
            else
                container.localPosition = Vector3.zero;
        }

        private void Start()
        {
            cellHealth.OnDamaged.AddListener(() => { CurrentCellState = CellState.Damaged; });
            cellHealth.OnDeath.AddListener(() => { CurrentCellState = CellState.Dead; });
        }

        /// <summary>
        /// Initialize the tile with a given tile type.
        /// </summary>
        [Button]
        public virtual void Initialize(CellType tileType)
        {
            CurrenSkillState = ISkillCaster.SkillState.Idle;
            
            // enable sprite renderer
            SetSpriteRenderer(true);

            // reset sprite renderer for special cube group
            SetSpecialTileComboGlow(false);
            
            // Reset spawned flag
            CellMotor.ResetSpawnFlag = () =>
            {
                SetSpawnFlag(false);
            };
            
            LastTileType = cellTypeContainer.CellType;
            CurrentCellState = CellState.Active;
            cellTypeContainer.CellType = tileType;

            name = $"{cellTypeContainer.CellType.TileName} {currentRectPoint}";
            spriteRenderer.sprite = tileType.IsVisible ? tileType.Sprite : null;
            subSpriteRenderer.sprite = tileType.IsVisible ? tileType.SubSprite : null;
            ShowParticleAttactor = true;
            PlayDeathAnim = true;
            IsSkillExecuted = false;
            ShowDisplay(true);
            ResetIcon();

            if (CellHealth != null)
                CellHealth.SetHealth(tileType.StartingHp);

            if (CellDeath != null)
                CellDeath.Rewind();

            RemoveCellStatusEffect();
        }

        public void SetSpriteRenderer(bool isEnabled, float delay = 0) => StartCoroutine(_SetSpriteRenderer(isEnabled, delay));
        IEnumerator _SetSpriteRenderer(bool isEnabled, float delay)
        {
            yield return new WaitForSeconds(delay);
            spriteRenderer.enabled = isEnabled;
            subSpriteRenderer.enabled = isEnabled;
            iconSpriteRenderer.enabled = isEnabled;
        }
        
        public void ResetIcon() => iconSpriteRenderer.sprite = cellTypeContainer.CellType.IconSprite;
        public void ShowGlow(bool isShow) => glowSpriteRenderer.gameObject.SetActive(isShow);
        public bool Matches(MatchGridCell tile) => Matches(tile.CellTypeContainer.CellType);
        public bool Matches(CellType tileType) => CellTypeContainer.Matches(tileType);
        public void UpdateIcon(Sprite sprite) => iconSpriteRenderer.sprite = sprite;
        public bool CanDamage(CellType_DamageCondition.DamageData damageData) => (SecondaryCell == null || SecondaryCell.cellHealth.IsDead) && CellTypeContainer.CellType.CanDamage(damageData);

        public bool TryDealDamage(CellType_DamageCondition.DamageData damageData, int damageValue)
        {
            var isDamaged = false;
            if (CanDamage(damageData))
            {   
                cellHealth.DealDamage(damageValue);
                isDamaged = true;
            }
            if (SecondaryCell != null && SecondaryCell.CanDamage(new CellType_DamageCondition.DamageData(SecondaryCell, damageData.chain, damageData.cellDestroyType, this)))
                SecondaryCell.cellHealth.DealDamage(damageValue);
            return isDamaged;
        }

        public virtual void UpdateSubIconBaseOnHp()
        {
            switch (CellTypeContainer.CellType.UpdateSprite)
            {
                case SkillEnums.CellUpdateSprite.None:
                    break;
                case SkillEnums.CellUpdateSprite.UpdateSubSprite:
                    subSpriteRenderer.sprite = CellTypeContainer.CellType.HpBasedIconSprite[CellHealth.Health - 1] ?? subSpriteRenderer.sprite;
                    break;
                case SkillEnums.CellUpdateSprite.UpdateSprite:
                    spriteRenderer.sprite = CellTypeContainer.CellType.HpBasedIconSprite[CellHealth.Health - 1] ?? spriteRenderer.sprite;
                    break;
                case SkillEnums.CellUpdateSprite.UpdateIconSprite:
                    iconSpriteRenderer.sprite = CellTypeContainer.CellType.HpBasedIconSprite[CellHealth.Health - 1] ?? iconSpriteRenderer.sprite;
                    break;
            }
        }

        public void SetSpecialTileComboGlow(bool isEnabled) => specialTileComboIndicator.gameObject.SetActive(!BattleManager.Instance.IsGameDone && isEnabled);
        
        public void OnCellStateChangedMethod(CellState state)
        {
            switch (state)
            {
                case CellState.Active:
                    CellDeath.Rewind();
                    break;
                case CellState.Damaged:
                    UpdateSubIconBaseOnHp();
                    CurrentCellState = CellState.Active;
                    if (PlayDeathAnim)
                        CellDeathManager.Instance.PlayDeathParticle(this);

                    break;
                case CellState.Dead:
                    SetSpawnFlag(true);
                    var matchGrid = CellTypeContainer.CellType.TileGridLocation == SkillEnums.CellGridLocation.Main ? PuzzleBoardManager.Instance.ActiveGrid : PuzzleBoardManager.Instance.SecondaryGrid;
                    matchGrid.Grid[CurrentRectPoint] = null;
                    if (TouchSkillObject)
                        ExecuteSkill(true);
                    else if (PlayDeathAnim)
                    {
                        CellDeathManager.Instance.PlayDeathParticle(this);
                        CellDeath.Play();
                    }
                    else
                        CellDeath.Die(1);
                    break;
                case CellState.Locked:
                    break;
            }
        }

        private void SetSpawnFlag(bool pIsSpawned) => isSpawned = pIsSpawned;

        public void RemoveCellStatusEffect()
        {
            foreach (var statusEffectInstance in StatusEffectInstanceLedger)
                Destroy(statusEffectInstance.gameObject);
            StatusEffectInstanceLedger.Clear();
        }


        public bool ExecuteSkill(bool isForced = false)
        {
            if (IsSkillExecuted || !TouchSkillObject)
                return false;
            SetSpriteRenderer(false);
            IsSkillExecuted = true;
            SkillQueueManager.Instance.AddSkillToQueue(this, TouchSkillObject, 
                waitToFinishCurrent: false, 
                onFinished: () =>
                {
                    CellDeath.Play();
                    MatchGridCellSpawner.Instance.RefreshGrid();
                }, 
                isForced: isForced, 
                firstInQueue: true);

            return true;
        }

        public void OnStatusEffectInstanceLedgerUpdate(StatusEffectInstance updatedStatusEffectInstance, IStatusEffectInstanceController.UpdateType updateType) { }
        public IEnumerator OnPreSkillCasted(SkillObject skillObject, Func<List<Component>> getTargets) { yield break; } 
        public IEnumerator OnNewCasterSkillCasted(SkillObject skillObject) { yield break; }
        public ISkillCaster.SkillState CurrenSkillState { get; set; }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}