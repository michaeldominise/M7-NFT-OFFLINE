public class SheetIDs
{
    public static string MasterListId => "1PXyv7LcuII8onVnkryjMbQugdpbooo9nOtjFTx2q0kI";
    /// <summary>
    /// Chapter Data / Locations
    /// </summary>
    public static string ChapterDataId => "584597723";
    /// <summary>
    /// Level Data / Stage Info
    /// </summary>
    public static string LevelDataId => "747056700";
    public static string LevelWaveId => "1363076992";
    public static string HeroSkillsId => "680781103";
    public static string EnemySkillsId => "1494656674";


    public static string GetFromType(System.Type t)
    {
        if (t.IsAssignableFrom(typeof(M7.GameData.LevelData)))
        {
            return LevelDataId;
        }

        if (t.IsAssignableFrom(typeof(M7.GameData.ChapterData)))
        {
            return ChapterDataId;
        }

        return "";
    }
}
