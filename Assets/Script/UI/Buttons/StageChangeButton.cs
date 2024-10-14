using System;
using UnityEngine;

namespace Script
{
    public class StageChangeButton : MonoBehaviour, IButton
    {
        public int changeNum;
        private StageController m_StageController;

        private void Start()
        {
            m_StageController = transform.parent.GetComponent<StageController>();
        }

        public bool Clicked()
        {
            m_StageController.UpdateFirstStageNum(m_StageController.nowStageNum + changeNum);
            return true;
        }
    }
}