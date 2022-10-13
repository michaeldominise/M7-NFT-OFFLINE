using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace M7.GameRuntime
{
    public class UIWaveManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI[] waveText;
        [SerializeField] TextMeshProUGUI stageText;
        [SerializeField] List<GameObject> currentWaveIndicator;
        [SerializeField] Sprite[] waveSequenceContainers;
        [SerializeField] CanvasGroup waveSequenceCanvas;
        [SerializeField] bool isNotFinalWave;
        [SerializeField] GameObject circleWaveIndicatorObj;
        [SerializeField] Transform waveParent;

        public delegate void WaveUpdate(int currentWave, int waveCount);

        public static event WaveUpdate OnWaveUpdate;

        [Button]
        public void UpdateUI(int currentWave, int totalWaveCount)
        {
            isNotFinalWave = currentWave + 1 < totalWaveCount;
            //OnWaveUpdate += UpdateWaveUI;
            //OnWaveUpdate?.Invoke(currentWave, totalWaveCount);
            UpdateWaveUI(currentWave, totalWaveCount);
        }

        private void UpdateWaveUI(int currentWave, int waveCount)
        {
            //stageText.gameObject.AddComponent<CanvasRenderer>();
            //stageText.gameObject.AddComponent<TextMeshProUGUI>();
            //waveText[0].gameObject.AddComponent<CanvasRenderer>();
            //waveText[0].gameObject.AddComponent<TextMeshProUGUI>();
            stageText.text = LevelManager.LevelData.DisplayName;
            waveText[0].text = $"{currentWave + 1} / {waveCount}";

            if (isNotFinalWave)
            {
                waveSequenceCanvas.GetComponent<Image>().sprite = waveSequenceContainers[0];
                waveText[1].SetText($"Wave {currentWave + 1} / {waveCount}");
            }
            else
            {
                waveSequenceCanvas.GetComponent<Image>().sprite = waveSequenceContainers[1];
                waveText[1].SetText($"Final Wave");
            }
            currentWaveIndicator[currentWave].transform.GetChild(0).gameObject.SetActive(true);
        }

        public IEnumerator InitSequence(float duration)
        {
            waveSequenceCanvas.DOFade(1, 1);
            yield return new WaitForSeconds(duration);
            waveSequenceCanvas.DOFade(0, 1);
        }

        public void Init(int currentWave, int waveCount)
        {
            for (int i = 0; i < waveCount; i++)
            {
                GameObject waveObject = Instantiate(circleWaveIndicatorObj, waveParent);
                waveObject.name = "CircleFill";
                waveObject.SetActive(true);
                currentWaveIndicator.Add(waveObject);

            }
        }
    }
}