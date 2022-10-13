using HutongGames.PlayMaker;

namespace M7.Match.PlaymakerActions {

    [ActionCategory("M7/Match")]
    public class StartHintCountdown : FsmStateAction {

        [RequiredField] public HintManager hintManager;
        public float countdownDuration = 3;

        public override void OnEnter() {
            hintManager.StartCountdown(countdownDuration);
        }

        public override void OnExit() {
            hintManager.StopCountdown();
        }
    }

}