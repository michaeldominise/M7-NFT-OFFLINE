using System;
using M7.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

namespace M7.GameRuntime
{
    public class TeamManager_Editable : TeamManager_Default
    {
        public enum State { Active, Empty, Selected }

        [SerializeField] Sprite[] platformSprites;
        [SerializeField] List<SpriteRenderer> platformList;
        [SerializeField] bool canSelect;
        public Action<TeamManager_Editable, int> OnPlatformClicked { get; set; }
        public Camera SelectorCamera { get; set; }

        [ShowInInspector, ReadOnly] public int SelectedIndex { get; set; } = -1;

        public override void Init(WaveData waveData, Action onFinish)
        {
            base.Init(waveData, onFinish);
            var saveableCharacters = waveData.SaveableCharacters;
            for (int i = 0; i < saveableCharacters.Count; i++)
                SetState(i, saveableCharacters[i] == null ? State.Active : State.Empty);

        }
        MenuTeamSelector_EditTeam e;

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) 
                return;
            if (EventSystem.current.alreadySelecting)
                return;

            RaycastHit hit;
            if (SelectorCamera == null || !Physics.Raycast(SelectorCamera.ScreenPointToRay(Input.mousePosition), out hit)) return;

            for (var i = 0; i < platformList.Count; i++)
            {
                var platform = platformList[i].gameObject;
                if (hit.collider.gameObject != platform) continue;
                
                PlatformSelected(i);
                return;
            }
        }

        protected virtual void PlatformSelected(int index)
        {
            if (canSelect)
                SetActiveState(index);
            OnPlatformClicked?.Invoke(this, index);
        }

        public void SetActiveState(int platformIndex)
        {
            if(SelectedIndex == platformIndex)
            {
                ResetState();
                return;
            }    

            SelectedIndex = platformIndex;
            for (var x = 0; x < platformList.Count; x++)
            {
                if(x == platformIndex)
                    SetState(x, State.Selected);
                else
                    SetState(x, RawCharacters[x] == null ? State.Active : State.Empty);
            }
        }

        public void SetState(int platformIndex, State stateType) {
            platformList[platformIndex].sprite = platformSprites[(int)stateType];
            Invoke("UpdateCheckCharacters", 0.2f);
        }

        public void ResetState()
        {
            SelectedIndex = -1;
            for (var x = 0; x < platformList.Count; x++)
                SetState(x, RawCharacters[x] == null ? State.Active : State.Empty);
            UpdateCheckCharacters ();
        }

        void UpdateCheckCharacters ()
        {
            for(int i = 0; i < ActiveCharacters?.Count; i++)
            {
                ActiveCharacters[i].SaveableData.IsDirty = true;
            }
            e = FindObjectOfType<MenuTeamSelector_EditTeam>();
            e?.InitDisplayValues();
        }
    }
}
