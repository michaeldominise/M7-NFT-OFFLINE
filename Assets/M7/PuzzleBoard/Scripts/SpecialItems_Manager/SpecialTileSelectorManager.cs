using System.Collections.Generic;
using UnityEngine;
using M7.Skill;

public class SpecialTileSelectorManager : MonoBehaviour
{
    public List<SkillObject> specialTiles;

    public void ToggleInit(SpecialTileToggleID toggle, int randomInt)
    {
        switch (toggle.toggleID)
        {
            case "bomb":
                
                if (toggle.toggle.isOn)
                {
                    specialTiles.Add(toggle.skillSpecialTileID[randomInt]);
                    toggle.previousRandomInt = randomInt;
                }
                else
                    specialTiles.Remove(toggle.skillSpecialTileID[toggle.previousRandomInt]);
                break;
            case "prism":
                if (toggle.toggle.isOn)
                {
                    specialTiles.Add(toggle.skillSpecialTileID[randomInt]);
                    toggle.previousRandomInt = randomInt;
                }
                else
                    specialTiles.Remove(toggle.skillSpecialTileID[toggle.previousRandomInt]);
                break;
            case "rocket":
                if (toggle.toggle.isOn)
                {
                    specialTiles.Add(toggle.skillSpecialTileID[randomInt]);
                    toggle.previousRandomInt = randomInt;
                }
                else
                    specialTiles.Remove(toggle.skillSpecialTileID[toggle.previousRandomInt]);
                break;
        }
    }
}
