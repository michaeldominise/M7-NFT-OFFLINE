using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    public class BaseAssetObject : ScriptableObject
    {
        [ShowInInspector, PropertyOrder(-1)] public string MasterID => name;

        public static bool operator ==(BaseAssetObject rpgobj1, BaseAssetObject rpgobj2)
        {
            var obj1 = rpgobj1 as object;
            var obj2 = rpgobj2 as object;
            if (obj1 != null && obj2 != null)
                return rpgobj1.name == rpgobj2.name;
            return obj1 == obj2;
        }

        public static bool operator !=(BaseAssetObject rpgobj1, BaseAssetObject rpgobj2)
        {
            var obj1 = rpgobj1 as object;
            var obj2 = rpgobj2 as object;
            if (obj1 != null && obj2 != null)
                return rpgobj1.name != rpgobj2.name;
            return obj1 != obj2;
        }

        public override bool Equals(object other)
        {
            if (this != null && other != null)
            {
                var rpg = other as RPGObject;
                return name == (rpg?.name ?? null);
            }
            return this == null && other == null;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

