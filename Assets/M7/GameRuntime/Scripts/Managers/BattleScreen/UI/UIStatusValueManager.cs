using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using M7.GameData;
using System;
using Sirenix.OdinInspector;
using PathologicalGames;
using M7.FX;

namespace M7.GameRuntime
{
    public class UIStatusValueManager : MonoBehaviour
    {
        public enum DamageType { Effective, Normal, Weak }
        public static UIStatusValueManager Instance => BattleManager.Instance.HurtDamageManager;
        [SerializeField] UIBattle_StatusValueManager hurtPref;
        [SerializeField] SpawnPool pool;

        public void Play(Vector3 worldPosition, float value, DamageType damageType, RPGElement rPGElement, bool isHeal)
        {
            var uiPosition = ParticleWorldManager.Instance.GetWorldPositionFromCameraType(worldPosition, ParticleWorldManager.CameraType.World, ParticleWorldManager.CameraType.SystemUI);
            var hurtObj = pool.Spawn(hurtPref.gameObject, transform).GetComponent<UIBattle_StatusValueManager>();

            hurtObj.transform.localScale = isHeal ? new Vector3(0.25f, 0.25f, 0.25f) : new Vector3(0.5f, 0.5f, 1.0f);
            hurtObj.Play(uiPosition, value, damageType, rPGElement, isHeal);
            hurtObj.transform.SetAsLastSibling();
            hurtObj.transform.localPosition -= Vector3.forward * 0.01f * hurtObj.transform.GetSiblingIndex();
        }

        public void Despawn(UIBattle_StatusValueManager uIBattle_HurtDamage) => pool.Despawn(uIBattle_HurtDamage.transform);
    }
}

