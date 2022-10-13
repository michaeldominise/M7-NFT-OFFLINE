#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = System.Object;

public class SkillObjectReader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame   //public static BundleReader Instance { get { if (Instance == null) return new BundleReader(); else return Instance; }}
        private static char[] LINESEPARATOR { get { char[] val = { '\n', '\r' }; return val; } }
        static char COMMASEPARATOR = ',';
        static char TABSEPARATOR = '\t';

        static TextAsset cSVFile;
        static string jsonFileName;
        static bool isDownload;
        public static SkillObjectList bundleData { get; private set; }
        public static bool usePrettyPrint;

        static int nodeIndex = 2;

        static string Data;
        // Use this for initialization

        [Button]
        private static void StartCSVReader()
        {
            Data = cSVFile.text;
            ReadCSV(cSVFile.text, sub => { });
        }
        
        public static void ReadCSV(string Data, Action<SkillObjectList> Success)
        {
            
            string[] DataArray = Data.Split(LINESEPARATOR);

            string[] ColumnNames = DataArray[0].Split(',');
            foreach (string columnName in ColumnNames)
            {
                Debug.Log("column: " + columnName);
            }

            List<string> cleanList = new List<string>();

            for (int i = 1; i < DataArray.Length; i++)
            {
                string[] stringData = DataArray[i].Split(COMMASEPARATOR);

                string BundleName = stringData[0];

                if (BundleName.Length > 0)
                {
                    cleanList.Add(DataArray[i]);
                }

            }

            SkillObjectList list = new SkillObjectList();
            list.SkillObject = new SkillOjectCapsule[cleanList.Count];

            for (int i = 0; i < cleanList.Count; i++)
            {
                Debug.Log("count:" + cleanList.Count);

                string[] stringData = cleanList[i].Split(TABSEPARATOR);
                
                SkillOjectCapsule info = new SkillOjectCapsule();
                info.Hero = stringData[(int)ColumnId.Hero];
                info.Gear = stringData[(int)ColumnId.Gear];
                info.Element = stringData[(int) ColumnId.Element];
                info.Gear_Name = stringData[(int) ColumnId.Gear_Name];
                info.Rarity = stringData[(int) ColumnId.Rarity];
                info.Skill_Name = stringData[(int) ColumnId.Skill_Name];
                info.Skill_Illustration = stringData[(int) ColumnId.Skill_Illustration];
                info.Card_Type = stringData[(int) ColumnId.Card_Type];
                info.Cost = 0; int.TryParse(stringData[(int) ColumnId.Cost], out info.Cost);
                info.ATK = 0; int.TryParse(stringData[(int) ColumnId.ATK], out info.ATK);
                info.DEF = 0; int.TryParse(stringData[(int) ColumnId.DEF], out info.DEF);
                info.M7_Description = stringData[(int) ColumnId.M7_Description];
                list.SkillObject[i] = info;

            }
            bundleData = list;
            Debug.Log("data count:" + list.SkillObject.Length);
            Success(list);
        }

        public static void ConvertToJsonFile()
        {
            string json = JsonUtility.ToJson(bundleData, usePrettyPrint);

            string jsonOutputPath = Application.dataPath + "/BGGamesCore/BGPlayFab/CSVReader/Files/GeneratedJsonFiles/" + jsonFileName + ".json";
            File.WriteAllText(jsonOutputPath, json);
            Debug.Log(json);
        }

        private enum ColumnId
        {
            Hero,
            Gear,
            Element,
            Gear_Name,
            Rarity,
            Skill_Name,
            Skill_Illustration,
            Card_Type,
            Cost,
            ATK,
            DEF,
            M7_Description
        }
    }
#endif
