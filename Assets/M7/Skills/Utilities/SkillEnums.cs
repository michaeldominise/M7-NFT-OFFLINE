using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public static class SkillEnums
    {
        public enum TargetCharacterStats
        {
            CurrentHp,
            MaxHp,
            Attack, 
            Defense,
            Passion,
            Luck,
            Durability,
            MatchBoardDamage,
            AttackTurn,
            DamageReduction,
            Stun,
        }

        public enum TargetTeamStats 
        { 
            SkillPoints
        }

        [Flags]
        public enum TeamFilter
        {
            None = 0,
            Ally = 1 << 0,
            Opponent = 1 << 1,
            All = (1 << 2) - 1
        }

        [Flags]
        public enum SelectionFilter
        {
            None = 0,
            Caster = 1 << 0,
            Others = 1 << 1,
            All = (1 << 2) - 1
        }

        public enum StatusValueSource
        {
            Caster,
            Target,
        }

        [Flags]
        public enum HpStatusFilter
        {
            None = 0,
            Alive = 1 << 0,
            Dead = 1 << 1,
            All = 3
        }

        [Flags]
        public enum ElementFilter
        {
            None = 0,
            Fire = 1 << 0,
            Light = 1 << 1,
            Water = 1 << 2,
            Earth = 1 << 3,
            Dark = 1 << 4,
            Wind = 1 << 5,
            BasicElements = Fire | Light | Water | Earth | Dark | Wind,
            Special = 1 << 6,
            Blockers = 1 << 7,
            All = (1 << 8) - 1
        }

        [Flags]
        public enum SpecialTilesEnum
        {
            None = 0,
            RocketHorizontal = 1 << 0,
            RocketVertical = 1 << 1,
            Bomb = 1 << 2,
            Prism = 1 << 3
        }
        
        [Flags]
        public enum RarityFilter
        {
            None = 0,
            Common = 1 << 0,
            Uncommon = 1 << 1,
            Rare = 1 << 2,
            Epic = 1 << 3,
            Legendary = 1 << 4,
            All = (1 << 5) - 1
        }

        [Flags]
        public enum FormationGroupFilter
        {
            None = 0,
            FrontLine = 1 << 0,
            Backline = 1 << 1,
            All = 3
        }

        [Flags]
        public enum EventTrigger
        {
            None,
            Execute = 1 << 1,
            NewRound = 1 << 2,
            StartTurnCaster = 1 << 4,
            EndTurnCaster = 1 << 5,
            StartTurnTarget = 1 << 6,
            EndTurnTarget = 1 << 7,
            OnAttack = 1 << 8,
            OnDefend = 1 << 9,
            All = (1 << 10) - 1,
        }

        [Flags]
        public enum SkillActionTigger
        {
            None,
            Execute = 1 << 1,
            Expire = 1 << 2,
        }

        public enum ComputationType
        {
            None,
            Multiply,
            Increase,
            Decrease,
            IncreaseMultiply,
            DecreaseMultiply,
        }

        public enum SkillTransitionType
        {
            None,
            MoveToTarget,
            InPlace,
            BattleCenter
        }

        public enum SkillAnimationType
        {
            Attack,
            Skill,
            SkillLegendary,
            None,
        }
        
        public enum SkillType
        {
            Attack,
            Defense,
            Skill
        }

        [Flags]
        public enum MatchGridTilePatternFilter
        {
            None = 0,
            Horizontal = 1 << 0,
            Vertical = 1 << 1,
            DiagonalLeftUpToRightDown = 1 << 2,
            DiagonalLeftDownToRightUp = 1 << 3,
            All = (1 << 4) - 1
        }

        [Flags]
        public enum MatchGridTileSelectionFilter
        {
            None = 0,
            Caster = 1 << 0,
            Others = 1 << 1,
            All = (1 << 3) - 1
        }

        [Flags]
        public enum TargetMatchGridTileStats
        {
            TileGeneration,
            MaxHp,
            Attack,
            Defense,
            Mining,
            Luck,
            Durability,
            MatchBoardDamage,
        }
        
        public enum OverdriveCriteria
        {
            AttackAmountCondition,
            GotHitAmountCondition,
            HpPercentCondition,
            GemMatchCondition
        }

        public enum CellGridLocation
        {
            Main,
            Secondary
        }

        [Flags]
        public enum CellDestroyType
        {
            None,
            Normal = 1 << 0,
            SpecialItem = 1 << 1,
            Booster = 1 << 2,
            All = (1 << 3) - 1,
        }

        public enum CellUpdateSprite
        {
            None = 0,
            UpdateSprite = 1,
            UpdateSubSprite = 2,
            UpdateIconSprite = 3,
        }

        public enum BlockerType
        {
            None = 0,
            Box = 1,
            Drone = 2,
            Barrier = 3,
            Cage = 4,
            LightTube = 5,
            Obstacle = 6,
            Signal = 7,
            Switch = 8
        }
    }
}
