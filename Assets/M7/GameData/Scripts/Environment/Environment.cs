using M7.GameData;
using M7.Skill;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using PathologicalGames;
using Sirenix.OdinInspector;
using M7.GameRuntime;

namespace M7.GameData
{
    public class Environment : MonoBehaviour
    {
        public static Environment Instance { get; private set; }

        [SerializeField] List<EnvironmentItem> items;
        [SerializeField] AnimationCurve blendCurve;
        public float horizontalOffset;
        [ShowInInspector] public string EnvironmentId => name;
        [ShowInInspector] Camera TargetCamera { get; set; }

        private void Awake()
        {
            Instance = this;
            InitRendererMaterial();
        }

        public void Init(Camera targetCamera) => TargetCamera = targetCamera;

        private void LateUpdate() => items.ForEach(x =>
        {
            if (!TargetCamera)
                return;
            x.UpdateOffset(TargetCamera.transform.position.x - horizontalOffset);
            if (WaveTransitionManager.Instance)
            {
                var pingpongVal = Mathf.PingPong((TargetCamera.transform.position.x - WaveTransitionManager.Instance.NextWaveDistance) / WaveTransitionManager.Instance.NextWaveDistance, 1);
                x.UpdateBlend(blendCurve.Evaluate(pingpongVal));
            }
        });

        [Button]
        void InitRendererMaterial() => items.ForEach(x => x.InitRendererMaterial());
    }
}