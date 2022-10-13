using M7.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class ServerTester : MonoBehaviour
{
    [SerializeField] private MaxGaiData _maxGaiData = new MaxGaiData();
    public void Init()
    {
        TestLevelComputation(PlayerDatabase.Inventories.Characters.GetItems());
    }
    void TestLevelComputation(List<SaveableCharacterData> saveableCharacterDatas)
    {
        var highestLevel = saveableCharacterDatas.Max(x => x.Level);
        //int adjustment = 1;
        int multiplier = 5;

        int curTotalLevel = highestLevel;
        _maxGaiData.Amount = curTotalLevel * multiplier;
        SaveFileToJson(_maxGaiData, "MaxGaianite");
    }

    void SaveFileToJson(MaxGaiData maxGaiData, string fileName)
    {
        string maxGaianite = JsonUtility.ToJson(maxGaiData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + fileName +".json", maxGaianite);
    }

    public void FetchJson(string jsonPath, string jsonFileName)
    {
        string jsonString = System.IO.File.ReadAllText(jsonPath + "/" + jsonFileName + ".json");
        SetToPlayerDatabase(jsonString, jsonFileName);
    }

    void SetToPlayerDatabase(string jsonData, string jsonFileName)
    {
        switch (jsonFileName)
        {
            case "CollectedGai":
                var collectedGai = PlayerDatabase.Inventories.Currencies.FindItem("Gaianite_TotalCollectedItem");
                collectedGai.OverwriteValues(jsonData);
                break;
            case "MaxGaianite":
                var maxGai = PlayerDatabase.Inventories.Currencies.FindItem("Gaianite_MaxTotalItem");
                maxGai.OverwriteValues(jsonData);
                break;
        }
    }
}
[System.Serializable]
public class MaxGaiData
{
    public float Amount;
}
public class CollectedGaiData
{
    public float Amount;
}
