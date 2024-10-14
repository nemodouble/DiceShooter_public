using System;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Script.StageController;

namespace Script
{
    public class StageSelectButton : MonoBehaviour, IButton
    {
        private int firstStageNum;
        public int secondStageNum;

        public Sprite lockSprite;
        public Sprite unlockSprite;
        public Sprite clearSprite;
        
        private SpriteRenderer m_SpriteRenderer;
        private TextMeshPro m_TextMeshPro;
        private GameObject m_LockedCoverSprite;
        private GameObject m_CrownSprite;

        private StageState m_NowState;
        
        private void Awake()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            firstStageNum = transform.parent.GetComponent<StageController>().nowStageNum;
            
            m_TextMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
            m_LockedCoverSprite = transform.Find("Locked").gameObject;
            m_CrownSprite = transform.Find("crown").gameObject;
            
            transform.Find("Text").GetComponent<TextMeshPro>().text = secondStageNum.ToString();
        }

        public void UpdateFirstStageNum(int stageNum)
        {
            firstStageNum = stageNum;
            LoadIsClear();
        }

        private void LoadIsClear()
        {
            var stageName = firstStageNum + "-" + secondStageNum;
            if (!PlayerPrefs.HasKey(stageName))
            {
                PlayerPrefs.SetInt(stageName, (int)StageState.Lock);
            }

            m_NowState = (StageState) PlayerPrefs.GetInt(stageName);
            switch (m_NowState)
            {
                case StageState.Lock:
                    m_SpriteRenderer.sprite = lockSprite;
                    m_TextMeshPro.color = Color.clear;
                    m_LockedCoverSprite.SetActive(true);
                    m_CrownSprite.SetActive(false);
                    break;
                case StageState.UnLock:
                    m_SpriteRenderer.sprite = unlockSprite;
                    m_TextMeshPro.color = Color.white;
                    m_LockedCoverSprite.SetActive(false);
                    m_CrownSprite.SetActive(false);
                    break;
                case StageState.Clear:
                    m_SpriteRenderer.sprite = clearSprite;
                    m_TextMeshPro.color = Color.grey;
                    m_LockedCoverSprite.SetActive(false);
                    m_CrownSprite.SetActive(false);
                    break;
                case StageState.Perfect:
                    m_SpriteRenderer.sprite = clearSprite;
                    m_TextMeshPro.color = new Color(0xfb / 255f, 0xf2 / 255f, 0x36 / 255f);
                    m_LockedCoverSprite.SetActive(false);
                    m_CrownSprite.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool Clicked()
        {
            if (m_NowState == StageState.Lock)
            {
                RuntimeManager.PlayOneShot("event:/ui/ui_beep");
                return false;
            }
            Destroy(BgmController.Instance.gameObject);
            if (!PlayerPrefs.HasKey("ClearTutorial"))
                SceneManager.LoadScene("Tutorial");
            else
                SceneManager.LoadScene(firstStageNum + "-" + secondStageNum);
            return true;
        }
    }
}