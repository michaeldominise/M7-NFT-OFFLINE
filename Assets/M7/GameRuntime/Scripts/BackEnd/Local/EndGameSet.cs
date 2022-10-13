using M7.GameData;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameSet : MonoBehaviour
{
    public void SetGaianite()
    {
        var gaianiteCalc = new GaianiteCalculator();

        var result_today = PlayerDatabase.Inventories.SystemCurrencies.FindItem(x => x.MasterID.Equals("Gaianite_Today"));
        var result_gaiTotal = PlayerDatabase.Inventories.SystemCurrencies.FindItem(x => x.MasterID.Equals("Gaianite"));

        var selectedTeamName = PlayerDatabase.Teams.selectedTeamName;
        var teamDataList = PlayerDatabase.Teams.teamDataList.Find(x => x.TeamName == selectedTeamName);

        if (!gaianiteCalc.MetMaxCap())
        {
            var gaianiteCollection = gaianiteCalc.ComputeGaiRate(PlayerDatabase.Inventories.Characters, PlayerDatabase.GlobalDataSetting.StageAllDataSettings.StageDataSettings, PlayerDatabase.CampaignData, PlayerDatabase.Teams);
            var gaianiteClaimable = gaianiteCalc.ComputeSumClaimable(gaianiteCollection);
            var gaianiteToday = gaianiteCalc.ComputeSumToday(gaianiteCollection);

            var characterDataCollection = PlayerDatabase.Inventories.Characters.GetItems();
            for (int i = 0; i < characterDataCollection.Count; i++)
            {
                for (int x = 0; x < teamDataList.Waves[0].SaveableCharacters.Count; x++)
                {
                    if (characterDataCollection[i].InstanceID == teamDataList.Waves[0].SaveableCharacters[x].ToString())
                    {
                        var hpValueIdx = 0;
                        if ((int)characterDataCollection[i].SaveableStats.Hp > PlayerDatabase.DurabilityCostSetting.DurabilityCost.hpValue.Length)
                        {
                            hpValueIdx = PlayerDatabase.DurabilityCostSetting.DurabilityCost.hpValue.Length;
                        }
                        else
                        {
                            hpValueIdx = Array.IndexOf(PlayerDatabase.DurabilityCostSetting.DurabilityCost.hpValue, (int)characterDataCollection[i].SaveableStats.Hp);
                        }

                        var durCost = PlayerDatabase.DurabilityCostSetting.DurabilityCost.durabilityCost[hpValueIdx];
                        var roundedDurCost = Math.Floor(durCost);
                        characterDataCollection[i].SaveableStats.MinusValues((float)roundedDurCost);

                        var charItem = JsonConvert.SerializeObject(characterDataCollection[i]);
                        PlayerDatabase.Inventories.Characters.OverwriteValues(charItem);

                        //Updates.Add(update.Inc(x => x.metadata.saveableStats.durability, -durCost));

                        //NftCharacterFunctions.UpdateNftCharacter(characterDataCollection[i].instanceID, encryptedHashID, update.Combine(Updates));

                    }
                }
            }

            result_gaiTotal.Amount = gaianiteClaimable;
            result_today.Amount = gaianiteToday;

            PlayerDatabase.Inventories.SystemCurrencies.FindItem(item => item == result_today).Amount = result_today.Amount;
            PlayerDatabase.Inventories.SystemCurrencies.FindItem(item => item == result_gaiTotal).Amount = result_gaiTotal.Amount;

            //var totalCubesDestroyed = JsonConvert.SerializeObject(gameFinish);
            //var sysCurrencies = JsonConvert.SerializeObject(systemCurrencyCollection);

            //var setTotalCube = DataRequest.UpdateUserDataRequest(context, new Dictionary<string, string> { { PlayerDataKeys.gameFinishDataKey, totalCubesDestroyed }, { PlayerDataKeys.systemCurrencyKey, sysCurrencies } });

            //await serverApi.UpdateUserDataAsync(setTotalCube);
            //return new OkObjectResult(gaianiteCollection);
        }
    }
}
