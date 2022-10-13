using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

public class PopupCanvas : MonoBehaviour {
    static PopupCanvas _Instance;
    public static PopupCanvas Instance
    {
        get
        {
            _Instance = _Instance ?? FindObjectOfType<PopupCanvas>();

            if (_Instance == null)
            {
                _Instance = Instantiate(Resources.Load<PopupCanvas>("PopupCanvas"));
            }

            return _Instance;
        }
    }

    [SerializeField] SpawnPool _spawnPool;
    public SpawnPool SpawnPool { get { return _spawnPool; } }
}
