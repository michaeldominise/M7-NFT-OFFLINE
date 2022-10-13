using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using M7.Skill;
using System.Collections.Generic;
using M7.Match;
using System.Linq;
using M7.GameData;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using Newtonsoft.Json;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using UnityEngine.SceneManagement;
using M7.ServerTestScripts;

namespace M7.GameRuntime.Scripts.Managers.BattleScreen
{
    public class GaianiteCollectionManager : MonoBehaviour
    {
        [SerializeField] private UIDelayValue txtGaianiteCollection;
        [SerializeField] private TextMeshProUGUI txtGaiTotal;
        [SerializeField] private TextMeshProUGUI[] txtCurrentGai;
        [SerializeField] private UIDelayValue gaiMeter;

        public delegate void UpdateCollection(float value, float totalCollection);

        float InitialvalueGaianiteAddValue => MenuTeamSelector.curTeamAtk > 0 ? MenuTeamSelector.curTeamAtk : 50 * 1;
        float GaianiteAddValue => InitialvalueGaianiteAddValue * LevelManager.LevelData.StageBonus; 
        [SerializeField] float gaianiteValue;
        float TotalGaianiteCap => PlayerDatabase.Inventories.SystemCurrencies.GetAmount("Gaianite_MaxTotal");
        float TotalCollectedGaianite => PlayerDatabase.Inventories.SystemCurrencies.GetAmount("Gaianite_Today");
        float TotalGaianiteLeft => Mathf.Abs(TotalGaianiteCap - TotalCollectedGaianite);

        float LevelGaianiteCap => LevelManager.LevelData.GaianiteCap;

        [SerializeField] GameObject[] cubeTypesObj;
        public int[] cubeCountTestOnly;

        public void Init()
        {
            UpdateGaiaCollection(gaianiteValue, LevelGaianiteCap);
        }
        public void AddCollection()
        {
            if (gaianiteValue < TotalGaianiteLeft)
            {
                gaianiteValue = Mathf.Clamp(gaianiteValue + GaianiteAddValue, 0, TotalGaianiteCap < LevelGaianiteCap ? TotalGaianiteCap : LevelGaianiteCap);
                UpdateGaiaCollection(gaianiteValue, LevelGaianiteCap);
            }
            else if (gaianiteValue >= TotalGaianiteLeft)
            {
                gaianiteValue = TotalGaianiteLeft;
            }
        }

        private void UpdateGaiaCollection(float currentCollection, float fullCollection)
        {
            txtGaianiteCollection.SetValue(currentCollection);
            txtGaiTotal.SetText($"/{fullCollection:N}");
            gaiMeter.SetValue(currentCollection / fullCollection);

            for(int i = 0; i < txtCurrentGai.Length; i++)
            {
                txtCurrentGai[i].text = gaianiteValue.ToString("0.00");
            }
        }

        public void SetCollected()
        {
            DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.SetCollection);
        }

        public void SaveCubeCount()
        {
            for (int i = 0; i < cubeTypesObj.Length; i++)
            {
                cubeTypesObj[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cubeCountTestOnly[i].ToString();
            }
        }

        public void ActivateTile(MatchGridCell cell)
        {
            List<CharacterInstance_Battle> noDupes = new List<CharacterInstance_Battle>();

            for(int i = 0; i < BattleManager.Instance.PlayerTeam.AliveCharacters.Count; i++)
            {
                bool dupes = false;
                for(int z = 0; z < i; z++)
                {
                    if(BattleManager.Instance.PlayerTeam.AliveCharacters[z].Element == BattleManager.Instance.PlayerTeam.AliveCharacters[i].Element)
                    {
                        dupes = true;
                        break;
                    }
                }
                if (!dupes)
                {
                    noDupes.Add(BattleManager.Instance.PlayerTeam.AliveCharacters[i]);
                }
            }
            for (int i = 0; i < cubeTypesObj.Length; i++)
            {
                foreach (CharacterInstance_Battle charElem in noDupes)
                {
                    if (charElem.Element.ElementType == cell.CellTypeContainer.CellType.ElementType)
                        if (cubeTypesObj[i].GetComponent<CubeElementTestOnly>().elementTypeString == cell.CellTypeContainer.CellType.ElementType.ToString())
                        {
                            cubeCountTestOnly[i]++;
                            if (!cubeTypesObj[i].activeInHierarchy)
                            {
                                cubeTypesObj[i].SetActive(true);
                            }
                        }
                }
            }
        }

    }
}
