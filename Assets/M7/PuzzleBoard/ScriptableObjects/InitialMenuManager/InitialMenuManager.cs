using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialMenuManager : MonoBehaviour
{
    public static InitialMenuManager Instance { get; private set; }

    [SerializeField] SpecialTileSelectorManager specialTileSelectorManager;
    [SerializeField] GameInventoryManager gameInventoryManager;

    public SpecialTileSelectorManager SpecialTileSelectorManager => specialTileSelectorManager;
    public GameInventoryManager GameInventoryManager => gameInventoryManager;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

}
