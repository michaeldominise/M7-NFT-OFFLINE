using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using M7.Extensions.Text;

namespace M7
{
    public class MessageBox : PopupBox
    {
        [Flags]
        public enum ButtonType
        {
            None,
            Ok = 1 << 0,
            No = 1 << 1,
            Cancel = 1 << 2,
            Loading = 1 << 3,
            Ok_No = Ok | No,
            Ok_Cancel = Ok | Cancel,
            Ok_No_Cancel = Ok | No | Cancel,
            CancelableLoading = Loading | Cancel,
        }

        static MessageBox NewInstance
        {
            get
            {
                var instance = SpawnPool.Spawn(Resources.Load<MessageBox>("MessageBox").transform).GetComponent<MessageBox>();
                instance.transform.localPosition = Vector3.zero;
                instance.gameObject.SetActive(false);
                instance.Clear();
                return instance;
            }
        }

        static List<MessageBox> activeMessageBoxList = new List<MessageBox>();
        public static bool HasActiveMessage => activeMessageBoxList.Count > 0;

        [Header("Labels")]
        [SerializeField] TextMeshProUGUI titleLabel;
        [SerializeField] TextMeshProUGUI bodyLabel;
        [SerializeField] TextMeshProUGUI okBtnLabel;
        [SerializeField] TextMeshProUGUI noBtnLabel;

        [Header("Button Refs")]
        [SerializeField] Button okButton;
        [SerializeField] Button noButton;
        [SerializeField] Button cancelButton;
        [SerializeField] GameObject loading;

        Action<ButtonType> onResult;
        bool canClose;

        public static MessageBox Create(string body, ButtonType buttonType, string title = "Notice", string okLabel = "Ok", string noLabel = "No",  Action<ButtonType> onResult = null, bool canClose = false)
            => NewInstance.InitValues(body, buttonType, title, okLabel, noLabel, onResult, canClose);
        public static void HideCurrent()
        {
            if (activeMessageBoxList.Count == 0 || activeMessageBoxList[activeMessageBoxList.Count - 1] == null)
                return;
            activeMessageBoxList[activeMessageBoxList.Count - 1].Hide();
        }

        public MessageBox InitValues(string body, ButtonType buttonType, string title = "Notice", string okLabel = "Ok", string noLabel = "No", Action<ButtonType> onResult = null, bool canClose = false)
        {
            titleLabel.text = title;
            bodyLabel.text = body;

            this.okBtnLabel.text = okLabel.ToUppercaseFirst();
            this.noBtnLabel.text = noLabel.ToUppercaseFirst();

            if ((buttonType & ButtonType.Loading) == ButtonType.Loading)
            {
                loading.SetActive(true);
                okButton.gameObject.SetActive(false);
                noButton.gameObject.SetActive(false);
            }
            else
            {
                loading.SetActive(false);
                okButton.gameObject.SetActive((buttonType & ButtonType.Ok) == ButtonType.Ok);
                noButton.gameObject.SetActive((buttonType & ButtonType.No) == ButtonType.No);
            }
            cancelButton.gameObject.SetActive((buttonType & ButtonType.Cancel) == ButtonType.Cancel);

            this.onResult = onResult;
            this.canClose = cancelButton.gameObject.activeSelf || canClose;

            return this;
        }

        public void OnButtonClick(int response) => OnButtonClick((ButtonType)response);
        public void OnButtonClick(ButtonType response)
        {
            if (response == ButtonType.Cancel && !canClose)
                return;

            onResult?.Invoke(response);
            Hide();
        }

        public override void Hide()
        {
            activeMessageBoxList.Remove(this);
            base.Hide();
        }

        public MessageBox Show()
        {
            activeMessageBoxList.Add(this);
            return Show<MessageBox>();
        }
    }
}
