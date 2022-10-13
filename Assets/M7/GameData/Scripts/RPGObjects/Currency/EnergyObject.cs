using System;
using UnityEditor;
using UnityEngine;

namespace M7.GameData.Scripts.RPGObjects.Currency
{
    [Serializable]
    public class EnergyObject: RPGObject
    {
#if UNITY_EDITOR
        public static EnergyObject ActiveEnergyObject { get { return (EnergyObject)Selection.activeObject; } }
#endif
    }
}