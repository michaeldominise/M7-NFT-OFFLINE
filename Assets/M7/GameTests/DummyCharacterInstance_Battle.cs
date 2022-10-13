#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameTests
{
    public class DummyCharacterInstanceBattle : MonoBehaviour
    {
#if UNITY_EDITOR
        [Button]
        private void TestOverDrive()
        {
            
        }
#endif
    }
}
#endif
