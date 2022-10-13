using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchTimerSlider : MonoBehaviour {

    [Header("UI Object refs")]
    [SerializeField] Slider _timerSlider;
    [SerializeField] TextMeshProUGUI _timerLabelDisplay;

    M7.Match.TimerBasedMovementManager _timerManager => M7.Match.TimerBasedMovementManager.Instance;


    void TryUpdate_Label()
    {
        if (_timerLabelDisplay == null)
            return;

        if (_timerManager == null)
            return;

        _timerLabelDisplay.text = _timerManager.timerRawRemaining_String;
    }

    void TryUpdate_Slider()
    {
        if (_timerSlider == null)
            return;

        if (_timerManager == null)
            return;

        _timerSlider.value = _timerManager.timerProgressRemaining;
        
    }

    public void Show() => gameObject.SetActive(true);

    public void ResetTimer()
    {
        gameObject.SetActive(false);
        _timerManager.ResetTimer();
        TryUpdate_Label();
        TryUpdate_Slider();

        //Debug.Log("[RESET TIMER]");
    }

	private void Update()
	{
        TryUpdate_Label();
        TryUpdate_Slider();
	}
}
