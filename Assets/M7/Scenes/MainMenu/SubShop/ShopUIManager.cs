using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.AddressableAssets;
using TMPro;

namespace M7
{
    public class ShopUIManager : MonoBehaviour
    {
        [SerializeField] GameObject popUpPanel;
        [SerializeField] Image displayImage;
        [SerializeField] TextMeshProUGUI packageTitleText;
        [SerializeField] TextMeshProUGUI descriptionText;

        [SerializeField] Sprite characterPack;
        [SerializeField] Sprite ticketPack;
        [SerializeField] Sprite boosterPack1;
        [SerializeField] Sprite boosterPack2;
        [SerializeField] Sprite boosterPack3;
        [SerializeField] Sprite gaiPack1;
        [SerializeField] Sprite gaiPack2;
        [SerializeField] Sprite gaiPack3;
        public static ShopUIManager Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }
        
        /// <summary>
        /// THIS IS TEXT ONLY FOR BUTTONS
        /// </summary>
        /// <param name="value"></param>
        public void SuccessPopUp(string value)
        {
            switch(value)
            {
                case "character_nonnft":
                    packageTitleText.text = "Hero Recruit";
                    descriptionText.text = "Purchase Successful";
                    displayImage.sprite = characterPack;
                    break;
                case "Ticket":
                    packageTitleText.text = "Ticket";
                    descriptionText.text = "Purchase Successful";
                    displayImage.sprite = ticketPack;
                    break;
                case "boosterpack_1":
                    packageTitleText.text = "Booster Package 1";
                    descriptionText.text = "Purchase Successful";
                    displayImage.sprite = boosterPack1;
                    break;
                case "boosterpack_2":
                    packageTitleText.text = "Booster Package 2";
                    descriptionText.text = "Purchase Successful";
                    displayImage.sprite = boosterPack2;
                    break;
                case "boosterpack_3":
                    packageTitleText.text = "Booster Package 3";
                    descriptionText.text = "Purchase Successful";
                    displayImage.sprite = boosterPack3;
                    break;
                case "20_gaianite_test":
                    packageTitleText.text = "Gaianite Bundle 1";
                    descriptionText.text = "Purchase Successful";
                    displayImage.sprite = gaiPack1;
                    break;
                case "40_gaianite_test":
                    packageTitleText.text = "Gaianite Bundle 2";
                    descriptionText.text = "Purchase Successful";
                    displayImage.sprite = gaiPack2;
                    break;
                case "60_gaianite_test":
                    packageTitleText.text = "Gaianite Bundle 3";
                    descriptionText.text = "Purchase Successful";
                    displayImage.sprite = gaiPack3;
                    break;
            }    

            popUpPanel.SetActive(true);
        }
        public void ClosePopUp() => popUpPanel.SetActive(false);
    }
}
