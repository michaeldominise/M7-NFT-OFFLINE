using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialTileUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bombCount_Text;
    [SerializeField] TextMeshProUGUI prismCount_Text;
    [SerializeField] TextMeshProUGUI rocketCount_Text;

    private void Awake()
    {
        //Init();
    }
    public void Init()
    {
        bombCount_Text.text = InitialMenuManager.Instance.GameInventoryManager.bombAmount.ToString("0");
        prismCount_Text.text = InitialMenuManager.Instance.GameInventoryManager.prismAmount.ToString("0");
        rocketCount_Text.text = InitialMenuManager.Instance.GameInventoryManager.rocketAmount.ToString("0");
    }
}
