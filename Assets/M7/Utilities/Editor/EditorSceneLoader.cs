using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class EditorSceneLoade
{
    [MenuItem("M7/Run/Initial Scene %g")]
    private static void RunInitialScene()
    {
        EditorSceneManager.OpenScene("Assets/M7/GameRuntime/Scenes/M7Puzzle_InitialScene.unity");
        EditorApplication.EnterPlaymode();
    }
    [MenuItem("M7/Run/Battle Scene #g")]
    private static void RunBattleScene()
    {
        EditorSceneManager.OpenScene("Assets/M7/GameRuntime/Scenes/M7Puzzle_BattleScene.unity");
        EditorApplication.EnterPlaymode();
    }
}
