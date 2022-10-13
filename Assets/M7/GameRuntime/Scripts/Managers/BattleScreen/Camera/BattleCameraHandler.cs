using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    public class BattleCameraHandler : MonoBehaviour
    {
        public static BattleCameraHandler Instance => BattleManager.Instance.BattleCameraHandler;

        [SerializeField] BattleWorldCamera battleWorldCamera;
        [Header("Camera Prefs")]
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Camera particleCamera;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private Camera selectionCamera;
        [SerializeField] private Camera puzzleCamera;
        [SerializeField] private Camera puzzleParticleCamera;


        [HideInInspector] public int worldLayer;
        [HideInInspector] public int particleLayer;
        [HideInInspector] public int systemUILayer;
        [HideInInspector] public int selectionLayer;
        [HideInInspector] public int puzzleLayer;
        [HideInInspector] public int puzzleParticleLayer;

#if UNITY_EDITOR
        [FoldoutGroup("Layers"), ValueDropdown("LayerNames"), ShowInInspector] string WorldLayer { get => LayerMask.LayerToName(worldLayer); set => worldLayer = LayerMask.NameToLayer(value); }
        [FoldoutGroup("Layers"), ValueDropdown("LayerNames"), ShowInInspector] string ParticleLayer { get => LayerMask.LayerToName(particleLayer); set => particleLayer = LayerMask.NameToLayer(value); }
        [FoldoutGroup("Layers"), ValueDropdown("LayerNames"), ShowInInspector] string SystemUILayer { get => LayerMask.LayerToName(systemUILayer); set => systemUILayer = LayerMask.NameToLayer(value); }
        [FoldoutGroup("Layers"), ValueDropdown("LayerNames"), ShowInInspector] string SelectionLayer { get => LayerMask.LayerToName(selectionLayer); set => selectionLayer = LayerMask.NameToLayer(value); }
        [FoldoutGroup("Layers"), ValueDropdown("LayerNames"), ShowInInspector] string PuzzleLayer { get => LayerMask.LayerToName(puzzleLayer); set => puzzleLayer = LayerMask.NameToLayer(value); }
        [FoldoutGroup("Layers"), ValueDropdown("LayerNames"), ShowInInspector] string PuzzleParticleLayer { get => LayerMask.LayerToName(puzzleParticleLayer); set => puzzleParticleLayer = LayerMask.NameToLayer(value); }

        public string[] LayerNames
        {
            get
            {
                var layers = new string[32];
                layers[0] = "Nothing";
                for (var x = 0; x < 31; x++)
                    layers[x + 1] = LayerMask.LayerToName(x + 1);
                return layers;
            }
        }
#endif
        public BattleWorldCamera BattleWorldCamera => battleWorldCamera;
        public Camera WorldCamera => worldCamera;
        public Camera ParticleCamera => particleCamera;
        public Camera UiCamera => uiCamera;
        public Camera SelectionCamera => selectionCamera;
        public Camera PuzzleCamera => puzzleCamera;
        public Camera PuzzleParticleCamera => puzzleParticleCamera;
    }
}