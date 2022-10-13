using M7.GameData.CharacterSkill;
using M7.Match;
using M7.Skill;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace M7.GameRuntime.Scripts.UI.OverDrive
{
    public class CharacterSkillUI : MonoBehaviour
    {
        [SerializeField] private UIDelayValue overDriveBar;

        private CharacterSkillInputEvent _heroOverDriveInputEvent;
        private CharacterInstance_Battle characterInstanceBattle;
        public CharacterSkillData CharacterSkillData => characterInstanceBattle.CharacterSkillData;

        public CharacterSkillInputEvent _HeroOverDriveInputEvent => _heroOverDriveInputEvent;
        public void Init(CharacterInstance_Battle characterInstanceBattle)
        {
            this.characterInstanceBattle = characterInstanceBattle;
            if (characterInstanceBattle == null) return;

            if (characterInstanceBattle.IsPlayerObject)
            {
                _heroOverDriveInputEvent = characterInstanceBattle.GetComponent<CharacterSkillInputEvent>();
                _heroOverDriveInputEvent.OnClick += HeroOverdrive;
                _heroOverDriveInputEvent.IsClickable = false;
            }
            
            // overdrive
            CharacterSkillData.Init(characterInstanceBattle);
            CharacterSkillData.onSkillPointsUpdate += OnSkillPointsUpdate;
        }

        void OnSkillPointsUpdate(float value)
        {
            overDriveBar.SetValue(value);
            if (characterInstanceBattle.IsPlayerObject)
                _heroOverDriveInputEvent.IsClickable = value >= 1;
        }

        private void HeroOverdrive()
        {
            if(TurnManager.Instance.CurrentState == TurnManager.State.EnemyTurn) 
                return;

            if (PuzzleBoardManager.Instance.CurrentState == PuzzleBoardManager.State.WaitingForPlayerInput ||
                PuzzleBoardManager.Instance.CurrentState == PuzzleBoardManager.State.PlayerUseSkill)
            {
                CharacterSkillData.Execute();   
            }
        }

        private void OnDestroy()
        {
            CharacterSkillData.onSkillPointsUpdate -= OnSkillPointsUpdate;
        }
    }
}
