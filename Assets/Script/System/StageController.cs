using System;
using TMPro;
using UnityEngine;

namespace Script
{
    public class StageController : MonoBehaviour
    {
        public int nowStageNum;
        public int minStageNum;
        public int maxStageNum;

        private StageSelectButton[] StageButtons;
        
        public enum StageState
        {
            Lock,
            UnLock,
            Clear,
            Perfect
        }
        
        private void Start()
        {
            if(!PlayerPrefs.HasKey("1-1"))
                PlayerPrefs.SetInt("1-1", (int)StageState.UnLock);
            
            var objects = GameObject.FindGameObjectsWithTag("StageSelect");
            StageButtons = new StageSelectButton[objects.Length];
            for (var i = 0; i < objects.Length; i++)
            {
                StageButtons[i] = objects[i].GetComponent<StageSelectButton>();
            }
            UpdateFirstStageNum(nowStageNum);
        }

        public void UpdateFirstStageNum(int num)
        {
            if (num < minStageNum || num > maxStageNum) return;
            nowStageNum = num;
            transform.Find("StageNum").GetComponent<TextMeshPro>().text = nowStageNum.ToString();
            foreach (var button in StageButtons)
            {
                button.UpdateFirstStageNum(nowStageNum);
            }
        }
    }
}