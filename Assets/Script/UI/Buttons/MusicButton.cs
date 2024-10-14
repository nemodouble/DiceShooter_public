using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class MusicButton : MonoBehaviour
    {
        private Image m_Image;
        private bool m_IsOn;

        public Sprite onImage;
        public Sprite offImage;

        private void Start()
        {
            m_Image = gameObject.GetComponent<Image>();
            m_IsOn = true;
        }

        public void ChangeOnOffImage()
        {
            m_Image.sprite = m_IsOn ? offImage : onImage;
            m_IsOn = !m_IsOn;
        }
    }
}