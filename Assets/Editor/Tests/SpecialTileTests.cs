using System.Collections.Generic;
using M7.Match;
using M7.PuzzleBoard.Scripts.SpecialTiles;
using M7.Skill;
using NUnit.Framework;
using UnityEngine;

namespace Editor.Tests
{
    public class SpecialTileTests
    {
        /*
        // A Test behaves as an ordinary method
        [Test]
        public void SpecialTilePrismCombo()
        {
            var currentTile = new SpecialTileComboCriteria
            {
                elementFilter = SkillEnums.ElementFilter.Fire,
                specialTile = SkillEnums.SpecialTilesEnum.Prism
            };
        
            var tileRequirements = new SpecialTileComboCriteria
            {
                elementFilter = SkillEnums.ElementFilter.Fire,
                specialTile = SkillEnums.SpecialTilesEnum.Prism
            };

            var isPrism = tileRequirements.specialTile.HasFlag(currentTile.specialTile);
            var hasElement = tileRequirements.elementFilter.HasFlag(currentTile.elementFilter);
        
            Assert.True(isPrism && hasElement);
        }
    
        [Test]
        public void SpecialTilePrismRocketCombo()
        {
            var currentTile = new SpecialTileComboCriteria
            {
                elementFilter = SkillEnums.ElementFilter.None,
                specialTile = SkillEnums.SpecialTilesEnum.RocketHorizontal
            };
        
            var tileRequirements = new SpecialTileComboCriteria
            {
                elementFilter = SkillEnums.ElementFilter.None,
                specialTile = SkillEnums.SpecialTilesEnum.RocketHorizontal | SkillEnums.SpecialTilesEnum.RocketVertical
            };

            var isPrism = tileRequirements.specialTile.HasFlag(currentTile.specialTile);
            var hasElement = tileRequirements.elementFilter.HasFlag(currentTile.elementFilter);
        
            Assert.True(isPrism && hasElement);
        }
        
        [Test]
        public void SpecialTilePrismRocketComboFromList()
        {
            var specialTileComboObject = new SpecialTileComboObject();
            var specialComboDataList = new List<SpecialTileComboData>
            {
                new SpecialTileComboData
                {
                    comboName = "PrismPrism",
                    specialTileCombo = new List<SpecialTileComboCriteria>
                    {
                        // prism
                        new SpecialTileComboCriteria 
                        {
                            elementFilter = SkillEnums.ElementFilter.Water,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        },
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.None,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        }
                    }
                },
                new SpecialTileComboData
                {
                    comboName = "PrismRocket",
                    specialTileCombo = new List<SpecialTileComboCriteria>
                    {
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.Water,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        },
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.None,
                            specialTile = SkillEnums.SpecialTilesEnum.RocketHorizontal | SkillEnums.SpecialTilesEnum.RocketVertical
                        }
                    }
                }
            };

            //var waterPrism = ScriptableObject.CreateInstance<TileType>();
            //waterPrism.ElementType = SkillEnums.ElementFilter.Water;
            //waterPrism.SpecialTileCategory = SkillEnums.SpecialTilesEnum.Prism;
            
            //var rocket = ScriptableObject.CreateInstance<TileType>();
            //rocket.ElementType = SkillEnums.ElementFilter.None;
            //rocket.SpecialTileCategory = SkillEnums.SpecialTilesEnum.RocketHorizontal;

            //specialTileComboObject.SetSpecialTileComboList(specialComboDataList);

            //var skillName = specialTileComboObject.CheckSpecialTileCombo(rocket, waterPrism);

            //Assert.AreEqual("PrismRocket", skillName);
        }
        
        [Test]
        public void SpecialTilePrismPrismCombo()
        {
            var specialTileComboObject = new SpecialTileComboObject();
            var specialComboDataList = new List<SpecialTileComboData>
            {
                new SpecialTileComboData
                {
                    comboName = "PrismPrism",
                    specialTileCombo = new List<SpecialTileComboCriteria>
                    {
                        // prism
                        new SpecialTileComboCriteria 
                        {
                            elementFilter = SkillEnums.ElementFilter.All,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        },
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.All,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        }
                    }
                },
                new SpecialTileComboData
                {
                    comboName = "PrismGreenRocket",
                    specialTileCombo = new List<SpecialTileComboCriteria>
                    {
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.Earth | SkillEnums.ElementFilter.Special,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        },
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.Special,
                            specialTile = SkillEnums.SpecialTilesEnum.RocketHorizontal | SkillEnums.SpecialTilesEnum.RocketVertical
                        }
                    }
                }
            };

            //var earthPrism = ScriptableObject.CreateInstance<TileType>();
            //earthPrism.ElementType = SkillEnums.ElementFilter.Earth | SkillEnums.ElementFilter.Special;
            //earthPrism.SpecialTileCategory = SkillEnums.SpecialTilesEnum.Prism;
            
            //var firePrism = ScriptableObject.CreateInstance<TileType>();
            //firePrism.ElementType = SkillEnums.ElementFilter.Fire | SkillEnums.ElementFilter.Special;
            //firePrism.SpecialTileCategory = SkillEnums.SpecialTilesEnum.Prism;

            //specialTileComboObject.SetSpecialTileComboList(specialComboDataList);

            //var skillName = specialTileComboObject.CheckSpecialTileCombo(firePrism, earthPrism);

            //Assert.AreEqual("PrismPrism", skillName);
        }
        
        [Test]
        public void SpecialTilePrismBlueRocketCombo()
        {
            var specialTileComboObject = new SpecialTileComboObject();
            var specialComboDataList = new List<SpecialTileComboData>
            {
                new SpecialTileComboData
                {
                    comboName = "PrismPrism",
                    specialTileCombo = new List<SpecialTileComboCriteria>
                    {
                        // prism
                        new SpecialTileComboCriteria 
                        {
                            elementFilter = SkillEnums.ElementFilter.All,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        },
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.All,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        }
                    }
                },
                new SpecialTileComboData
                {
                    comboName = "PrismBlueRocket",
                    specialTileCombo = new List<SpecialTileComboCriteria>
                    {
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.Water | SkillEnums.ElementFilter.Special,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        },
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.Special,
                            specialTile = SkillEnums.SpecialTilesEnum.RocketHorizontal | SkillEnums.SpecialTilesEnum.RocketVertical
                        }
                    }
                }
            };

            //var waterPrism = ScriptableObject.CreateInstance<TileType>();
            //waterPrism.ElementType = SkillEnums.ElementFilter.Water | SkillEnums.ElementFilter.Special;
            //waterPrism.SpecialTileCategory = SkillEnums.SpecialTilesEnum.Prism;
            
            //var horizontalRocket = ScriptableObject.CreateInstance<TileType>();
            //horizontalRocket.ElementType = SkillEnums.ElementFilter.Special;
            //horizontalRocket.SpecialTileCategory = SkillEnums.SpecialTilesEnum.RocketVertical;

            //specialTileComboObject.SetSpecialTileComboList(specialComboDataList);

            //var skillName = specialTileComboObject.CheckSpecialTileCombo(horizontalRocket, waterPrism);

            //Assert.AreEqual("PrismBlueRocket", skillName);
        }
        
        [Test]
        public void SpecialTilePrismGreenRocketCombo()
        {
            var specialTileComboObject = new SpecialTileComboObject();
            var specialComboDataList = new List<SpecialTileComboData>
            {
                new SpecialTileComboData
                {
                    comboName = "PrismPrism",
                    specialTileCombo = new List<SpecialTileComboCriteria>
                    {
                        // prism
                        new SpecialTileComboCriteria 
                        {
                            elementFilter = SkillEnums.ElementFilter.All,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        },
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.All,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        }
                    }
                },
                new SpecialTileComboData
                {
                    comboName = "PrismGreenRocket",
                    specialTileCombo = new List<SpecialTileComboCriteria>
                    {
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.Earth | SkillEnums.ElementFilter.Special,
                            specialTile = SkillEnums.SpecialTilesEnum.Prism
                        },
                        // prism
                        new SpecialTileComboCriteria
                        {
                            elementFilter = SkillEnums.ElementFilter.Special,
                            specialTile = SkillEnums.SpecialTilesEnum.RocketHorizontal | SkillEnums.SpecialTilesEnum.RocketVertical
                        }
                    }
                }
            };

            //var earthPrism = ScriptableObject.CreateInstance<TileType>();
            //earthPrism.ElementType = SkillEnums.ElementFilter.Earth | SkillEnums.ElementFilter.Special;
            //earthPrism.SpecialTileCategory = SkillEnums.SpecialTilesEnum.Prism;
            
            //var horizontalRocket = ScriptableObject.CreateInstance<TileType>();
            //horizontalRocket.ElementType = SkillEnums.ElementFilter.Special;
            //horizontalRocket.SpecialTileCategory = SkillEnums.SpecialTilesEnum.RocketVertical;

            //specialTileComboObject.SetSpecialTileComboList(specialComboDataList);

            //var skillName = specialTileComboObject.CheckSpecialTileCombo(horizontalRocket, earthPrism);

            //Assert.AreEqual("PrismGreenRocket", skillName);
        }
        */
    }
}
