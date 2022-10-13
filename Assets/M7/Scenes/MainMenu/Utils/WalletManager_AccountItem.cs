using M7.GameData;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using M7.GameRuntime;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Collections;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using UnityEngine.Events;

//WalletConnect
using WalletConnectSharp.Core.Models;
using WalletConnectSharp.Unity;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;

namespace M7
{
    public class WalletManager_AccountItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] GameObject separator;
        Action onClickEvent;

        public void Init(string description, Action onClickEvent, bool showSeparator)
        {
            gameObject.SetActive(true);
            this.description.text = description;
            this.onClickEvent = onClickEvent;
            separator.SetActive(showSeparator);
        }

        public void OnClick() => onClickEvent?.Invoke();
    }
}
