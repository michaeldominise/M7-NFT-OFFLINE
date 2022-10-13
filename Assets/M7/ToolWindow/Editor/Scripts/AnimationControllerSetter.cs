#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Animations;
    using UnityEngine;

    // 
    // With our custom RPG Editor window, this ScriptableObjectCreator is a replacement for the [CreateAssetMenu] attribute Unity provides.
    // 
    // For instance, if we call ScriptableObjectCreator.ShowDialog<Item>(..., ...), it will automatically find all 
    // ScriptableObjects in your project that inherits from Item and prompts the user with a popup where he 
    // can choose the type of item he wants to create. We then also provide the ShowDialog with a default path,
    // to help the user create it in a specific directory.
    // 

    [Serializable]
    public class AnimationControllerSetter
    {
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] AnimationClip[] animationClips;

        [Button]
        public void ClearAnimations() => animationClips = new AnimationClip[0];

        [Button]
        public void SetAnimations()
        {
            animatorOverrideController["attack"] = animationClips.FirstOrDefault(x => x.name.ToLower() == "attack");
            animatorOverrideController["death"] = animationClips.FirstOrDefault(x => x.name.ToLower() == "death");
            animatorOverrideController["hit"] = animationClips.FirstOrDefault(x => x.name.ToLower() == "hit");
            animatorOverrideController["idle"] = animationClips.FirstOrDefault(x => x.name.ToLower() == "idle");
            animatorOverrideController["run"] = animationClips.FirstOrDefault(x => x.name.ToLower() == "run");

            var skilAnimation = animationClips.FirstOrDefault(x => x.name.ToLower() == "skill");
            animatorOverrideController["skill"] = skilAnimation != null ? skilAnimation : animationClips.FirstOrDefault(x => x.name.ToLower() == "attack");

            var skilLegendaryAnimation = animationClips.FirstOrDefault(x => x.name.ToLower() == "skill-legendary");
            animatorOverrideController["skill-legendary"] = skilLegendaryAnimation != null ? skilLegendaryAnimation : animationClips.FirstOrDefault(x => x.name.ToLower() == "attack");

            EditorUtility.SetDirty(animatorOverrideController);
        }
    }
}
#endif
