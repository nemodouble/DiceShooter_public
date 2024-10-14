using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Script.StageController;

namespace Script
{
    public class DebugController : MonoBehaviour
    {
        public static bool IsDebugOn;
        public int opentry;

        public void ResetSave()
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene("MainMenu");
        }

        public void ResetTutorial()
        {
            PlayerPrefs.SetInt("ClearTutorial", 0);
            SceneManager.LoadScene("Tutorial");
        }

        public void OpenTry()
        {
            if (++opentry >= 4)
            {
                PlayerPrefs.SetInt("DebugOn", 1);
            }
        }

        public void ConsoleOn()
        {
            GameObject.Find("Console").GetComponent<Consolation.Console>().OpenConsole();
        }
        
        public void UnLockAll()
        {
            var sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                var name = SceneManager.GetSceneByBuildIndex(i).name;
                if(PlayerPrefs.GetInt(name, (int)StageState.Lock) == (int)StageState.Lock)
                    PlayerPrefs.SetInt(name, (int)StageState.UnLock);
            }
        }

        public void DebugOff()
        {
            PlayerPrefs.SetInt("DebugOn", 0);
            GameController.instance.PauseOrResumeGame();
        }
    }
}