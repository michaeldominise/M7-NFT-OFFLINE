using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewComboTimerState", menuName = "Assets/Data/ComboTimer/Combo timer State")]
public class ComboTimerState:ScriptableObject {


    [SerializeField] ComboTimerEnums.DecrementState _timerStateType;
    [SerializeField] float _deductionRate = 1.0f;
    [SerializeField] float _minTimerVal = 1;
    [SerializeField] float _startingTimerVal = 3;


    public ComboTimerEnums.DecrementState timerStateType { get { return _timerStateType; } }

    public float deductionRate { get { return _deductionRate; } }
    public float GetMinValOverride { get { return _minTimerVal; } }
    public float GetStartingValOverride {get { return _startingTimerVal; }
    }

   
}
