using M7.GameData;
using M7.GameRuntime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop_GatchaManager : MonoBehaviour
{
    [SerializeField] UIDelayValue atkBaseProgressBar;
    [SerializeField] UIDelayValue lukBaseProgressBar;
    [SerializeField] UIDelayValue passBaseProgressBar;
    [SerializeField] UIDelayValue hpBaseProgressBar;

    [SerializeField] UIDelayValue atkText;
    [SerializeField] UIDelayValue lukText;
    [SerializeField] UIDelayValue passText;
    [SerializeField] UIDelayValue hpText;

    public float atkValue;
    public float lukValue;
    public float passValue;
    public float hpValue;

    //public static string characterName;

    [SerializeField] Image element;
    [SerializeField] TextMeshProUGUI characterNameText;

    public List<SaveableCharacterData> gatchaCharsData;
    public SaveableCharacterData gatchaData;

    [SerializeField] Transform spineContainer;

    [SerializeField] CharacterInstance_Spine characterSpine;

    private void Awake()
    {
        Init();

        atkBaseProgressBar.SetValue(atkValue);
        lukBaseProgressBar.SetValue(lukValue);
        passBaseProgressBar.SetValue(passValue);
        hpBaseProgressBar.SetValue(hpValue);

        atkText.SetValue(atkValue);
        lukText.SetValue(lukValue);
        passText.SetValue(passValue);
        hpText.SetValue(hpValue);

    }
    public void Init()
    {
        gatchaCharsData = PlayerDatabase.Inventories.Characters.GetItems();
        gatchaData = gatchaCharsData[^1];

        atkValue = gatchaData.BaseStats.Attack;
        lukValue = gatchaData.BaseStats.Luck;
        passValue = gatchaData.BaseStats.Passion;
        hpValue = gatchaData.BaseStats.Hp;

        characterSpine.Init(gatchaData, null);
        StartCoroutine(InitCharObject());
    }

    IEnumerator InitCharObject()
    {
        yield return new WaitUntil(() => characterSpine.CharacterObject != null);
        element.sprite = characterSpine.CharacterObject.Element.DisplaySprite;

        characterNameText.text = characterSpine.CharacterObject.DisplayStats.DisplayName;
    }
}
