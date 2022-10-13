using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_ComboTimerIncreaseDisplay : MonoBehaviour
{
    public static VFX_ComboTimerIncreaseDisplay Instance { get; private set; }

    Animator ComboTimerDisplay;

    private void Awake()
    {
        Instance = this;
        ComboTimerDisplay = GetComponentInChildren<Animator>();
        HideTimeIncrease();
    }

    void HideTimeIncrease()
    {
        ComboTimerDisplay.gameObject.SetActive(false);

        if (IsInvoking("HideTimeIncrease"))
            CancelInvoke();
    }

    public void DisplayTimeIncrease()
    {
        ComboTimerDisplay.gameObject.SetActive(true);
        ComboTimerDisplay.Play("Show");
        float animduration = ComboTimerDisplay.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Invoke("HideTimeIncrease", animduration);
    }

}
