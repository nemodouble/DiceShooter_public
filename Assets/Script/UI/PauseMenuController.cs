using System;
using UnityEngine;

namespace Script
{
    public class PauseMenuController : MonoBehaviour
    {
        private void Awake()
        {
            if(!PlayerPrefs.HasKey("DebugOn"))
                PlayerPrefs.SetInt("DebugOn", 0);
        }

        private void Update()
        {
            if (PlayerPrefs.GetInt("DebugOn") != 0)
            {
                transform.Find("Debug").gameObject.SetActive(true);
            }
            else
            {
                transform.Find("Debug").gameObject.SetActive(false);
            }
        }
    }
}