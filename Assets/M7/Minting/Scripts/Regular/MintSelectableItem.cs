 using UnityEngine;
using TMPro;
using UnityEngine.UI;
using M7.GameData;
using DG.Tweening;
using M7.GameRuntime;
using M7;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.EventSystems;// Required when using Event data.

public class MintSelectableItem : CharacterInstance_Avatar
{
    public Button buttonCharacter;
    public CharacterInstance_InventoryCard characterSelectedM;
    public string charId;

    public void OnClickLoadHero ()
    {
        characterSelectedM.Init(PlayerDatabase.Inventories.Characters.FindItem(charId), null);
        
        foreach (Transform child in this.transform.parent) {
            child.GetComponent<Button>().interactable = true;
        }
        this.GetComponent<Button>().interactable = false;
    }
}
