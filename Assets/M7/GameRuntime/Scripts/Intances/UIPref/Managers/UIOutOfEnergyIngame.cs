using M7.GameData;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.Energy;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIOutOfEnergyIngame : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI energyText;
    [SerializeField] TextMeshProUGUI eneryTimer;
    private void Start()
    {
        DisplayEnergy();
    }
    public void OnEnable()
    {
        EnergyManager.OnTimerTick += EnergyTimerUpdater;
        EnergyManager.OnUpdateEnergyData += DisplayEnergy;
    }

    public void OnDisable()
    {
        EnergyManager.OnTimerTick -= EnergyTimerUpdater;
        EnergyManager.OnUpdateEnergyData -= DisplayEnergy;
    }


    private void DisplayEnergy()
    {
        eneryTimer.text = "Full";
        //energyText.text = $"{PlayerDatabase.Inventories.Energy.currentEnergy}/{PlayerDatabase.Inventories.Energy.energyCap}";
        energyText.SetText(PlayerDatabase.Inventories.Energy.currentEnergy.ToString());
    }

    private void EnergyTimerUpdater(string time)
    {
        eneryTimer.text = $"<size=50%>Next Energy in</size> \n" +
                            $"{time}";
    }

}
