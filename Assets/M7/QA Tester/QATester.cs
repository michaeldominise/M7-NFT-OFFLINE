using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameData;
using Newtonsoft.Json;
using static M7.Skill.SkillEnums;
using System;
using System.IO;
using TMPro;
using System.Linq;

namespace M7.QATester
{
#if UNITY_EDITOR

    [Serializable]
    public class RarityStatRange
    {
        public RarityFilter rarity;
        public StatRange statRange;
    }

    [Serializable]
    public class StatRange
    {
        public int stat_min;
        public int stat_max;
    }

    [Serializable]
    public class HeroElement
    {
        public string masterId;
        public ElementDetails elementDetails;
    }

    [Serializable]
    public class ElementDetails
    {
        public string name;
        public ElementFilter element;
        public RarityFilter rarity;
        public string skillId;
        public int skillCost;
    }

    public class QATester : MonoBehaviour
    {
        #region Public Variables
        /// <summary>
        /// Use Hero Ingame Json Data as is instead of using M7 Puzzle's Scriptable Object: SaveableCharacterData 
        /// </summary>
        [Header("MetaData")]
        public bool useMetaDataAsIs = false;

        [Header("Directory")]
        /// <summary>
        /// The directory for the metaData to read
        /// </summary>
        public string metaDataDirectory = "";

        /// <summary>
        /// The directory for us to write the Names
        /// </summary>
        public string namesDirectory = "";

        /// <summary>
        /// The directory for us to write the errors
        /// </summary>
        public string errorsDirectory = "";

        [Header("UI")]
        /// <summary>
        /// The Instance ID Text to show result
        /// </summary>
        public TextMeshProUGUI InstanceIDText;

        /// <summary>
        /// The rarityStats Text to show result
        /// </summary>
        public TextMeshProUGUI rarityStatsText;

        /// <summary>
        /// The elementName Text to show result
        /// </summary>
        public TextMeshProUGUI elementNameText;

        /// <summary>
        /// The hero Skill according to the hero's rarity Text to show result
        /// </summary>
        public TextMeshProUGUI heroSkillText;

        /// <summary>
        /// The hero Skill Cost according to the hero's rarity Text to show result
        /// </summary>
        public TextMeshProUGUI heroSkillCostText;

        /// <summary>
        /// The equipmentId Text to show result
        /// </summary>
        public TextMeshProUGUI equipmentIdText;

        /// <summary>
        /// The socket 1 Attribute Count Details
        /// </summary>
        public TextMeshProUGUI socket1Text;

        /// <summary>
        /// The socket 2 Attribute Count Details
        /// </summary>
        public TextMeshProUGUI socket2Text;

        /// <summary>
        /// The socket 3 Attribute Count Details
        /// </summary>
        public TextMeshProUGUI socket3Text;

        /// <summary>
        /// The socket 4 Attribute Count Details
        /// </summary>
        public TextMeshProUGUI socket4Text;

        /// <summary>
        /// The rarity Count Text
        /// </summary>
        public TextMeshProUGUI rarityCountText;

        /// <summary>
        /// The result for redundancy check for Display name
        /// </summary>
        public TextMeshProUGUI displayNameResultText;

        /// <summary>
        /// The result for redundancy check for Instance Id
        /// </summary>
        public TextMeshProUGUI instanceIdResultText;

        /// <summary>
        /// The result for finding the Longest Display Name Adjective along with its Length
        /// </summary>
        public TextMeshProUGUI longestDisplayNameAdjectiveText;


        [Header("Longest Display Name Adjective")]
        public string LongestDisplayNameAdjective = "";
        public int LongestDisplayNameAdjectiveLenght = 0;

        [Header("Criteria")]
        [SerializeField]
        /// <summary>
        /// List of Rarity Stat Range Criteria to be parse into a Dictionary for QA purposes
        /// </summary>
        public List<RarityStatRange> rarityStatRanges = new List<RarityStatRange>() { };

        /// <summary>
        /// Dictionary that contains the Stat Ranges according to its Rarity
        /// </summary>
        public Dictionary<RarityFilter, StatRange> rarityStatRangesDictionary = new Dictionary<RarityFilter, StatRange>() { };

        /// <summary>
        /// Dictionary that contains the Counts according to its Rarity
        /// </summary>
        public Dictionary<RarityFilter, int> rarityCounts = new Dictionary<RarityFilter, int>() { };

        /// <summary>
        /// List of Hero with their Corresponding Element to be parse into a Dictionary for QA purposes
        /// </summary>
        public List<HeroElement> heroElement = new List<HeroElement>() { };

        /// <summary>
        /// Dictionary that contains the Element Details according to the masterId
        /// </summary>
        public Dictionary<string, ElementDetails> heroElementDictionary = new Dictionary<string, ElementDetails>() { };
        #endregion

        #region Private Variables
        private SaveableCharacterData saveableCharacterData;
        private HeroIngameJsonData heroIngameJsonData;
        private string nameOutputDirectory => namesDirectory + "/nameOutput.txt";
        private string errorOutputDirectory => errorsDirectory + "/errorOutput.txt";
        private int index = 0;
        private string currentIndexString = "";
        private string currentJsonPath = "";
        private string filePath = "hero_ingame_";
        private string fileExtention = ".json";
        private List<string> errors = new List<string>() { };
        private List<string> displayNames = new List<string>() { };
        private List<string> displayNamesAdjectives = new List<string>() { };
        private List<string> instanceIds = new List<string>() { };
        private int socket1_attack = 0;
        private int socket1_luck = 0;
        private int socket1_passion = 0;
        private int socket1_hp = 0;
        private bool socket1_gemId_locked = true;
        private int socket2_attack = 0;
        private int socket2_luck = 0;
        private int socket2_passion = 0;
        private int socket2_hp = 0;
        private bool socket2_gemId_locked = true;
        private int socket3_attack = 0;
        private int socket3_luck = 0;
        private int socket3_passion = 0;
        private int socket3_hp = 0;
        private bool socket3_gemId_locked = true;
        private int socket4_attack = 0;
        private int socket4_luck = 0;
        private int socket4_passion = 0;
        private int socket4_hp = 0;
        private bool socket4_gemId_locked = true;
        #endregion

        #region Public Functions
        public void CheckRange()
        {
            index++;
            startCheckRange();
        }

        public void ContinueCheckingRange()
        {
            CheckRange();
        }
        #endregion

        #region Private Functions
        private void Awake()
        {
            foreach (var item in rarityStatRanges)
            {
                rarityStatRangesDictionary[item.rarity] = item.statRange;
                rarityCounts[item.rarity] = 0;
            }

            foreach (var item in heroElement)
            {
                heroElementDictionary[item.masterId] = item.elementDetails;
            }

            if (File.Exists(nameOutputDirectory))
                File.Delete(nameOutputDirectory);

            if (File.Exists(errorOutputDirectory))
                File.Delete(errorOutputDirectory);
        }

        private void Start()
        {
            //index = 10000;
            CheckRange();
        }


        private void startCheckRange()
        {
            setCurrentIndexString();
        }

        private void setCurrentIndexString()
        {
            currentIndexString = ("0000000" + index.ToString()).Substring(index.ToString().Length);
            getJsonPath();
        }

        private void getJsonPath()
        {
            currentJsonPath = metaDataDirectory + "/" + currentIndexString + "/" + filePath + currentIndexString + fileExtention;
            readJsonFile();
        }
        private void readJsonFile()
        {
            if (File.Exists(currentJsonPath))
            {
                string json = File.ReadAllText(currentJsonPath);
                deserialize(json);
            }
            else
            {
                checkDisplayNameRedundancy();
                checkIdRedundancy();
                displayLongestDisplayNameAdjective();
            }
        }

        private void deserialize(string json)
        {
            saveableCharacterData = JsonConvert.DeserializeObject<SaveableCharacterData>(json);
            heroIngameJsonData = JsonConvert.DeserializeObject<HeroIngameJsonData>(json);
            displayNames.Add(GetDisplayName());
            displayNamesAdjectives.Add(removeActualNames(GetDisplayName()));
            instanceIds.Add(GetInstanceId());
            InstanceIDText.text = (GetInstanceId());
            checkMinAttackRange();
            //StartCoroutine(loopCheck());
        }

        private string removeActualNames(string displayName)
        {
            return displayName.Replace(" Hale","").Replace(" Lily","").Replace(" Ellie","").Replace(" Naomi","").Replace(" Lexy","").Replace(" Oya", "").Replace(" Dune", "")
                .Replace(" Lady Luck", "").Replace(" Alicia", "").Replace(" Eliza", "").Replace(" Sabre", "").Replace(" Aya", "").Replace(" Clover", "").Replace(" Tink", "")
                .Replace(" Narcissa", "").Replace(" Brenda", "").Replace(" Holly", "").Replace(" Sheba", "").Replace(" Camellia", "").Replace(" Cadenza", "").Replace(" Kairi", "")
                .Replace(" Claire", "").Replace(" Michiko", "").Replace(" Valiant", "").Replace(" Jade", "").Replace(" Maven", "").Replace(" Silas", "").Replace(" Neil", "")
                .Replace(" Marcus", "").Replace(" Zephyr", "");
        }

        private void checkMinAttackRange()
        {
            if (rarityStatRangesDictionary[GetRarity()].stat_min <= GetBaseAttack())
            {
                rarityStatsText.color = Color.white;
                rarityStatsText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : min Attack is CORRECT";
            }
            else
            {
                rarityStatsText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has underlap the Attack minimum range according to its rarity: {GetRarity()} : {rarityStatRangesDictionary[GetRarity()].stat_min}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                rarityStatsText.text = errorResult;
            }
            checkMaxAttackRange();
        }

        private void checkMaxAttackRange()
        {
            if (rarityStatRangesDictionary[GetRarity()].stat_max >= GetBaseAttack())
            {
                rarityStatsText.color = Color.white;
                rarityStatsText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : max Attack is CORRECT";
            }
            else
            {
                rarityStatsText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has overlap the Attack maximum range according to its rarity: {GetRarity()} : {rarityStatRangesDictionary[GetRarity()].stat_max}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                rarityStatsText.text = errorResult;
            }
            checkMinLuckRange();
        }

        private void checkMinLuckRange()
        {
            if (rarityStatRangesDictionary[GetRarity()].stat_min <= (GetBaseLuck()))
            {
                rarityStatsText.color = Color.white;
                rarityStatsText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : min Luck is CORRECT";
            }
            else
            {
                rarityStatsText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has underlap the Luck minimum range according to its rarity: {GetRarity()} : {rarityStatRangesDictionary[GetRarity()].stat_min}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                rarityStatsText.text = errorResult;
            }
            checkMaxLuckRange();
        }

        private void checkMaxLuckRange()
        {
            if (rarityStatRangesDictionary[GetRarity()].stat_max >= (GetBaseLuck()))
            {
                rarityStatsText.color = Color.white;
                rarityStatsText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : max Luck is CORRECT";
            }
            else
            {
                rarityStatsText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has overlap the Luck maximum range according to its rarity: {GetRarity()} : {rarityStatRangesDictionary[GetRarity()].stat_max}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                rarityStatsText.text = errorResult;
            }
            checkMinPassionRange();
        }

        private void checkMinPassionRange()
        {
            if (rarityStatRangesDictionary[GetRarity()].stat_min <= GetBasePassion())
            {
                rarityStatsText.color = Color.white;
                rarityStatsText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : min Passion is CORRECT";
            }
            else
            {
                rarityStatsText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has underlap the Passion minimum range according to its rarity: {GetRarity()} : {rarityStatRangesDictionary[GetRarity()].stat_min}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                rarityStatsText.text = errorResult;
            }
            checkMaxPassionRange();
        }

        private void checkMaxPassionRange()
        {
            if (rarityStatRangesDictionary[GetRarity()].stat_max >= GetBasePassion())
            {
                rarityStatsText.color = Color.white;
                rarityStatsText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : max Passion is CORRECT";
            }
            else
            {
                rarityStatsText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has overlap the Passion maximum range according to its rarity: {GetRarity()} : {rarityStatRangesDictionary[GetRarity()].stat_max}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                rarityStatsText.text = errorResult;
            }
            checkMinHpRange();
        }

        private void checkMinHpRange()
        {
            if (rarityStatRangesDictionary[GetRarity()].stat_min <= GetBaseHP())
            {
                rarityStatsText.color = Color.white;
                rarityStatsText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : min Hp is CORRECT";
            }
            else
            {
                rarityStatsText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has underlap the Hp mainimum range according to its rarity: {GetRarity()} : {rarityStatRangesDictionary[GetRarity()].stat_min}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                rarityStatsText.text = errorResult;
            }
            checkMaxHpRange();
        }

        private void checkMaxHpRange()
        {
            if (rarityStatRangesDictionary[GetRarity()].stat_max >= GetBaseHP())
            {
                rarityStatsText.color = Color.white;
                rarityStatsText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : max Hp is CORRECT";
            }
            else
            {
                rarityStatsText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has overlap the Hp maximum range according to its rarity: {GetRarity()} : {rarityStatRangesDictionary[GetRarity()].stat_max}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                rarityStatsText.text = errorResult;
            }
            checkElement();
        }

        private void checkElement()
        {
            if (GetElement() == heroElementDictionary[GetMasterID()].element)
            {
                elementNameText.color = Color.white;
                elementNameText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : element : {GetElement()} is CORRECT";
            }
            else
            {
                elementNameText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : element : {GetElement()} is WRONG, its suppose to be : element : {heroElementDictionary[GetMasterID()].element}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                elementNameText.text = errorResult;
            }
            checkHeroSkillRarity();
        }

        private void checkHeroSkillRarity()
        {
            if(GetSkillID() == heroElementDictionary[GetMasterID()].skillId)
            {
                heroSkillText.color = Color.white;
                heroSkillText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : skill : {GetSkillID()} is CORRECT";
            }
            else
            {
                heroSkillText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : skill : {GetSkillID()} is WRONG for the hero, its suppose to be : skill : {heroElementDictionary[GetMasterID()].skillId}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                heroSkillText.text = errorResult;
            }
            string name = removeActualNames(GetDisplayName());
            if (name.Length > LongestDisplayNameAdjectiveLenght)
            {
                LongestDisplayNameAdjectiveLenght = name.Length;
                LongestDisplayNameAdjective = name;
            }

            checkHeroSkillCost();
        }

        private void checkHeroSkillCost()
        {
            if(GetSkillCost() == heroElementDictionary[GetMasterID()].skillCost)
            {
                heroSkillCostText.color = Color.white;
                heroSkillCostText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} : skillCost : {GetSkillCost()} is CORRECT";
            }
            else
            {

                heroSkillCostText.color = Color.red;
                string errorResult = $"{currentIndexString} :  {GetMasterID()} : {GetDisplayName()}  : skillCost : {GetSkillCost()} is WRONG for the hero, its suppose to be : skillCost : {heroElementDictionary[GetMasterID()].skillCost}";
                Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                heroSkillCostText.text = errorResult;
            }
            checkHeroSocket();
        }

        private void checkHeroSocket()
        {
            for (int i = 0; i < GetSocketsDatas().ToArray().ToArray().Length; i++)
            {
                if(i == 0)
                {
                    if (GetSocketsDatas().ToArray().ToArray()[i].statsType == "Attack")
                    {
                        socket1_attack++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Luck")
                    {
                        socket1_luck++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Passion")
                    {
                        socket1_passion++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Hp")
                    {
                        socket1_hp++;
                    }
                    
                    if (GetSocketsDatas().ToArray().ToArray()[i].gemId == "locked")
                    {
                        socket1Text.color = Color.white;
                        socket1_gemId_locked = true;
                    }
                    else
                    {
                        socket1Text.color = Color.red;
                        socket1_gemId_locked = false;
                    }
                }
                else if(i == 1)
                {
                    if (GetSocketsDatas().ToArray()[i].statsType == "Attack")
                    {
                        socket2_attack++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Luck")
                    {
                        socket2_luck++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Passion")
                    {
                        socket2_passion++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Hp")
                    {
                        socket2_hp++;
                    }
                    if (GetSocketsDatas().ToArray().ToArray()[i].gemId == "locked")
                    {
                        socket2Text.color = Color.white;
                        socket2_gemId_locked = true;
                    }
                    else
                    {
                        socket2Text.color = Color.red;
                        socket2_gemId_locked = false;
                    }
                }
                else if(i == 2)
                {
                    if (GetSocketsDatas().ToArray()[i].statsType == "Attack")
                    {
                        socket3_attack++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Luck")
                    {
                        socket3_luck++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Passion")
                    {
                        socket3_passion++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Hp")
                    {
                        socket3_hp++;
                    }
                    if (GetSocketsDatas().ToArray().ToArray()[i].gemId == "locked")
                    {
                        socket3Text.color = Color.white;
                        socket3_gemId_locked = true;
                    }
                    else
                    {
                        socket3Text.color = Color.red;
                        socket3_gemId_locked = false;
                    }
                }
                else if(i == 3)
                {
                    if (GetSocketsDatas().ToArray()[i].statsType == "Attack")
                    {
                        socket4_attack++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Luck")
                    {
                        socket4_luck++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Passion")
                    {
                        socket4_passion++;
                    }
                    else if (GetSocketsDatas().ToArray()[i].statsType == "Hp")
                    {
                        socket4_hp++;
                    }
                    if (GetSocketsDatas().ToArray().ToArray()[i].gemId == "locked")
                    {
                        socket4Text.color = Color.white;
                        socket4_gemId_locked = true;
                    }
                    else
                    {
                        socket4Text.color = Color.red;
                        socket4_gemId_locked = false;
                    }
                }
            }
            socket1Text.text = $"socket 1: Attack: {socket1_attack} - Luck: {socket1_luck} - Passion: {socket1_passion} - Hp: {socket1_hp} - gemId is locked : {socket1_gemId_locked}";
            socket2Text.text = $"socket 2: Attack: {socket2_attack} - Luck: {socket2_luck} - Passion: {socket2_passion} - Hp: {socket2_hp} - gemId is locked : {socket2_gemId_locked}";
            socket3Text.text = $"socket 3: Attack: {socket3_attack} - Luck: {socket3_luck} - Passion: {socket3_passion} - Hp: {socket3_hp} - gemId is locked : {socket3_gemId_locked}";
            socket4Text.text = $"socket 4: Attack: {socket4_attack} - Luck: {socket4_luck} - Passion: {socket4_passion} - Hp: {socket4_hp} - gemId is locked : {socket4_gemId_locked}";
            setRarityCount();
        }

        private void setRarityCount()
        {
            rarityCounts[GetRarity()] = rarityCounts[GetRarity()] + 1;
            string rarityCountTextValue = "";
            foreach (var item in rarityCounts)
            {
                rarityCountTextValue = $"{rarityCountTextValue}{item.Key}: {item.Value} - ";
            }
            rarityCountText.text = rarityCountTextValue;
            checkEquipments();
        }

        private void checkEquipments()
        {
            string value = $"EquipmentItem_{GetName().Replace(" ", "")}";
            bool result = GetEquipments().All(x => x.Contains(value));
            if (result)
            {
                equipmentIdText.color = Color.white;
                equipmentIdText.text = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has all Equipment Ids contain: {value}";
            }
            else
            {
                equipmentIdText.color = Color.red;
                string errorResult = $"{currentIndexString} : {GetMasterID()} : {GetDisplayName()} has some Equipment Ids which does not contain: {value}"; Debug.Log(errorResult);
                errors.Add(errorResult);
                writeInText(errorOutputDirectory, errorResult);
                equipmentIdText.text = errorResult;
            }
            StartCoroutine(loopCheck(GetDisplayName()));
        }

        private IEnumerator loopCheck(string name)
        {
            writeInText(nameOutputDirectory, name);
            yield return new WaitForEndOfFrame();
            CheckRange();
        }

        private async void writeInText(string pathDirectory, string content)
        {
            using (StreamWriter writer = new StreamWriter(pathDirectory, true))
            {
                await writer.WriteAsync($"{content}\n");
                writer.Close();
            }
        }

        private void checkDisplayNameRedundancy()
        {
            string result;
            if (displayNames.Count == displayNames.Distinct().Count())
            {
                result = "Display Names does not contain any duplicates";
            }
            else
            {
                result = "Display Names contain duplicates";
            }
            displayNameResultText.text = result;
        }
        private void checkIdRedundancy()
        {
            string result;
            if (instanceIds.Count == instanceIds.Distinct().Count())
            {
                result = "Instance ID does not contain any duplicates";
            }
            else
            {
                result = "Instance ID contain duplicates";
            }
            instanceIdResultText.text = result;
        }

        private void displayLongestDisplayNameAdjective()
        {
            longestDisplayNameAdjectiveText.text = $"\"{LongestDisplayNameAdjective}\" is the Longest Adjective Name with the Lenght of {LongestDisplayNameAdjectiveLenght}";
        }

        private RarityFilter GetRarity() => useMetaDataAsIs ? heroIngameJsonData.rarity : saveableCharacterData.rarity;
        private int GetBaseAttack() => (int)(useMetaDataAsIs ? heroIngameJsonData.baseStats.attack : saveableCharacterData.BaseStats.Attack);
        private int GetBaseLuck() => (int)(useMetaDataAsIs ? heroIngameJsonData.baseStats.luck : saveableCharacterData.BaseStats.Luck);
        private int GetBasePassion() => (int)(useMetaDataAsIs ? heroIngameJsonData.baseStats.passion : saveableCharacterData.BaseStats.Passion);
        private int GetBaseHP() => (int)(useMetaDataAsIs ? heroIngameJsonData.baseStats.hp : saveableCharacterData.BaseStats.Hp);
        private string GetMasterID() => useMetaDataAsIs? heroIngameJsonData.masterID : saveableCharacterData.MasterID;
        private string GetDisplayName() => useMetaDataAsIs ? heroIngameJsonData.displayName : saveableCharacterData.DisplayName;
        private string GetInstanceId() => useMetaDataAsIs ? heroIngameJsonData.instanceID : saveableCharacterData.InstanceID;
        private ElementFilter GetElement() => (useMetaDataAsIs ? heroIngameJsonData.element : saveableCharacterData.elementType);
        private string GetSkillID() => (useMetaDataAsIs ? heroIngameJsonData.skillId : saveableCharacterData.skillId);
        private int GetSkillCost() => useMetaDataAsIs ? heroIngameJsonData.skillCost : saveableCharacterData.skillCost;
        private SocketsData[] GetSocketsDatas() => useMetaDataAsIs ? heroIngameJsonData.sockets : saveableCharacterData.Sockets;
        private string[] GetEquipments() => useMetaDataAsIs ? heroIngameJsonData.equipmentIDs : saveableCharacterData.EquipmentIDs;
        private string GetName() => heroElementDictionary[GetMasterID()].name;
        #endregion
    }
#endif
}