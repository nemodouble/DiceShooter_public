using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class TutorialController : MonoBehaviour
    {
        public int tutorialCount;
        private void Start()
        {
            InputController.instance.IsPaused = true;
            ActiveChildByIndex(tutorialCount);
        }

        public void NextTutorial(int change)
        {
            if (tutorialCount + change < 0 || tutorialCount + change >= transform.childCount) return;
            tutorialCount += change;
            ActiveChildByIndex(tutorialCount);
        }

        private void ActiveChildByIndex(int index)
        {
            // InputController.instance.IsPaused =
            //     transform.GetChild(index).GetComponent<TutorialPlayController>() == null;
            var numOfChild = transform.childCount;
            for (int i = 0; i < numOfChild; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == index);
            }
        }

        public void ClearTutorial()
        {
            PlayerPrefs.SetInt("ClearTutorial", 1);
            SceneManager.LoadScene("1-2");
        }
    }
}