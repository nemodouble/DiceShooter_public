using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class RestartButton : MonoBehaviour, IButton
    {
        public bool Clicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return true;
        }
    }
}