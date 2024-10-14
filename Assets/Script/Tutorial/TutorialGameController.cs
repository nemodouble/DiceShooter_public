using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class TutorialGameController : GameController
    {
        public TutorialPlayController tutorialPlayController;

        protected override IEnumerator WinGame()
        {
            var isPerfect = GetSuccessRate() == 100;
            
            // Clear or Perfect text 표시
            if (isPerfect)
                ClearBoxText.instance.ShowPerfectText();
            else
                ClearBoxText.instance.ShowClearText();
            
            // Clear 정보 저장 
            const string nowSceneName = "1-1";
            if(PlayerPrefs.HasKey(nowSceneName))
                if(PlayerPrefs.GetInt(nowSceneName) != (int)StageController.StageState.Perfect)
                {
                    var stageState = isPerfect ? StageController.StageState.Perfect : StageController.StageState.Clear;
                    PlayerPrefs.SetInt(nowSceneName, (int) stageState);
                }
            if(PlayerPrefs.GetInt(nextSceneName) == (int)StageController.StageState.Lock)
                PlayerPrefs.SetInt(nextSceneName, (int)StageController.StageState.UnLock);
            
            RuntimeManager.PlayOneShot("event:/bgm/stage_clear");
            
            // 종료 대기, 후처리
            yield return new WaitForSeconds(3f);
            RolledNumberText.rollNumber = null;
            InputController.OnDeselect();
            
            
            isShooted = false;
            ClearBoxText.instance.ShowBlinkText();
            tutorialPlayController.Success();
        }

        protected override IEnumerator LoseGame()
        {
            ClearBoxText.instance.ShowFailText();
            RuntimeManager.PlayOneShot("event:/ui/ui_fail");
            yield return new WaitForSeconds(3f);
            RolledNumberText.rollNumber = null;
            InputController.OnDeselect();
            
            
            isShooted = false;
            ClearBoxText.instance.ShowBlinkText();
            tutorialPlayController.Fail();
        }
    }
}