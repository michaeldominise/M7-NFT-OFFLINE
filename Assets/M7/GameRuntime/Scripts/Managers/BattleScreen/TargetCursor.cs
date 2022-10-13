using System;
using System.Security.Principal;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Managers.BattleScreen
{
    [Serializable]
    public class TargetCursor
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
        [SerializeField] private Vector2 cursorHotSpot = Vector2.zero;
        
        public void SetCursor()
        {
            //Debug.Log("SetCursor");
            Cursor.SetCursor(cursorTexture, cursorHotSpot, cursorMode);
        }
        
        public void UnsetCursor()
        {
            //Debug.Log("UnsetCursor");
            Cursor.SetCursor(null, cursorHotSpot, cursorMode);
        }
    }
}
