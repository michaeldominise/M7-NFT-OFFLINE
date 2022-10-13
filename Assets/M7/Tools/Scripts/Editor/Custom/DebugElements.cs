using Fasterflect;
using M7.FX.VFX.Scripts;
using M7.Skill;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Creates debug elements for objects
/// This is so that debug texts/elements would be
/// separated from runtime codes
/// </summary>
public static class DebugElements
{
    public static VisualElement Get<T>(T obj) where T : UnityEngine.Object
    {
        if(obj is TargetManager_CharacterInstance)
        {
            return TargetManagerCharacterInstance(obj as TargetManager_CharacterInstance);
        }

        if(obj is TargetManager_Team)
        {
            return TargetManagerTeam(obj as TargetManager_Team);
        }

        if(obj is TargetManager_MatchGridCell)
        {
            return TargetManagerMatchGridTile(obj as TargetManager_MatchGridCell);
        }

        if(obj is VFXSkillSystem)
        {
            return VFX(obj as VFXSkillSystem);
        }

        return Default(obj);
    }

    private static VisualElement TargetManagerCharacterInstance(TargetManager_CharacterInstance obj)
    {
        var root = Root();

        var filterLabel = new Label("Filters");
        filterLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        var defaultFilters = new Label();
        defaultFilters.text =
            $"Team: {obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("teamFilter")} | " +
            $"Selection: {obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("selectionFilter")} | " +
            $"Hp Status: {obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("selectionFilter")} | " +
            $"Element: {obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("elementFilter")}";
        var customFilters = new Label();
        customFilters.text = $"{obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("targetFilterItemCustomList").GetPropertyValue("Count")} Custom Filters";
        var sorters = new Label();
        sorters.text = $"{obj.GetFieldValue("sorter").GetFieldValue("sorterItems").GetPropertyValue("Count")} Sorters";

        root.Add(filterLabel);
        root.Add(defaultFilters);
        root.Add(customFilters);
        root.Add(sorters);

        return root;
    }

    private static VisualElement TargetManagerTeam(TargetManager_Team obj)
    {
        var root = Root();

        var filterLabel = new Label("Filters");
        filterLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        var defaultFilters = new Label();
        defaultFilters.text =
            $"Team: {obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("teamFilter")}";
        var customFilters = new Label();
        customFilters.text = $"{obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("targetFilterItemCustomList").GetPropertyValue("Count")} Custom Filters";
        var sorters = new Label();
        sorters.text = $"{obj.GetFieldValue("sorter").GetFieldValue("sorterItems").GetPropertyValue("Count")} Sorters";

        root.Add(filterLabel);
        root.Add(defaultFilters);
        root.Add(customFilters);
        root.Add(sorters);

        return root;
    }

    private static VisualElement TargetManagerMatchGridTile(TargetManager_MatchGridCell obj)
    {
        var root = Root();

        var filterLabel = new Label("Filters");
        filterLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        var defaultFilters = new Label();
        defaultFilters.text =
            $"Element: {obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("elementFilter")} | " +
            $"Pattern: {obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("patternFilter")} | " +
            $"Selection: {obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("selectionFilter")}";
        var customFilters = new Label();
        customFilters.text = $"{obj.GetFieldValue("filter").GetFieldValue("filter").GetFieldValue("targetFilterItemCustomList").GetPropertyValue("Count")} Custom Filters";
        var sorters = new Label();
        sorters.text = $"{obj.GetFieldValue("sorter").GetFieldValue("sorterItems").GetPropertyValue("Count")} Sorters";

        root.Add(filterLabel);
        root.Add(defaultFilters);
        root.Add(customFilters);
        root.Add(sorters);

        return root;
    }

    public static VisualElement SkillEffect(SkillData_MultipleEffect obj, int number)
    {
        var root = Root();

        var skillEffectNumberLabel = new Label($"# {number} [{obj.GetFieldValue("effectId")}]");
        skillEffectNumberLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

        var targetManagerLabel = new Label();
        targetManagerLabel.text =
            $"Target Manager: {obj.GetFieldValue("targetManager") ?? "None"}";

        var statusEffectsCountLabel = new Label($"{obj.GetFieldValue("statusEffects").GetPropertyValue("Count")} Status Effects");


        root.Add(skillEffectNumberLabel);
        root.Add(targetManagerLabel);
        root.Add(statusEffectsCountLabel);

        return root;
    }

    public static VisualElement StatusEffect(StatusEffect obj, int number)
    {
        var root = Root();

        var statusEffectNumberLabel = new Label($"# {number} Status Effect");
        statusEffectNumberLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

        var effectName = new Label($"{obj.name}");
        root.Add(statusEffectNumberLabel);
        root.Add(effectName);

        return root;
    }

    public static VisualElement VFX(VFXSkillSystem obj)
    {
        var root = Root();
        var vfxNameLabel = new Label($"{obj.name}");

        root.Add(vfxNameLabel);

        return root;
    }

    private static VisualElement Default(Object obj)
    {
        return new Label($"No debug element for this type: {obj.GetType()}");
    }

    private static VisualElement Root()
    {
        var root = new VisualElement();
        root.style.marginTop = 5;
        root.style.marginBottom = 5;
        root.style.marginLeft = 5;
        root.style.marginRight = 5;

        return root;
    }
}
