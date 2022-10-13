using M7.GameRuntime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using System.Collections;
using M7.GameData;
using M7;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

public partial class SROptions
{
    [Category("Test")]
    public void RestartGame()
    {
        SRDebug.Instance.HideDebugPanel();
        GameManager.RestartGameDialog();
    }
}