using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class UIValueImage_Update : UIValueImage
    {
        [SerializeField] GameObject[] uiTeamManaPoints;
        int curValue;
        int lastValue;

        public override void OnValueUpdate(float value)
        {
            //print("SKILLP " + curValue);

            curValue = (int)value;

            if (curValue > lastValue)
            {
                for (int i = 0; i < curValue; i++)
                {
                    uiTeamManaPoints[i].gameObject.SetActive(true);
                }
            }
            else if (curValue < lastValue)
            {
                uiTeamManaPoints[(int)value].gameObject.SetActive(false);
            }

            lastValue = curValue;
        }
    }
}