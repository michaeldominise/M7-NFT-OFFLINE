using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LerpRoutineData 
{
    public float duration;
    float elapsedTime;
    public float lerpStep;


    public void UpdateValues()
    {
        elapsedTime += Time.deltaTime;

        lerpStep = elapsedTime / duration;
    }


    public void Finish()
    {
        elapsedTime = duration;
        lerpStep = 1;
    }

    public void Reinit()
    {
        lerpStep = 0;
        elapsedTime = 0;
    }

 
}
