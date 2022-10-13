using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace M7.ToonBlastPrototype.Scripts
{
    public class Menu : MonoBehaviour,IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData pointerEventData)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
