using M7.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fasterflect;
using M7.Tools.Utility;
using UnityEngine.AddressableAssets;
using System.Linq;

namespace M7.Tools
{
    public class TSVTranslator
    {
        private static char TAB_SEPARATOR = '\t';

        public static IEnumerable Translate<T>(string tsvText) where T : Object
        {
            if (IsTypeEqual(typeof(LevelData), typeof(T)))
            {
                return TranslateToLevelData(tsvText);
            }

            if (IsTypeEqual(typeof(ChapterData), typeof(T)))
            {
                return TranslateToChapterData(tsvText);
            }

            if (IsTypeEqual(typeof(TeamData_Enemy), typeof(T)))
            {
                return TranslateToStageWave(tsvText);
            }

            return default;
        }

        private static bool IsTypeEqual(System.Type t1, System.Type t2)
        {
            return t1.IsAssignableFrom(t2);
        }

        private static List<LevelData> TranslateToLevelData(string tsvText)
        {
            List<LevelData> levelDataList = new List<LevelData>();
            var stages = AssetUtility.GetAssets<LevelData>("", new string[] { AssetPaths.LevelDataPath });
            var envs = AssetUtility.GetAssets<Environment>("", new string[] { AssetPaths.EnvironmentsPath });

            TSVReader.Read(tsvText, (lineNumber, line) =>
            {
                if (lineNumber == 0)
                {
                    return;
                }

                if (line == "")
                {
                    return;
                }

                string[] data = line.Split(TAB_SEPARATOR);

                LevelData levelData = stages.FirstOrDefault(x => x.name == data[0]) ?? ScriptableObject.CreateInstance<LevelData>();
                levelData.name = data[0];
                levelData.SetFieldValue("totalMoveCount", int.Parse(data[24]));

                //var goalData = (typeof(GoalData)).CreateInstance();
                //goalData.SetFieldValue("goalType", GoalData.GoalType.Cube);
                //goalData.SetFieldValue("goalCount", int.Parse(data[2]));

                //levelData.SetFieldValue("goalConditions", (typeof(GoalData)).MakeArrayType().CreateInstance(1));
                //levelData.GetFieldValue("goalConditions").SetElement(0, goalData);

                //var env = envs.Find(e => e.EnvironmentId == data[5]) ?? (envs.Count > 0 ? envs[0] : null);
                //string guid;
                //long file;
                //if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(env.gameObject, out guid, out file))
                //{
                //    levelData.SetFieldValue("environment", new AssetReferenceT<GameObject>(guid));
                //}

                levelDataList.Add(levelData);
            });

            return levelDataList;
        }

        private static List<ChapterData> TranslateToChapterData(string tsvText)
        {
            List<ChapterData> chapterDataList = new List<ChapterData>();
            var stages = AssetUtility.GetAssets<LevelData>("", new string[] { AssetPaths.LevelDataPath });

            TSVReader.Read(tsvText, (lineNumber, line) =>
            {
                if (lineNumber == 0)
                {
                    return;
                }

                if (line == "")
                {
                    return;
                }

                string[] data = line.Split(TAB_SEPARATOR);
                ChapterData chapterData = ScriptableObject.CreateInstance<ChapterData>();
                chapterData.name = data[0];
                chapterData.SetFieldValue("locationId", data[0]);
                chapterData.SetFieldValue("displayName", data[1]);

                var stageList = new List<LevelData>();
                var minStageIndex = int.Parse(data[2].Replace("Stage_", ""));
                var maxStageIndex = int.Parse(data[3].Replace("Stage_", ""));
                for (var i = minStageIndex; i <= maxStageIndex; i++)
                {
                    var stage = stages.FirstOrDefault(x => x.name == $"Stage_{i}");
                    if (stage)
                        stageList.Add(stage);
                }
                chapterData.SetFieldValue("levelDataList", stageList);
                // list suggestion in googlesheet

                chapterDataList.Add(chapterData);
            });

            return chapterDataList;
        }

        public static Dictionary<string, TeamData_Enemy> TranslateToStageWave(string tsvText)
        {
            Dictionary<string, TeamData_Enemy> stageWaveDataList = new Dictionary<string, TeamData_Enemy>();

            TSVReader.Read(tsvText, (lineNumber, line) =>
            {
                if (lineNumber == 0)
                {
                    return;
                }

                if (line == "")
                {
                    return;
                }

                string[] data = line.Split(TAB_SEPARATOR);
                TeamData_Enemy enemies;
                string[] ids = data[0].Split('_');
                string stageName = ids[0];

                if (stageWaveDataList.ContainsKey(stageName))
                {
                    enemies = stageWaveDataList[stageName];
                }
                else
                {
                    enemies = new TeamData_Enemy();
                    stageWaveDataList.Add(stageName, enemies);
                }

                WaveData_Enemy enemyWaveData = new WaveData_Enemy();

                for (int i = 0; i < 5; i++)
                {
                    int enemyIndex = i * 4 + 4;

                    SaveableCharacterData_Enemy enemy = new SaveableCharacterData_Enemy(
                        masterID: data[enemyIndex],
                        instanceID: "",
                        level: 0,
                        equipments: new string[] { });

                    if (data[enemyIndex] != "")
                    {
                        int hpIndex = enemyIndex + 1;
                        int attackIndex = enemyIndex + 2;
                        int turnIndex = enemyIndex + 3;
                        var stats = enemy.GetFieldValue("saveableStats");
                        if (data[hpIndex] != "")
                        {
                            stats.SetFieldValue("hp", float.Parse(data[hpIndex]));
                        }
                        if (data[attackIndex] != "")
                        {
                            stats.SetFieldValue("attack", float.Parse(data[attackIndex]));
                        }
                        if(data[turnIndex] != "")
                        {
                            enemy.SetFieldValue("attackTurn", int.Parse(data[turnIndex]));
                        }
                }


                    var characters = enemyWaveData.GetFieldValue("saveableCharacters");
                    characters.CallMethod("Add", enemy);
                }

                var stageWaveData = enemies.GetFieldValue("waves");
                stageWaveData.CallMethod("Add", enemyWaveData);
            });

            return stageWaveDataList;
        }

        public static List<List<string>> TranslateToStringsList(string tsvText)
        {
            List<List<string>> skillstringsList = new List<List<string>>();

            TSVReader.Read(tsvText, (lineNumber, line) =>
            {
                if (lineNumber == 0)
                {
                    return;
                }

                if (line == "")
                {
                    return;
                }

                List<string> strings = new List<string>();
                string[] data = line.Split(TAB_SEPARATOR);

                skillstringsList.Add(data.ToList());
            });

            return skillstringsList;
        }
    }
}