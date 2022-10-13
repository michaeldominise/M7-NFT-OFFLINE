using M7.GameRuntime.Scripts.PlayfabCloudscript;
using PlayFab;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace M7.GameRuntime.Scripts.OnBoarding.Game.PuzzleBoardSegments
{
    public class PuzzleOnBoardingBehaviour_ShowGaianite : OnBoardingSegmentBase, IEndSegment
    {
        #if UNITY_EDITOR
        [FormerlySerializedAs("_puzzleBoardOnBoardingManager")] [SerializeField] private PuzzleBoardOnBoardingManager puzzleBoardOnBoardingManager;
        #endif
        
        public override void Execute()
        {
            base.Execute();
         
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetBoxPosition(
                new Vector2(2, -629));
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetBoxSize(
                new Vector2(700, 400));
            
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.ShowTail();
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetTailPosition(
                new Vector2(0, 224.6f));
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetTailRotation(
                new Vector3(0, 0, -180));
            
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.ShowPanel(ShowDialog, false);
        }

        public override void Exit()
        {
            base.Exit();

            PuzzleBoardOnBoardingManager.Instance.Stages["Stage 1"].IsDone = true;
            SaveToJson();
            
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.DestroyGaianiteAndCube();
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.HidePanel();
        }

        private void ShowDialog()
        {
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.gameObject.SetActive(true);
            PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour
                .ShowDialog("Gaianite earned is based on your team attack and number of blasted cubes on the stage.");
            // PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.InstantiatePanel();
        }

        [Button]
        private void SaveToJson()
        {
            string puzzleOnBoardingJson;
            
#if UNITY_EDITOR
                puzzleOnBoardingJson = puzzleBoardOnBoardingManager.PuzzleOnBoardingServer.ObjectToJson();
#elif !UNITY_EDITOR
                puzzleOnBoardingJson = PuzzleBoardOnBoardingManager.Instance.PuzzleOnBoardingServer.ObjectToJson();
#endif

            PuzzleBoardOnBoardingDataStorage.Save(puzzleOnBoardingJson);
            
            // var param = new Dictionary<string, object>
            // {
            //     { "json", puzzleOnBoardingJson }
            // };
            
            // PlayFabFunctions.PlayFabCallFunction("UpdatePuzzleBoardOnBoarding", false, param, CallBack, ErrorCallBack);
        }

        private void CallBack(ExecuteResult obj)
        {
            Debug.Log($"Save OK {obj.Result.FunctionResult}");
        }

        private void ErrorCallBack(PlayFabError obj)
        {
            Debug.Log($"Error {obj.ErrorMessage}");
        }
    }
}