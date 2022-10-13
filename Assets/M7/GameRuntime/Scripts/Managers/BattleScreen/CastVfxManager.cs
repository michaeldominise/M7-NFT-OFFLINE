using PathologicalGames;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameData;
using M7.Skill;
using Sirenix.OdinInspector;

namespace M7.GameRuntime
{
    public class CastVfxManager : MonoBehaviour
    {
        public static CastVfxManager Instance => BattleManager.Instance.CastVfxManager;

        [SerializeField] SpawnPool spawnPool;
        [SerializeField] ElementCastVfx[] elementCastVfxList;
        [ShowInInspector] Dictionary<CharacterInstance_Battle, Transform> activeVfx = new Dictionary<CharacterInstance_Battle, Transform>();

        public void AnimateVfx(CharacterInstance_Battle targetInstance)
        {
            if (elementCastVfxList.Length == 0 || activeVfx.ContainsKey(targetInstance))
                return;

            var elementCastVfx = elementCastVfxList.FirstOrDefault(x => x.elementType == targetInstance.Element.ElementType) ?? elementCastVfxList[0];
            var vfx = spawnPool.Spawn(elementCastVfx.vfx ?? elementCastVfxList[0].vfx, Vector3.zero, Quaternion.identity, targetInstance.MainSpineInstance.transform);
            vfx.transform.localPosition = Vector3.zero;
            activeVfx[targetInstance] = vfx;
        }

        public void RemoveVfx(CharacterInstance_Battle targetInstance)
        {
            if (!activeVfx.ContainsKey(targetInstance))
                return;
            spawnPool.Despawn(activeVfx[targetInstance]);
            activeVfx.Remove(targetInstance);
        }
    }

    [Serializable]
    class ElementCastVfx
    {
        public SkillEnums.ElementFilter elementType;
        public GameObject vfx;
    }
}
