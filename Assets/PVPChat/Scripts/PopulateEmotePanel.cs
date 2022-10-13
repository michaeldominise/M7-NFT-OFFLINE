using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Chat;

public class PopulateEmotePanel : MonoBehaviour
{
    public GameObject EmoteButtonTemplate;
    [SerializeField] TMP_InputField ChatInput;
    //[SerializeField] TMP_Text EmoteArea;
    // Start is called before the first frame update
    void Start()
    {
        Populate();
    }

    private void Populate()
    {
        //Debug.Log("Default sprite asset=" + TMPro.TMP_Settings.defaultSpriteAsset.spriteCharacterTable.Count);
        //Debug.Log("Number of Sprites=" + TMPro.TMP_Settings.GetSpriteAsset().spriteCharacterTable.Count);

        foreach (var SpriteElement in TMPro.TMP_Settings.GetSpriteAsset().spriteCharacterTable)
        {
            //Debug.Log(SpriteElement.name);
            GameObject gameObject = Instantiate(EmoteButtonTemplate) as GameObject;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(EmoteButtonTemplate.transform.parent, false);
            gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "<sprite="+SpriteElement.glyphIndex+">";
            gameObject.GetComponent<Button>().onClick.AddListener(() => AddEmote(SpriteElement));
        }    
    }

    private void AddEmote(TMP_SpriteCharacter SpriteElement)
    {
        ChatInput.text += "<sprite=" + SpriteElement.glyphIndex + ">";
    }
}
