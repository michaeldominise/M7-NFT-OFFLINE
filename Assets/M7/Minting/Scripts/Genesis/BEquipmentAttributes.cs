using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BEquipmentAttributes : MonoBehaviour
{
    public Image defaultImage;
    public Image addImage;
   
    public TMP_Text label;

    public void UpdateUIAttributes (float value, float total)
    {
        float a = Mathf.Clamp01(value / total);
        float b = Mathf.Clamp01(total / total);

        defaultImage.fillAmount = a;
        addImage.fillAmount = b;
        label.text = total.ToString();
    }
}
