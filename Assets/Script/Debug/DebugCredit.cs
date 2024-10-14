using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class DebugCredit : MonoBehaviour
    {
        private void Update()
        {
            if (PlayerPrefs.GetInt("DebugOn", 0) == 1)
            {
                GetComponent<Image>().color = Color.white;
            }
            else
            {
                GetComponent<Image>().color = Color.clear;
            }
        }
    }
}