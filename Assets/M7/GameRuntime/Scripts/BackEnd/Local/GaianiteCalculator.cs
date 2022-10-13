using M7.GameData;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.Managers.BattleScreen;
using System;
using System.Collections.Generic;
using System.Linq;

public class GaianiteCalculator
{
    public float ComputeMaxCap(List<SaveableCharacterData> characterDataCollection)
    {
        var characters = characterDataCollection;

        var initialGaianite = 5;
        var multiplier = 5;

        if (characters.Count > 0)
        {
            var highestCharacters = characters.Max(t => t.Level);
            float level = highestCharacters;
            float initialCap = level * multiplier;
            float totalGaianiteCap = initialCap + initialGaianite;
            return totalGaianiteCap;
        }
        return 0;
    }

    public float ComputeGaiRate(InventoryData<SaveableCharacterData> characterDataCollection, List<StageDataSettings> stageData, CampaignData campaignData, PlayerTeams teams)
    {
        List<SaveableCharacterData> myCurTeam = new List<SaveableCharacterData>();
        var characterData = characterDataCollection;
        var teamsData = teams;
        var campaign = campaignData;

        var currentStageInt = int.Parse(campaign.currentStage.Replace("Stage_", ""));
        var customStageInt = int.Parse(campaign.customStage.Replace("Stage_", ""));

        var curStageName = customStageInt < currentStageInt ? campaign.customStage : campaign.currentStage;
        var currentStage = stageData.Where(x => x.stageId.Contains(curStageName)).FirstOrDefault();

        var curTeamName = teamsData.teamDataList.Where(x => x.TeamName.Contains(teamsData.selectedTeamName)).FirstOrDefault();
        var waveInstanceIdList = curTeamName.Waves[0].SaveableCharacters;

        for (int index = 0; index < waveInstanceIdList.Count; index++)
        {
            var instanceId = characterData.FindItem(instance => instance == waveInstanceIdList[index]);

            if (instanceId != null)
                myCurTeam.Add(instanceId);
        }

        var cubeAmount = BattleManager.Instance.GaianiteCollectionManager.cubeCountTestOnly;
        float sum = cubeAmount.Sum();

        float finalAttack = myCurTeam.Sum(x => x.SaveableStats.Attack);
        float stageBonus = currentStage.stageBonus;

        float finalComputation = (finalAttack * sum) * stageBonus;

        return finalComputation > currentStage.gaianiteCap ? currentStage.gaianiteCap : finalComputation;
    }

    public float ComputeSumToday(float sessionCollectedThisDay)
    {
        var sessionCollected = sessionCollectedThisDay;
        var totalCollectedThisDay = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite_Today");
        var maxGaianiteCap = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite_MaxTotal");

        float result = sessionCollected + totalCollectedThisDay.Amount;

        return result > maxGaianiteCap.Amount ? maxGaianiteCap.Amount : result;
    }

    public float ComputeSumClaimable(float sessionCollectedThisDay)
    {
        var sessionCollected = sessionCollectedThisDay;
        var totalClaimableGaianite = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite");
        var totalCollectedThisDay = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite_Today");
        var maxGaianiteCap = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite_MaxTotal");

        var gaianiteLeft = Math.Abs(totalCollectedThisDay.Amount - maxGaianiteCap.Amount);

        float result = (totalClaimableGaianite?.Amount ?? 0) + sessionCollected;
        float result2 = (totalClaimableGaianite?.Amount ?? 0) + gaianiteLeft;

        return sessionCollected > gaianiteLeft ? result2 : result;
    }


    public bool MetMaxCap()
    {
        var totalCollectedThisDay = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite_Today");
        var maxCap = PlayerDatabase.Inventories.SystemCurrencies.FindItem("Gaianite_MaxTotal");


        if (totalCollectedThisDay.Amount >= maxCap.Amount)
            return true;

        return false;
    }

}
