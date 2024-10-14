using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class VolumeSlider : MonoBehaviour
    {
        private MusicButton m_MusicButton;
        
        private Slider m_Slider;
        private float m_OriginVolume;
        private bool m_IsMute;

        private void Start()
        {
            m_MusicButton = transform.parent.Find("MusicButton").GetComponent<MusicButton>();
            m_Slider = GetComponent<Slider>();
            if (!PlayerPrefs.HasKey("MasterVolume"))
            {
                PlayerPrefs.SetFloat("MasterVolume", 1f);
            }
            m_Slider.value = PlayerPrefs.GetFloat("MasterVolume");
            if (m_Slider.value == 0)
                m_IsMute = true;
            ChangeVolume();
        }

        public void ChangeVolume()
        {
            if (m_IsMute && m_Slider.value != 0)
            {
                m_IsMute = false;
                m_MusicButton.ChangeOnOffImage();
            }

            if (!m_IsMute && m_Slider.value == 0)
            {
                m_IsMute = true;
                m_OriginVolume = 1;
                m_MusicButton.ChangeOnOffImage();
            }
            BgmController.Instance.SetMasterVolume(m_Slider.value);
            PlayerPrefs.SetFloat("MasterVolume", m_Slider.value);
        }

        public void ChangeMute()
        {
            if (!m_IsMute)
            {
                m_IsMute = true;
                m_OriginVolume = m_Slider.value;
                m_Slider.value = 0;
                ChangeVolume();
            }
            else
            {
                m_IsMute = false;
                m_Slider.value = m_OriginVolume;
                ChangeVolume();
            }
        }
    }
}