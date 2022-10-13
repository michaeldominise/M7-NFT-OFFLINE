using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M7.GameRuntime;
using Sirenix.OdinInspector;

public class StarConditionManager : MonoBehaviour
{
    [SerializeField] UIDelayValue[] barFill;
    [SerializeField] public int currentStarCount;
    [SerializeField] public float destroyedTilesLeftOver;

    public int TotalGoalCount => LevelManager.LevelData.GoalConditions[0].GoalCount;

    public List<float> multiplier;
    public List<float> finalGoalCount;

    public void Init()
    {
        //Multiplier();

        for(int i = 0; i < barFill.Length; i++)
        {
            barFill[i].GetComponent<Slider>().maxValue = finalGoalCount[i];
        }
    }

    [Button]
    public void DestroyTileTest(float destroyedTiles)
    {
        float totalDestroyedCubes = destroyedTiles;
        destroyedTilesLeftOver = 0;

        barFill[currentStarCount].GetComponent<Slider>().value += totalDestroyedCubes;
        //barFill[currentStarCount].SetValue(totalDestroyedCubes);

        float updateSliderValue = totalDestroyedCubes - barFill[currentStarCount].GetComponent<Slider>().value;
        //print(updateSliderValue);

        if (updateSliderValue > 0)
        {
            destroyedTilesLeftOver = updateSliderValue;
        }

        if (barFill[currentStarCount].GetComponent<Slider>().value >= barFill[currentStarCount].GetComponent<Slider>().maxValue)
        {
            foreach(Transform child in barFill[currentStarCount].transform)
            {
                if(child.name == "Star")
                {
                    Transform starFill = child.transform.Find("Star_Fill");
                    starFill.gameObject.SetActive(true);
                }
            }

            if (currentStarCount < 2)
                currentStarCount++;
        }


        if (destroyedTilesLeftOver > 0)
        {
            DestroyTileTest(destroyedTilesLeftOver);
        }

    }

    void Multiplier()
    {
        for (int i = 0; i < multiplier.Count; i++)
        {
            finalGoalCount.Add(TotalGoalCount * multiplier[i]);
        }
    }
}
