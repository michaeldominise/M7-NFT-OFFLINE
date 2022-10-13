using M7.GameRuntime.Scripts.Managers.BattleScreen;
using M7.GameRuntime.Scripts.UI.OverDrive;
using TMPro;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Managers.Avatars
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtWaveCount;
        [SerializeField] private TextMeshProUGUI txtStage;
        [SerializeField] private UIDelayValue txtGaianiteCollection;
        [SerializeField] private TextMeshProUGUI txtTotal;
        [SerializeField] private UIDelayValue gaiMeter;

        private void OnEnable()
        {
            //UIWaveManager.OnWaveUpdate += UpdateUI;
            //UIGameBattleManager.OnStageUpdate += UpdateStage;
            //GaianiteCollectionManager.OnUpdateGaiaCollection += UpdateGaiaCollection;

        }

        //private void OnDisable()
        //{
        //    UIWaveManager.OnWaveUpdate -= UpdateUI;
        //    UIGameBattleManager.OnStageUpdate -= UpdateStage;
        //}
        
        //private void UpdateUI(int currentWave, int waveCount) => txtWaveCount.SetText($"{currentWave + 1} / {waveCount + 1}");

        //private void UpdateStage(string stagename) => txtStage.SetText(stagename);

        //private void UpdateGaiaCollection(float currentCollection, float fullCollection)
        //{
        //    //var collectionPercentage = currentCollection / fullCollection;
        //    txtGaianiteCollection.SetValue(currentCollection);
        //    txtTotal.SetText($"/{fullCollection:N}");
        //    gaiMeter.SetValue(currentCollection / fullCollection);
        //    //txtGaianiteCollection.SetText($"{currentCollection:N}/{fullCollection:N}");
        //}
    }
}
