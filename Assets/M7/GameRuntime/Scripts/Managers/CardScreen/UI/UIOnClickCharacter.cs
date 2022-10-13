using UnityEngine;

namespace M7.GameRuntime
{
    public class UIOnClickCharacter : MonoBehaviour
    {
        //public CharacterCardInfoManager castedCharacterCardInfoManager => CharacterCardInfoManager.Instance;

        // Update is called once per frame
        void FixedUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

                if (hit.collider)
                {
                    //castedCharacterCardInfoManager.targetRef = hit.collider.gameObject.GetComponent<CharacterInstance_Battle>();
                    //castedCharacterCardInfoManager.ShowCardInfo();
                }
            }
        }
    }
}