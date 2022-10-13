using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using Sirenix.OdinInspector;
using System;
using M7.GameRuntime;
using Unity.Collections.LowLevel.Unsafe;

namespace M7.FX
{
    public class ParticleWorldManager : MonoBehaviour
    {
        [SerializeField] private Transform 
            boosterLaserGunContainer,
            boosterLaserSwordContainer,
            puzzleBoardCenter;
        
        public static ParticleWorldManager Instance => BattleManager.Instance.ParticleWorldManager;

        public enum CameraType
        {
            World,
            Puzzle,
            Particle,
            SystemUI,
            PuzzleParticle,
        }

        [Header("Object Refs Prefs")]
        [SerializeField] SpawnPool spawnPool;

        public System.Action OnFinishedMoving;

        public Vector3 BoosterLaserGunContainerPosition => boosterLaserGunContainer.position;
        public Vector3 BoosterLaserSwordContainerPosition => boosterLaserSwordContainer.position;

        public Vector3 PuzzleBoardCenter => puzzleBoardCenter.position;
        
        public GameObject SpawnVFX(GameObject particlePref, Vector3 worldPos, CameraType cameraType, CameraType cameraTypeTo = CameraType.Particle)
        {
            return SpawnVFX<GameObject>(particlePref, worldPos, cameraType, cameraTypeTo);
        }

        public T SpawnVFX<T>(GameObject particlePref, Vector3 worldPos, CameraType cameraType, CameraType cameraTypeTo = CameraType.Particle)
        {
            Vector3 particleLocalPos = GetParticleLocalPositionFromCameraType(worldPos, cameraType, cameraTypeTo);
            var particle = spawnPool.Spawn(particlePref.transform, particleLocalPos, particlePref.transform.rotation, spawnPool.transform);
            return particle.GetComponent<T>();
        }

        public void DespawnPartcle(Transform particlePref)
        {
            spawnPool.Despawn(particlePref);
        }

        public Vector3 GetParticleLocalPositionFromCameraType(Vector3 worldPosFrom, CameraType cameraTypeFrom, CameraType cameraTypeTo = CameraType.Particle)
        {
            return spawnPool.transform.InverseTransformPoint(GetWorldPositionFromCameraType(worldPosFrom, cameraTypeFrom, cameraTypeTo));
        }

        public Vector3 GetWorldPositionFromCameraType(Vector3 worldPosFrom, CameraType cameraTypeFrom, CameraType cameraTypeTo)
        {
            if (cameraTypeFrom == cameraTypeTo)
                return worldPosFrom;

            Camera cameraFrom = GetCamera(cameraTypeFrom);
            Camera cameraTo = GetCamera(cameraTypeTo);

            if(cameraFrom == null || cameraTo == null)
                return worldPosFrom;

            var asdf = cameraFrom.WorldToScreenPoint(worldPosFrom);
            return cameraTo.ScreenToWorldPoint(cameraFrom.WorldToScreenPoint(worldPosFrom));
        }

        public static void SetLayer(GameObject obj, CameraType targetCameraType)
        {
            switch (targetCameraType)
            {
                case CameraType.World:
                    SetLayerRecursively(obj, BattleCameraHandler.Instance.worldLayer);
                    break;
                case CameraType.SystemUI:
                    SetLayerRecursively(obj, BattleCameraHandler.Instance.systemUILayer);
                    break;
                case CameraType.Particle:
                    SetLayerRecursively(obj, BattleCameraHandler.Instance.particleLayer);
                    break;
                case CameraType.Puzzle:
                    SetLayerRecursively(obj, BattleCameraHandler.Instance.puzzleLayer);
                    break;
                case CameraType.PuzzleParticle:
                    SetLayerRecursively(obj, BattleCameraHandler.Instance.puzzleParticleLayer);
                    break;
            }
        }

        Camera GetCamera(CameraType cameraType) =>
            cameraType switch
            {
                CameraType.SystemUI => BattleCameraHandler.Instance.UiCamera,
                CameraType.World => BattleCameraHandler.Instance.WorldCamera,
                CameraType.Puzzle => BattleCameraHandler.Instance.PuzzleCamera,
                CameraType.Particle => BattleCameraHandler.Instance.ParticleCamera,
                CameraType.PuzzleParticle => BattleCameraHandler.Instance.PuzzleParticleCamera,
                _ => null,
            };

        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            if (!obj)
                return;
            obj.layer = layer;
            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject,  layer);
        }
    }
}