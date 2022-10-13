using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenVisiblityObject : MonoBehaviour
{
    [SerializeField] Camera m_Cam;
    [SerializeField] CanvasGroup m_canvas;
    [SerializeField] string ScreenName;
    [SerializeField] bool ScreenOverlay = false;
    public bool IsScreenOverlay { get { return ScreenOverlay; } }

    ScreenVisiblityObject screenVisiblityObject { get { return GetComponent<ScreenVisiblityObject>(); } }
    ScreenVisibiltyListener screenVisibiltyListener { get { return ScreenVisibiltyListener.Instance; } }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = ScreenName;
        screenVisibiltyListener.ActiveScreen(screenVisiblityObject);
    }

    private void OnDestroy()
    {
        screenVisibiltyListener.InActiveScreen(screenVisiblityObject);
    }

    public void CanvasUpdate(float alpha, bool interactable)
    {
        if (m_canvas != null)
        {
            m_canvas.alpha = alpha;
            m_canvas.interactable = interactable;
            m_canvas.blocksRaycasts = interactable;
        }
    }

    public void CameraUpdate(bool toggle)
    {
        if (m_Cam != null)
            m_Cam.enabled = toggle;
    }

}
