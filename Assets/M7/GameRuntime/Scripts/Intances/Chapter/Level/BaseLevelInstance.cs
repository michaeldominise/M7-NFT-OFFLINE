using M7.CDN.Addressable;
using M7.GameData;
using M7.Skill;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using M7.CDN;

namespace M7.GameRuntime
{
    [System.Serializable]
    public abstract class BaseLevelInstance : InitializableInstance<LevelData>
    {
        public LevelData LevelData => ObjectData;
    }
}