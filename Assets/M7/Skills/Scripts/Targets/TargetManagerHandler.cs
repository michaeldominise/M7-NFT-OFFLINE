using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace M7.Skill
{
    [Serializable]
    public class TargetManagerHandler
    {
        [SerializeField] public bool useManualSelection;
        [ValueDropdown("@M7.Skill.TargetManagerIndex.TreeViewOfTargetManagers")]
        [SerializeField, ShowIf("useManualSelection")] int manualSelectionTargetManagerIndex;
        [SerializeField] List<TargetManager> targetManagerList;

        public List<TargetManager> TargetManagerList { get => targetManagerList; set => targetManagerList = value; }
        public TargetManager GetTargetManager(int index) => index < targetManagerList.Count ? targetManagerList[(int)index] : null;
        public void GetTargetManagerData(Component caster, Action<List<Func<List<Component>>>> onFinish)
        {
            var targetManagerData = new List<Func<List<Component>>>();
            List<Component> selectionTargets = null;
            var manualSelectionTargetManager = useManualSelection ? GetTargetManager(manualSelectionTargetManagerIndex) : null;
            if (manualSelectionTargetManager != null && manualSelectionTargetManager is TargetManager_CharacterInstance)
            {
                CharacterSelectionTargetManager.Instance.StartSelectTargets(caster, manualSelectionTargetManager as TargetManager_CharacterInstance,
                    () =>
                    {
                        if (CharacterSelectionTargetManager.Instance.CurrentSelectedTargets != null && CharacterSelectionTargetManager.Instance.CurrentSelectedTargets.Count > 0)
                            selectionTargets = CharacterSelectionTargetManager.Instance.CurrentSelectedTargets.Select(x => x as Component).ToList();
                        SetTargetManagerData();
                        onFinish.Invoke(targetManagerData);
                    });
            }
            else
            {
                SetTargetManagerData();
                onFinish.Invoke(targetManagerData);
            }

            void SetTargetManagerData()
            {
                for (int i = 0; i < targetManagerList.Count; i++)
                {
                    var targetManager = targetManagerList[i];
                    var targets = targetManager.ValidateOnSkillDataExecute ? null : targetManager.GetTargets(caster);
                    targetManagerData.Add(i == 0 && selectionTargets != null ? () => selectionTargets : () => targetManager.ValidateOnSkillDataExecute ? targetManager.GetTargets(caster) : targets);
                }
            }
        }
    }

    public static class TargetManagerIndex
    {
#if UNITY_EDITOR
        public static SkillObject ActiveSkillObject => UnityEditor.Selection.activeGameObject.GetComponent<SkillObject>();
        public static IEnumerable TreeViewOfTargetManagers
        {
            get
            {
                var targetManagerList = ActiveSkillObject?.TargetManagerHandler.TargetManagerList;
                var dropDownList = new ValueDropdownList<int>();
                if (targetManagerList.Count > 0)
                {
                    for (int i = 0; i < targetManagerList.Count; i++)
                    {
                        TargetManager targetManager = targetManagerList[i];
                        dropDownList.Add($"{i}.{targetManager?.name ?? "null"}", i);
                    }
                }
                return dropDownList;
            }
        }
#endif
    }
}