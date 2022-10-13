using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[Serializable]
public class Chips
{
    public string Id;
    public Sprite _chipImage;
    public string _chipName;
}


[CreateAssetMenu( fileName = "ChipsData", menuName = "Assets/M7/ChipsData")]
public class ChipData : ScriptableObject
{
    public Chips[] CipsData;

}
