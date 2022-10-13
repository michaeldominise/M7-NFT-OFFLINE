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
using Newtonsoft.Json;
using UnityEngine.Events;

namespace M7
{
    [Serializable]
    public class VerificationCode
    {
        [SerializeField] TextMeshProUGUI verificationCodeLabel;
        [SerializeField] float requestCodeInterval = 60;

        public bool IsReady => timeLastCodeSent < Time.time;
        float timeLastCodeSent = float.MinValue;
        string originalLabel = "";
        
        public enum VerificationType
        {
            Login, Deletion
        }

        public void OnClick(string email, VerificationType type)
        {
            if (!StartCountdown())
                return;

            // verification logic
            var theEmail = new Dictionary<string, object> { { "email", email } };

            switch (type)
            {
                case VerificationType.Login:
                    AzureFunction.EmailVerification(theEmail, OkResult, ErrorResult);
                    break;
                case VerificationType.Deletion:
                    // AzureFunction.EmailVerificationDeleteAccount(theEmail, OkResult, ErrorResult);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void ErrorResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Email verification", "OK").Show();
        }

        private void OkResult(string obj)
        {
            //MessageBox.Create("Verification code sent to email", MessageBox.ButtonType.Ok, "Email verification", "OK").Show();
        }

        bool StartCountdown()
        {
            if (!IsReady)
                return false;

            timeLastCodeSent = Time.time + requestCodeInterval;
            originalLabel = verificationCodeLabel.text;
            GameManager.Instance.StartCoroutine(StartIntervalCountdown());

            return true;
        }

        private IEnumerator StartIntervalCountdown()
        {
            for (var x = 0; x < requestCodeInterval; x++)
            {
                verificationCodeLabel.text = $"{requestCodeInterval - x}s";
                yield return new WaitForSeconds(1);
            }

            verificationCodeLabel.text = originalLabel;
        }
    }
}
