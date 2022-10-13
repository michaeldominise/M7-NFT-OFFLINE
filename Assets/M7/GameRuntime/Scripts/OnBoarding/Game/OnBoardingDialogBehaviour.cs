using System;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime.Scripts.OnBoarding.Game
{
    public class OnBoardingDialogBehaviour : MonoBehaviour
    {
        [SerializeField] private OnBoardingModel onBoardingModel;

        private void OnEnable()
        {
            onBoardingModel.NextButton.onClick.AddListener(NextButton);
        }

        private void OnDisable()
        {
            onBoardingModel.NextButton.onClick.RemoveListener(NextButton);
        }
        
        private void NextButton()
        {
            PuzzleBoardOnBoardingManager.Instance.Exit();
        }

        #region Button

        public OnBoardingDialogBehaviour ShowButton()
        {
            onBoardingModel.NextButton.gameObject.SetActive(true);
            return this;
        }

        public OnBoardingDialogBehaviour HideButton()
        {
            onBoardingModel.NextButton.gameObject.SetActive(false);
            return this;
        }
        
        #endregion
        
        #region DIALOG

        public OnBoardingDialogBehaviour ShowDialog(string message)
        {
            gameObject.SetActive(true);
            
            onBoardingModel.Description.gameObject.SetActive(true);
            onBoardingModel.Description.text = message;

            return this;
        }

        public OnBoardingDialogBehaviour ShowOkDialog(string message)
        {
            ShowDialog(message);
            onBoardingModel.Description.gameObject.SetActive(true);
            onBoardingModel.NextButton.gameObject.SetActive(true);

            return this;
        }
        
        public OnBoardingDialogBehaviour HideDialog()
        {
            onBoardingModel.Description.gameObject.SetActive(false);
            gameObject.SetActive(false);

            return this;
        }
        
        public OnBoardingDialogBehaviour SetBoxPosition(Vector2 position)
        {
            onBoardingModel.Box.anchoredPosition = position;
            return this;
        }

        public OnBoardingDialogBehaviour SetBoxSize(Vector2 size)
        {
            onBoardingModel.Box.sizeDelta = size;
            return this;
        }

        #endregion

        #region TAIL

        public OnBoardingDialogBehaviour SetTailPosition(Vector2 position)
        {
            onBoardingModel.Tail.anchoredPosition = position;
            return this;
        }

        public OnBoardingDialogBehaviour SetTailSize(Vector2 size)
        {
            onBoardingModel.Tail.sizeDelta = size;
            return this;
        }
        
        public OnBoardingDialogBehaviour SetTailRotation(Vector3 rotation)
        {
            onBoardingModel.Tail.rotation = Quaternion.Euler(rotation);
            return this;
        }

        public OnBoardingDialogBehaviour ShowTail()
        {
            onBoardingModel.Tail.gameObject.SetActive(true);
            return this;
        }
        
        public OnBoardingDialogBehaviour HideTail()
        {
            onBoardingModel.Tail.gameObject.SetActive(false);
            return this;
        }        

        #endregion

        #region DisplayImage

        public OnBoardingDialogBehaviour ShowDisplayImage()
        {
            onBoardingModel.DisplayImage.gameObject.SetActive(true);
            return this;
        }
        
        public OnBoardingDialogBehaviour HideDisplayImage()
        {
            onBoardingModel.DisplayImage.gameObject.SetActive(false);
            return this;
        }
        
        public OnBoardingDialogBehaviour SetImage(Sprite pSprite)
        {
            onBoardingModel.DisplayImage.sprite = pSprite;
            return this;
        }

        public void Reset()
        {
            HideDisplayImage();
            HideButton();
            HideTail();
            HideDialog();
        }

        #endregion
    }
}
