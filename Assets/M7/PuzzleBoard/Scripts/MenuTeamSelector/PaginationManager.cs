using UnityEngine;
using DG.Tweening;
using M7.GameRuntime;
using TMPro;
using M7.GameData;
using System.Collections;
using M7;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using System;
using UnityEngine.UI;

public class PaginationManager : MonoBehaviour
{
    [SerializeField] Toggle togglePref;
    [SerializeField] Transform container;

    [ShowInInspector, ReadOnly] int CurrentIndex { get; set; }
    [ShowInInspector, ReadOnly] List<Toggle> TogglePool { get; set; } = new List<Toggle>();

    private void Awake()
    {
        TogglePool.Add(togglePref);
    }

    public void UpdateValues(int pageIndex, int pageCount)
    {
        if(TogglePool.Count < pageCount)
            for (var x = TogglePool.Count; x < pageCount; x++)
                TogglePool.Add(Instantiate(togglePref, container));

        CurrentIndex = pageIndex;
        TogglePool[pageIndex].isOn = true;
    }
}
