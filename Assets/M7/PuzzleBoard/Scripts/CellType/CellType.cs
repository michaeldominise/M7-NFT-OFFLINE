/*
 * TileType.cs
 * Author: Cristjan Lazar
 * Date: Oct 8, 2018
*/

using UnityEngine;
using Sirenix.OdinInspector;
using System;
using M7.Skill;
using System.Collections.Generic;
using Gamelogic.Grids;
using System.Linq;

namespace M7.Match {
    public class CellType : MonoBehaviour
    {
        private const string anyTileAssetName = "AnyTile";

        private static CellType any;

        public static CellType Any {
            get {
                if (any == null)
                    any = Resources.Load<CellType>(anyTileAssetName);

                return any;
            }
        }

        public enum StaticEnum { NotStatic, StaticNotPssable, StaticPassable }

        [SerializeField] private string tileName;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Vector2Int tileDimension = new Vector2Int(1, 1);
        [PreviewField(50f, ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite sprite;
        [PreviewField(50f, ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite subSprite;
        [PreviewField(50f, ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private Sprite[] hpBasedIconSprite;
        [SerializeField] private bool isTouchable;
        [SerializeField] private StaticEnum staticType;
        [SerializeField] private bool isMatchable = true;
        [SerializeField] private bool isVisible = true;
        [SerializeField] private bool isDestroyable = true;
        [SerializeField] private int startingHp;
        [SerializeField] private List<CellType_DamageCondition> damageConditions;
        //[SerializeField] private Color subBaseColor = Color.white;
        [SerializeField] private SkillEnums.ElementFilter elementType;
        [SerializeField] private Color baseColor = Color.white;
        [SerializeField] private bool shuffable = true;
        [SerializeField] private SkillEnums.CellGridLocation gridLocation;

        [ShowIf("@elementType.HasFlag(SkillEnums.ElementFilter.Special)")] 
        [SerializeField] private SkillEnums.SpecialTilesEnum specialTileCategory;
        
        [SerializeField] private SkillObject touchSkillOject;
        [SerializeField] private SkillEnums.CellUpdateSprite updateSprite;
        [SerializeField] private SkillEnums.BlockerType blockerType;

        public string TileName { get { return tileName; } set { tileName = value; } }

        //public int MatchId { get { return matchId; } set { matchId = value; } }

        public int MatchValue => (int)elementType;
        public GameObject Prefab { get { return prefab; } set { prefab = value; } }
        public Vector2Int TileDimension { get { return tileDimension; } set { tileDimension = value; } }
        public Sprite Sprite { get { return sprite; } set { sprite = value; } }
        public Sprite SubSprite { get { return subSprite; } set { subSprite = value; } }
        public Sprite IconSprite { get { return iconSprite; } set { iconSprite = value; } }
        public Sprite[] HpBasedIconSprite { get { return hpBasedIconSprite; } set { hpBasedIconSprite = value; } }
        public bool IsTouchable { get { return isTouchable; } set { isTouchable = value; } }
        public StaticEnum StaticType { get { return staticType; } set { staticType = value; } }
        public bool IsMatchable { get { return isMatchable; } set { isMatchable = value; } }
        public bool IsVisible { get { return isVisible; } set { isVisible = value; } }
        public bool IsDestroyable { get { return isDestroyable; } set { isDestroyable = value; } }
        public int StartingHp { get { return startingHp; } set { startingHp = value; } }
        public Color BaseColor { get { return baseColor; } set { baseColor = value; } }
        public bool Shufflable { get { return shuffable; } set { shuffable = value; } }
        //public Color SubBaseColor { get { return subBaseColor; } set { subBaseColor = value; } }
        public SkillObject TouchSkillOject => touchSkillOject;
        public SkillEnums.CellUpdateSprite UpdateSprite => updateSprite;
        public SkillEnums.BlockerType BlockerType => blockerType;

        // public SkillEnums.SpecialTilesEnum SpecialTileCategory => specialTileCategory;
        public SkillEnums.SpecialTilesEnum SpecialTileCategory
        {
            get { return specialTileCategory; }
            
#if UNITY_EDITOR
            set { specialTileCategory = value; }
#endif
        }
        
        // public SkillEnums.ElementFilter ElementType => elementType;

        public SkillEnums.ElementFilter ElementType
        {
            get { return elementType; }
            
            #if UNITY_EDITOR
            set { elementType = value; }
            #endif
        }

        public SkillEnums.CellGridLocation TileGridLocation => gridLocation;
        /// <summary>
        /// Returns true if the tile can form a match against a match value.
        /// </summary>

        public bool CanDamage(CellType_DamageCondition.DamageData damageData) => damageConditions.Count > 0 ? damageConditions.Any(x => x != null ? x.CanDamage(damageData) : false) : true;
    }
}
