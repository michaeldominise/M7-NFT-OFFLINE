using UnityEditor;
using UnityEngine;

namespace M7.GameData.Scripts.RPGObjects.Boosters
{
    [CreateAssetMenu(fileName = "GemObject", menuName = "Assets/M7/GameData/RpgObject/BoosterObject")]
    public class BoosterObject : RPGObject
    {
#if UNITY_EDITOR
        public static BoosterObject ActiveBoosterObject => (BoosterObject)Selection.activeObject;
#endif
        [SerializeField] BoosterObjectDisplayStats displayStats;
        public BoosterObjectDisplayStats DisplayStats => displayStats;
    }
}
