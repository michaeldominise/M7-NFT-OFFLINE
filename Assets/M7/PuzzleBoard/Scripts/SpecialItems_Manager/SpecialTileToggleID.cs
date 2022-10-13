using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Skill;
using UnityEngine.UI;
using M7.Match;
using UnityEngine.SceneManagement;
using M7.GameRuntime;

public class SpecialTileToggleID : MonoBehaviour
{
    [SerializeField] public SkillObject[] skillSpecialTileID;
    [SerializeField] public string toggleID;
    [SerializeField] public int previousRandomInt;
    [SerializeField] public Toggle toggle => GetComponent<Toggle>(); 
    public void InitToggle()
    {
        var rand = Random.Range(0, skillSpecialTileID.Length);
        InitialMenuManager.Instance.SpecialTileSelectorManager.ToggleInit(this, rand);
    }
}
