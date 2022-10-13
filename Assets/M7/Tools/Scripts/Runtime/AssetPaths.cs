using M7.GameData;

namespace M7.Tools.Utility
{
    public static class AssetPaths
    {
        public static string LevelDataPath = "Assets/M7/GameData/ScriptableObjects/Chapters/Adventure/Stages";
        public static string EnvironmentsPath = "Assets/M7/InGameAssets/Environments/Prefab";
        public static string ChapterDataPath = "Assets/M7/GameData/ScriptableObjects/Chapters/Adventure/Locations";
        public static string HeroSkillsPath = "Assets/M7/Skills/Prefab/Skills/Skills/Heroes";
        public static string EnemySkillsPath = "Assets/M7/Skills/Prefab/Skills/Skills/EnemySkills";

        public static string PathWithType(System.Type type)
        {
            if(type.IsAssignableFrom(typeof(LevelData)))
            {
                return LevelDataPath;
            }

            if(type.IsAssignableFrom(typeof(ChapterData)))
            {
                return ChapterDataPath;
            }

            return "";
        }
    }
}