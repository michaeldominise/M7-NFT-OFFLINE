using System;
using DG.Tweening;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityEvents;
using WalletConnectSharp.Unity.Utils;

public class PuzzleBoardOnBoardingUI : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private Camera onBoardingCamera;
    [SerializeField] private OnBoardingDialogBehaviour onBoardingDialogBehaviour;
    
    // UI
    [SerializeField] private GameObject moveCounter;
    [SerializeField] private GameObject gaianiteEarned, resultPanel;

    public Image Panel => panel;
    public Camera PuzzleCamera => puzzleCamera;
    public Camera OnBoardingCamera => onBoardingCamera;
    
    public OnBoardingDialogBehaviour OnBoardingDialogBehaviour => onBoardingDialogBehaviour;
    
    public void ShowPanel(Action onComplete, bool enablePanel = true)
    {
        gameObject.SetActive(true);
        panel.gameObject.SetActive(enablePanel);
        panel.color = new Color(0, 0, 0, 0);
        panel.DOFade(150 / 256f, 0.25f).OnComplete(() => onComplete?.Invoke());
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
        panel.color = new Color(0, 0, 0, 0);
    }

    public void ActivatePuzzleCamera()
    {
        puzzleCamera.depth = 9;
    }
    public void DeactivatePuzzleCamera()
    {
        puzzleCamera.depth = 1;
    }
    
    public void ActivateBoardingCamera()
    {
        
    }
    public void DeactivateBoardingCamera()
    {
        
    }

    private GameObject _moveCounter;
    
    [Button]
    public void InstantiateMoveCounter()
    {
        _moveCounter = Instantiate(moveCounter, transform);
        _moveCounter.layer = LayerMask.NameToLayer("OnBoarding");
    }

    public void DestroyMoveCounter()
    {
        Destroy(_moveCounter);
    }

    private GameObject _gaianite, _cubesDestroyed, _resultPanel;
    
    public void InstantiatePanel()
    {
        _resultPanel = Instantiate(resultPanel, transform);
        _gaianite = Instantiate(gaianiteEarned, transform);
        
        _resultPanel.transform.SetSiblingIndex(1);
        _gaianite.transform.SetSiblingIndex(2);
        _gaianite.GetComponent<Gai_ImageBehaviour>().Set();
    }

    public void DestroyGaianiteAndCube()
    {
        Destroy(_gaianite);
        Destroy(_cubesDestroyed);
    }
}
