using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEngine;

namespace M7.FX
{
    public abstract class FXManager : MonoBehaviour
    {
        [SerializeField] protected SpawnPool spawnPool;

        public FXObject _Spawn(FXObject fxObject, Vector3 localPos, Transform parent = null)
        {
            if (parent)
                return spawnPool.Spawn(fxObject.gameObject, localPos, Quaternion.identity, parent).GetComponent<FXObject>();
            else
                return spawnPool.Spawn(fxObject.gameObject, localPos, Quaternion.identity).GetComponent<FXObject>();
        }

        public void _Despawn(FXObject fxObject)
        {
            spawnPool.Despawn(fxObject.transform);
        }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
