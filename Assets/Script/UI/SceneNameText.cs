using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class SceneNameText : MonoBehaviour
    {
        private TextMeshPro m_TextMeshPro;
        
        private void Start()
        {
            m_TextMeshPro = GetComponent<TextMeshPro>();
            m_TextMeshPro.text = SceneManager.GetActiveScene().name;
        }
    }
}