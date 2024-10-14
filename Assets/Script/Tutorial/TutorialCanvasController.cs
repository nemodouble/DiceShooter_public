using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class TutorialCanvasController : MonoBehaviour
    {
        public TutorialController tutorialController;

        private TextMeshProUGUI m_StageLeftTextMeshPro;

        private void Start()
        {
            m_StageLeftTextMeshPro = transform.Find("LeftText").Find("Text").GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            var nowStep = tutorialController.tutorialCount + 1;
            var entireStep = tutorialController.transform.childCount;
            if (nowStep == entireStep)
            {
                m_StageLeftTextMeshPro.text = "Start!";
                transform.Find("LeftText").GetComponent<Button>().interactable = true;
                transform.Find("GunCancel").gameObject.SetActive(true);
            }
            else
            {
                m_StageLeftTextMeshPro.text = nowStep + "/" + entireStep;
                transform.Find("LeftText").GetComponent<Button>().interactable = false;
                transform.Find("GunCancel").gameObject.SetActive(false);
            }
        }
    }
}