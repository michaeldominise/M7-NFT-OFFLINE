//using M7.GameData.Scripts.RPGObjects.Boosters;
//using M7.GameRuntime;
//using M7.Match;
//using M7.Skill;
//using UnityEngine;

//namespace M7.PuzzleBoard.Scripts.Booster
//{
//    public abstract class BoosterBase: MonoBehaviour
//    {
//        [SerializeField] private BoosterObject boosterObject;
//        [SerializeField] private SkillObject skillObject;
//        public BoosterObject BoosterObject => boosterObject;
        
//        //public virtual bool BoosterToggle()
//        //{
//        //    print("Booster Base");
            
//        //    if(TurnManager.Instance.CurrentState == TurnManager.State.EnemyTurn) return false;

//        //    if (PuzzleBoardManager.Instance.CurrentState != PuzzleBoardManager.State.WaitingForPlayerInput 
//        //        && PuzzleBoardManager.Instance.CurrentState != PuzzleBoardManager.State.PlayerUseSkill) return false;
            
//        //    // set to active   
//        //    if (!BoosterManager.Instance.isBoosterActive)
//        //    {
//        //        BoosterManager.Instance.isBoosterActive = true;
//        //        BoosterManager.Instance.activeBoosterSkillObject = skillObject;
//        //        return true;

//        //    }
            
//        //    BoosterManager.Instance.isBoosterActive = false;
//        //    BoosterManager.Instance.activeBoosterSkillObject = null;
//        //    return false;
//        //}
//    }
//}