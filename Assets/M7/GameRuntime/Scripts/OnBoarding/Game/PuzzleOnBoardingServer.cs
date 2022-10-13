using System;
using System.Collections.Generic;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using Newtonsoft.Json;
using PlayFab;
using Sirenix.OdinInspector;
using Spine;
using UnityEngine;

namespace M7.GameRuntime.Scripts.OnBoarding.Game
{
    [Serializable]
    public class PuzzleOnBoardingServer
    {
        private PuzzleBoardOnBoardingManager _onBoardingManager;

        public PuzzleOnBoardingServer(PuzzleBoardOnBoardingManager onBoardingManager)
        {
            _onBoardingManager = onBoardingManager;
        }
        
        [Button]
        public void GetPuzzleBoardJsonToServer()
        {
            // PlayFabFunctions.PlayFabCallFunction("GetPuzzleBoardJsonToServer", callBack: CallBack, errorCallBack:ErrorCallBack);
            
            // stages = JsonConvert.DeserializeObject<PuzzleBoardStages>(json);

            var json = PuzzleBoardOnBoardingDataStorage.Load(); 
            
            Debug.Log("<color=green>GetPuzzleBoardJsonToServer</color>");

            var stages = JsonConvert.DeserializeObject<PuzzleBoardStages>(json);
            
            foreach (var stage in stages)
            {
                if (_onBoardingManager.Stages.ContainsKey(stage.Key))
                    _onBoardingManager.Stages[stage.Key].IsDone = stage.Value.IsDone;
            }
        }

        private void ErrorCallBack(PlayFabError obj)
        {
            Debug.Log("GetPuzzleBoardJsonToServer");
        }

        private void CallBack(ExecuteResult obj)
        {
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(obj.Result.FunctionResult.ToString());
            Debug.Log($"GetPuzzleBoardJsonToServer, {result["value"]}");
            // Debug.Log($"GetPuzzleBoardJsonToServer, {obj.Result.FunctionResult}");
            var stages = JsonConvert.DeserializeObject<PuzzleBoardStages>(result["value"]);

            foreach (var stage in stages)
            {
                if (_onBoardingManager.Stages.ContainsKey(stage.Key))
                    _onBoardingManager.Stages[stage.Key].IsDone = stage.Value.IsDone;
            }
        }

        public string ObjectToJson()
        {
            return JsonConvert.SerializeObject(_onBoardingManager.Stages);
        }
    }
}