using System;
using TMPro;
using UnityEngine;

namespace Script
{
    public class ClearPercentage : MonoBehaviour
    {
        public static ClearPercentage Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<ClearPercentage>();

                if (instance != null)
                    return instance;

                throw new UnityException();
            }
        }
        public static ClearPercentage instance;

        private TextMeshPro m_TextMeshPro;
        
        private void Start()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            m_TextMeshPro = GetComponent<TextMeshPro>();
            UpdatePercentage();
        }
        
        public void UpdatePercentage()
        {
            m_TextMeshPro.text = GameController.instance.GetSuccessRate().ToString().PadLeft(3, '0') + "%";
        }
    }
}