using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using ExitGames.Client.Photon.StructWrapping;
using Sirenix.OdinInspector;

namespace M7.FX
{
    [System.Serializable]
    public class VfxTargetData
    {
        public enum TargetType
        {
            None, TargetPosition, CenterRenderer, CenterScreen, Left, Top, CenterPuzzleBoard
        }

        [SerializeField] protected TargetType _targetType;
        [SerializeField] protected float _effectDelay;
        [SerializeField] protected float _SFXDelay = 0;
        [SerializeField] protected string sfxName;
        protected bool isInitialized;

        public float effectDelay => _effectDelay;

        public virtual void Init(bool flip)
        {
            Reset();
            if (isInitialized)
                return;

            isInitialized = true;
        }

        public virtual void Reset()
        {
        }

        public virtual Vector3[] GetTargetParticlePosition(Transform[] targets, ParticleWorldManager.CameraType fromCameraType, ParticleWorldManager.CameraType toCameraType, bool useTargetWorldPosIfPossible = true)
        {
            Vector3[] targetPos = null;
            switch (_targetType)
            {
                case TargetType.TargetPosition:
                    targetPos = targets.Select(data =>
                    {
                        if (toCameraType == ParticleWorldManager.CameraType.World && useTargetWorldPosIfPossible)
                            return data.position;
                        else
                            return ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(data.position, fromCameraType, toCameraType);
                    }).ToArray();
                    break;

                case TargetType.CenterRenderer:
                    targetPos = targets.Select(data =>
                    {         
                        if (toCameraType == ParticleWorldManager.CameraType.World && useTargetWorldPosIfPossible)
                            return data.position;
                        else
                            return ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(data.position, fromCameraType, toCameraType);
                    }).ToArray();
                    break;

                case TargetType.CenterScreen:
                    Vector3 currentPosition = Vector3.zero;

                    targetPos = targets.Select(data => ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(currentPosition, ParticleWorldManager.CameraType.Particle)).ToArray();
                    break;
                case TargetType.Left:
                    targetPos = targets.Select(data =>
                    {
                        var pos = data.position;
                        pos.x = ParticleWorldManager.Instance.BoosterLaserGunContainerPosition.x;
                        if (toCameraType == ParticleWorldManager.CameraType.World && useTargetWorldPosIfPossible)
                        {
                            return pos;
                        }

                        return ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(pos, fromCameraType, toCameraType);
                    }).ToArray();
                    break;
                case TargetType.Top:
                    targetPos = targets.Select(data =>
                    {
                        var pos = data.position;
                        pos.y = ParticleWorldManager.Instance.BoosterLaserSwordContainerPosition.y;
                        if (toCameraType == ParticleWorldManager.CameraType.World && useTargetWorldPosIfPossible)
                        {
                            return pos;
                        }

                        return ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(pos, fromCameraType, toCameraType);
                    }).ToArray();
                    break;
                case TargetType.CenterPuzzleBoard:
                    targetPos = targets.Select(data =>
                    {
                        var pos = ParticleWorldManager.Instance.PuzzleBoardCenter;
                        if (toCameraType == ParticleWorldManager.CameraType.World && useTargetWorldPosIfPossible)
                        {
                            return pos;
                        }

                        return ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(pos, fromCameraType, toCameraType);
                    }).ToArray();
                    break;
            }

            return targetPos;
        }

        public virtual bool PlayEffect(int targetCount = 0)
        {
            return _targetType != TargetType.None;
        }

        public virtual bool PlayEffectPerProjectile(int targetCount = 0, float projectileTravelDuration = 0)
        {
            return _targetType != TargetType.None;
        }
    }
}