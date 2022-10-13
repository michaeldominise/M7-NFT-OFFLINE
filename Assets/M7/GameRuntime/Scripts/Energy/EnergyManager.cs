using System;
using M7.GameData;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Energy
{
    public class EnergyManager : MonoBehaviour
    {
        public static EnergyManager Instance;

        public delegate void UpdateEnergyData();

        public static event UpdateEnergyData OnUpdateEnergyData;

        public delegate void TimerOutput(string remainingTime);
        public static event TimerOutput OnTimerTick;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                UpdateEnergy();
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        //public void Play(Action<ExecuteResult> executeResult)
        //{
        //    if (PlayerDatabase.Inventories.Energy.currentEnergy <= 0)
        //    {
        //        UnableToPlay();
        //        executeResult?.Invoke(new ExecuteResult { Status = ResultStatus.Error });
        //        return;
        //    }
        //    PlayFabFunctions.PlayFabCallFunction("PlayGame", false, "", "", result =>
        //    {
        //        EnergyCallback(result);
        //        executeResult?.Invoke(result);

        //        if (result.Status != ResultStatus.Ok)
        //            UnableToPlay();
        //    });
        //}

        public void UnableToPlay() => MessageBox.Create("Not enough energy to play.", MessageBox.ButtonType.Ok).Show();

        private void UpdateEnergy()
        {
            //PlayFabFunctions.PlayFabCallFunction("UpdateEnergy", false, "", "", EnergyCallback);
        }

        public void EnergyCallback(ExecuteResult result)
        {
            if (result.Status != ResultStatus.Ok)
                return;

            PlayerDatabase.Inventories.Energy.OverwriteValues(result.Result.FunctionResult.ToString());
            
            if (PlayerDatabase.Inventories.Energy != null && PlayerDatabase.Inventories.Energy.currentEnergy == PlayerDatabase.Inventories.Energy.energyCap)
                PlayerDatabase.Inventories.Energy.isClockTicking = false;
            else
                PlayerDatabase.Inventories.Energy.isClockTicking = true;
            
            OnUpdateEnergyData?.Invoke();
        }

        private void LateUpdate()
        {
            if(!PlayerDatabase.Inventories.Energy.isClockTicking) return;

            if (PlayerDatabase.Inventories.Energy.timeEnergyUsed != PlayerDatabase.Inventories.Energy.timeToNextEnergy)
                OnTimerTick?.Invoke(PlayerDatabase.Inventories.Energy.TimeToNextEnergy());

            if (DateTime.UtcNow < PlayerDatabase.Inventories.Energy.timeToNextEnergy) return;
            
            PlayerDatabase.Inventories.Energy.isClockTicking = false;
            UpdateEnergy();
        }
    }

    // [Serializable]
    // public class EnergyData
    // {
    //     public  int currentEnergy;
    //     public int energyCap;
    //     [ShowInInspector, DisplayAsString(false)]
    //     public  DateTime timeEnergyUsed;
    //     [ShowInInspector, DisplayAsString(false)]
    //     public  DateTime timeToNextEnergy;
    //
    //     [ShowInInspector, DisplayAsString(false)]
    //     [JsonIgnore] public string time;
    //     [JsonIgnore] [ReadOnly] public bool isClockTicking;
    //     // public delegate void 
    //     
    //     [Button]
    //     public void Play()
    //     {
    //         PlayFabFunctions.PlayFabCallFunction("PlayGame", "", "", EnergyCounter.Instance.UpdateEnergy);
    //     }
    //
    //     [Button]
    //     public void Init()
    //     {
    //         PlayFabFunctions.PlayFabCallFunction("UpdateEnergy", "", "", EnergyCounter.Instance.UpdateEnergy);
    //     }
    //     
    //     [Button]
    //     private void EnergyToJson()
    //     {
    //         Debug.Log(JsonConvert.SerializeObject(this));
    //     }
    //     
    //     public string TimeToNextEnergy()
    //     {
    //         var timeSpan = timeToNextEnergy - DateTime.UtcNow;
    //         return string.Format($" {timeSpan.Hours:00}h:{timeSpan.Minutes:00}min");
    //     }
    // }
}
