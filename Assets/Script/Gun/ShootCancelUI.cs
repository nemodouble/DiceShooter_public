using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class ShootCancelUI : MonoBehaviour
    {
        private void Update()
        {
            GetComponent<Image>().enabled =
                InputController.instance.nowSelectState == InputController.SelectState.AimingGun;
        }
    }
}