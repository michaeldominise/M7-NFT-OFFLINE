using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentSceneChecker : MonoBehaviour
{
    public bool allowSceneHack;
    public static bool bypassScene;

    public void Init()
    {
        if(!allowSceneHack)
        {
            if (!bypassScene)
                SceneManager.LoadScene("M7PuzzleMenuScene");
            else
                Debug.Log("Scene bypass is " + bypassScene);
        }
    }
}
