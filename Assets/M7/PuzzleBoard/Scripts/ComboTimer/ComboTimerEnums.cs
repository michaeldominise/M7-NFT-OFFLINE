using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboTimerEnums
{


    const float THREHSOLD_NORMAL = 0.66f;
    const float THRESHOLD_HALF = 0.45f;
    const float THRESHOLD_CRITICAL = 0.33f;

    public enum DecrementState
    {
        Normal,
        Slow,
        Fast,
        Stopped
    };

    public enum TimeRemainingState
    {
        Normal,
        Half,
        Critical
    };


    public TimeRemainingState Get_TimeRemainingState(float timeRemainingPercentage)
    {
        if (timeRemainingPercentage >= THREHSOLD_NORMAL)
            return TimeRemainingState.Normal;

        else if (timeRemainingPercentage < THREHSOLD_NORMAL && timeRemainingPercentage >= THRESHOLD_CRITICAL)
            return TimeRemainingState.Half;

        else
            return TimeRemainingState.Critical;
    }
}
