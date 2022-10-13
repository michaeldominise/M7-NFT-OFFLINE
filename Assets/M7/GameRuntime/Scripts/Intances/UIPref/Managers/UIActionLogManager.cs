using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M7.GameRuntime
{
    public class UIActionLogManager : MonoBehaviour
    {
        public GameObject actionLogPrefab;
        public Transform actionContent;

        public void SetInstance(string action, string time)
        {
            GameObject tempInst = Instantiate(actionLogPrefab, actionContent);
            tempInst.transform.GetChild(0).GetComponent<Text>().text = action;
            tempInst.transform.GetChild(1).GetComponent<Text>().text = time;
        }
    }
}