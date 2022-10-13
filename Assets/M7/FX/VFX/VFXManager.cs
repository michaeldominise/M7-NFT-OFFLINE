using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

public class VFXManager : FXManager
{
    public enum PredefinedVFX { Sample1 }

    static VFXManager _Instance;
    public static VFXManager Instance
    {
        get
        {
            _Instance = _Instance ?? Instantiate(Resources.Load<VFXManager>("VFXManager"));
            return _Instance;
        }
    }

    public List<VFXObject> predefinedVFXs = new List<VFXObject>();

    public static void Spawn(PredefinedVFX predefinedVFX, Transform parent = null)
    {
        Spawn(predefinedVFX, Vector3.zero, parent);
    }

    public static void Spawn(PredefinedVFX predefinedVFX, Vector3 localPos, Transform parent = null)
    {
        Spawn(Instance.predefinedVFXs[(int)predefinedVFX], localPos, parent);
    }

    public static void Spawn(VFXObject vfxObject, Vector3 localPos, Transform parent = null)
    {
        Instance._Spawn(vfxObject, localPos, parent);
    }

    public static void Despawn(VFXObject vfxObject)
    {
        Instance._Despawn(vfxObject);
    }
}
