using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenVisibiltyListener : MonoBehaviour
{
    public static ScreenVisibiltyListener Instance { get; private set; }
    public List<ScreenVisiblityObject> ActiveScene = new List<ScreenVisiblityObject>();
    int index(ScreenVisiblityObject value) { return ActiveScene.IndexOf(value); }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public void ActiveScreen(ScreenVisiblityObject screenName)
    {
        if (screenName != null)
            if (!ActiveScene.Contains(screenName))
                ActiveScene.Add(screenName);

        if (ActiveScene.Count > 1)
            if (!ActiveScene[index(screenName) - 1].IsScreenOverlay)
                ActiveScene[index(screenName) - 1].CameraUpdate(false);
            else
                ActiveScene[index(screenName) - 1].CanvasUpdate(0, false);
    }
   
    public void InActiveScreen(ScreenVisiblityObject screenName)
    {
        if (screenName != null)
            if (ActiveScene.Contains(screenName))
                ActiveScene.Remove(screenName);

        if (ActiveScene != null)
            if (ActiveScene.Count > 0)
                if (!ActiveScene[ActiveScene.Count - 1].IsScreenOverlay)
                    ActiveScene[ActiveScene.Count - 1].CameraUpdate(true);
                else
                    ActiveScene[ActiveScene.Count - 1].CanvasUpdate(1, true);
    }
}
