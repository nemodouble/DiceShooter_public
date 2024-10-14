using System;
using UnityEngine;

namespace Script.Ad
{
    public class AdButton : MonoBehaviour, IButton
    {
        private void Awake()
        {
            AdController.instance.ShowBannerAd();
        }

        private void OnDisable()
        {
            AdController.instance.HideBannerAd();
        }

        public bool Clicked()
        {
            if(GameController.instance.m_IsAddedBullet) return false;
            AdController.instance.ShowRewardAd();
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            return true;
        }
    }
}