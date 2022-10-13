using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "CurrencyObject", menuName = "Assets/M7/GameData/RpgObject/CurrencyObject")]
    public class CurrencyObject : RPGObject
    {
#if UNITY_EDITOR
        public static CurrencyObject ActiveCurrencyObject { get { return (CurrencyObject)Selection.activeObject; } }
#endif
        [SerializeField] private string displayName;
        [SerializeField] private Sprite displayImage;

        public string DisplayName => displayName;
        public Sprite DisplayImage => displayImage;
    }
}
