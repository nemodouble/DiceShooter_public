using System;
using System.Collections;
using System.Linq;
using FMODUnity;
using Script.Ad;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using static Script.StageController;

namespace Script
{
    public class GameController : MonoBehaviour
    { 
        public static GameController Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<GameController>();

                if (instance != null)
                    return instance;

                Create ();

                return instance;
            }
        }
        public static GameController instance;

        public static GameController Create ()
        {
            GameObject sceneControllerGameObject = new GameObject("GameController");
            instance = sceneControllerGameObject.AddComponent<GameController>();

            return instance;
        }

        private bool isPaused = false;
        
        public Dice[] diceArray;
        public Gun[] gunArray;
        public Wall[] wallArray;

        public string nextSceneName;
        
        public bool isShooted;

        public int[] answerNumbers;
        public GameObject pauseCanvas;
        public int rolledDiceNumber;

        private bool m_ChangeLocalActive;
        public bool m_IsAddedBullet;
        
        private static readonly int Success = Animator.StringToHash("Success");

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            m_IsAddedBullet = false;
        }

        protected virtual void Start()
        {
            var diceList = GameObject.FindGameObjectsWithTag("Dice");
            diceArray = new Dice[diceList.Length];
            for (int i = 0; i < diceArray.Length; i++)
            {
                diceArray[i] = diceList[i].GetComponent<Dice>();
            }
            var gunList = GameObject.FindGameObjectsWithTag("Gun");
            gunArray = new Gun[gunList.Length];
            for (int i = 0; i < gunArray.Length; i++)
            {
                gunArray[i] = gunList[i].GetComponent<Gun>();
            }
            var wallList = GameObject.FindGameObjectsWithTag("Wall");
            wallArray = new Wall[wallList.Length];
            for (int i = 0; i < wallArray.Length; i++)
            {
                wallArray[i] = wallList[i].GetComponent<Wall>();
            }

            InputController.instance.IsPaused = false;
            PlayerPrefs.GetInt("LocalKey", Application.systemLanguage == SystemLanguage.Korean ? 1 : 0);
            if (answerNumbers.Length != diceArray.Length)
                throw new UnityException($"정답 갯수와 주사위 갯수 일치 안함 \n 정답수 {answerNumbers.Length} != 주사위 수 {diceArray.Length}");
        }

        public bool CanRollDice()
        {
            foreach (var t in diceArray)
            {
                if(t.fittedDiceHole == null)
                {
                    return false;
                }
            }
            return true;
        }
        
        public IEnumerator RollDice()
        {
            if(!CanRollDice())
            {
                RuntimeManager.PlayOneShot("event:/ui/ui_beep");
                ClearButton.instance.ButtonUp();
                yield break;
            }

            // can roll
            RuntimeManager.PlayOneShot("event:/ui/dice_roll");
            ClearBoxText.instance.ShowRollingText();
            
            var rollNumberArray = new int[diceArray.Length];
            var getCoroutineArray = new Coroutine[diceArray.Length];
            rolledDiceNumber = 0;
            
            // 물리적 굴리기 시작
            for (int i = 0; i < diceArray.Length; i++)
            {
                if(diceArray[i].fittedDiceHole != null)
                {
                    getCoroutineArray[i] = StartCoroutine(diceArray[i].RollDice());
                }
            }

            var infCheck = 0;
            // 물리적 굴리기 종료 체크
            while (rolledDiceNumber < diceArray.Length)
            {
                if (infCheck++ >= 1000000)
                    break;
                yield return null;
            }
            
            // 주사위 값 가져오기
            for (int i = 0; i < diceArray.Length; i++)
            {
                if(diceArray[i].fittedDiceHole != null)
                {
                    rollNumberArray[i] = diceArray[i].rolledNumber;
                }
            }
            
            ClearButton.instance.ButtonUp();
            RolledNumberText.rollNumber = rollNumberArray;
            Array.Sort(rollNumberArray);
            Array.Sort(answerNumbers);

            if (rollNumberArray.SequenceEqual(answerNumbers))
                StartCoroutine( WinGame());
            else
                StartCoroutine(LoseGame());
            yield return null;
        }

        protected virtual IEnumerator WinGame()
        {
            // save clear data
            var isPerfect = GetSuccessRate() == 100;
            
            // Clear or Perfect text 표시
            if (isPerfect)
                ClearBoxText.instance.ShowPerfectText();
            else
                ClearBoxText.instance.ShowClearText();
            
            // Clear 정보 저장 
            var nowSceneName = SceneManager.GetActiveScene().name;
            if(PlayerPrefs.HasKey(nowSceneName))
                if(PlayerPrefs.GetInt(nowSceneName) != (int)StageState.Perfect)
                {
                    var stageState = isPerfect ? StageState.Perfect : StageState.Clear;
                    PlayerPrefs.SetInt(nowSceneName, (int) stageState);
                }
            if(PlayerPrefs.GetInt(nextSceneName) == (int)StageState.Lock)
                PlayerPrefs.SetInt(nextSceneName, (int)StageState.UnLock);
            
            RuntimeManager.PlayOneShot("event:/bgm/stage_clear");
            
            // 종료 대기, 후처리
            yield return new WaitForSeconds(3f);
            RolledNumberText.rollNumber = null;
            InputController.OnDeselect();
            try
            {
                AdController.instance.ShowInterstitialAd();
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                Console.WriteLine(e);
                throw;
            }
            SceneManager.LoadScene(nextSceneName);
        }

        protected virtual IEnumerator LoseGame()
        {
            ClearBoxText.instance.ShowFailText();
            RuntimeManager.PlayOneShot("event:/ui/ui_fail");
            yield return new WaitForSeconds(3f);
            RolledNumberText.rollNumber = null;
            InputController.OnDeselect();
            try
            {
                AdController.instance.ShowInterstitialAd();
            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                Console.WriteLine(e);
                throw;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void MakeBreakTrigger()
        {
            foreach (var dice in diceArray)
            {
                dice.MakeBreakTrigger();
            }
            foreach (var wall in wallArray)
            {
                wall.MakeBreakTrigger();
            }
        }

        public void MakeBreakNotTrigger()
        {
            foreach (var dice in diceArray)
            {
                dice.MakeBreakNotTrigger();
            }
            foreach (var wall in wallArray)
            {
                wall.MakeBreakNotTrigger();
            }
        }

        public int GetSuccessRate()
        {
            Array.Sort(answerNumbers);
            var pipOfDice = new int[diceArray.Length, 4];
            for (int i = 0; i < diceArray.Length; i++)
            {
                pipOfDice[i, 0] = diceArray[i].rightHoleCount;
                pipOfDice[i, 1] = diceArray[i].leftHoleCount;
                pipOfDice[i, 2] = diceArray[i].upHoleCount;
                pipOfDice[i, 3] = diceArray[i].downHoleCount;
            }

            var allAnswerArray = new int[(int)Math.Pow(4, diceArray.Length), diceArray.Length];
            for (int i_answerList = 0; i_answerList < Math.Pow(4,diceArray.Length); i_answerList++)
            {
                for (int i_diceArray = 0; i_diceArray < diceArray.Length; i_diceArray++)
                {
                    var pipIndex = i_answerList / (int) Mathf.Pow(4, i_diceArray) % 4;
                    try
                    {
                        allAnswerArray[i_answerList, i_diceArray] = pipOfDice[i_diceArray, pipIndex];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Debug.LogError($"인덱스 에러 [{i_answerList}, {i_diceArray}, {pipIndex}]");
                    }
                }
            }
            
            var success = 0;
            var fail = 0;
            for (int indexAllAnswer = 0; indexAllAnswer < (int)Mathf.Pow(4,diceArray.Length); indexAllAnswer++)
            {
                var answer = new int[diceArray.Length];
                for (int i = 0; i < diceArray.Length; i++)
                {
                    answer[i] = allAnswerArray[indexAllAnswer, i];
                }
                Array.Sort(answer);
                if (answer.SequenceEqual(answerNumbers))
                    success++;
                else
                    fail++;
            }
            return Mathf.FloorToInt(success / (success + fail + float.Epsilon) * 100);
        }

        public void PauseOrResumeGame()
        {
            PauseOrResumeGame(!isPaused);
        }
        public void PauseOrResumeGame(bool doPause)
        {
            if(!doPause)
            {
                isPaused = false;
                Time.timeScale = 1f;
                pauseCanvas.SetActive(false);
                InputController.instance.IsPaused = false;
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0f;
                pauseCanvas.SetActive(true);
                InputController.instance.IsPaused = true;
            }
        }

        public void ChangeLocale(int localeId)
        {
            if (m_ChangeLocalActive) return;
            StartCoroutine(SetLocale(localeId));
        }

        private IEnumerator SetLocale(int localeId)
        {
            m_ChangeLocalActive = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
            PlayerPrefs.SetInt("LocalKey", localeId);
            m_ChangeLocalActive = false;
        }
        
        public void MainMenu()
        {
            Destroy(BgmController.Instance.gameObject);
            PauseOrResumeGame(false);
            SceneManager.LoadScene("StageMenu");
        }
        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
        }
    }
}