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
using UnityEngine.UI;

namespace M7.Match
{
    /// <summary>
    /// Component wrangler for MatchGrid tiles.
    /// </summary>
    public class MatchGridCell_LightTube : MatchGridCell
    {
        [SerializeField]
        public class LightTubeData
        {
            public SkillEnums.ElementFilter elementType;
            public Sprite lightBulbOn;
            public Sprite lightBulbOff;

            bool isOn;
            [ShowInInspector, ReadOnly]
            public bool IsOn
            {
                get => isOn;
                set
                {
                    isOn = value;
                    UpdateSprite();
                }
            }
            [ShowInInspector, ReadOnly] public SpriteRenderer LightBulb { get; set; }

            public void UpdateSprite() => LightBulb.sprite = IsOn ? lightBulbOn : lightBulbOff;
        }

        [SerializeField] SpriteRenderer[] lightBulbs;
        [SerializeField] LightTubeData[] lightBulbSprites;
        [ShowInInspector, ReadOnly] List<LightTubeData> lightElements = new List<LightTubeData>();

        [Button]
        public override void Initialize(CellType cellType)
        {
            base.Initialize(cellType);
            for (var x = 0; x < lightBulbs.Length; x++)
            {
                var elementType = (SkillEnums.ElementFilter)(1 << UnityEngine.Random.Range(0, 5));
                var lightTubeData = lightBulbSprites.FirstOrDefault(x => x.elementType == elementType);
                if (lightTubeData == null)
                    continue;

                lightTubeData.LightBulb = lightBulbs[x];
                lightTubeData.UpdateSprite();
                lightElements.Add(lightTubeData);
            }
        }

        [Button]
        public bool Damage(SkillEnums.ElementFilter elementType)
        {
            foreach(var lightElement in lightElements)
                if(lightElement.elementType == elementType && !lightElement.IsOn)
                {
                    lightElement.IsOn = true;
                    return true;
                }

            return false;
        }
    }
}