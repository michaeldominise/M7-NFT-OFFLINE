public static class InspectorTag
{
    public static string SkillEffectsInitialTargetManager = "SE_ITM";
    public static string SingleTargetTargetManager = "SingleTarget_TargetManager";
    public static string SingleTargetVFX = "SingleTarget_VFX";
    public static string SingleTargetStatusEffect = "SingleTarget_StatusEffect";

    public static string InitialTargetManagerCustomFilters = "ITM_CustomFilters";

    public static string GetTag(string name)
    {
        var components = name.Split('.');
        return components.Length > 1 ? components[0] : "";
    }

    public static (string, string) GetNameComponents(string name)
    {
        var components = name.Split('.');
        var tag = components.Length > 1 ? components[0] : "";
        var objectName = components.Length > 1 ? components[1] : components[0];
        return (tag, objectName);
    }
}