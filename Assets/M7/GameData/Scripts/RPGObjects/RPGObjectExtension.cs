using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameData
{
    public static class RPGObjectExtension
    {
        public static bool IsCharacter(this RPGObject rpgObject) { return rpgObject as CharacterObject != null; }
        //public static bool IsEquipable(this RPGObject rpgObject) { return rpgObject != null && (rpgObject.GetType() == typeof(EquipableObject) || typeof(EquipableObject).IsAssignableFrom(rpgObject.GetType())); }
        //public static bool IsCurrency(this RPGObject rpgObject) { return rpgObject != null && (rpgObject.GetType() == typeof(CurrencyObject) || typeof(CurrencyObject).IsAssignableFrom(rpgObject.GetType())); }
        //public static bool IsBuilding(this RPGObject rpgObject) { return rpgObject != null && rpgObject.GetType() == typeof(BuildingObject); }
    }

}