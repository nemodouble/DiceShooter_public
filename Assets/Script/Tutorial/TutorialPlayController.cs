using System;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class TutorialPlayController : MonoBehaviour
    {
        public GameObject tutorialCanvas;

        private enum NextStepTodo
        {
            PlaceDice,
            PlaceGun1,
            ShootGun1,
            PlaceGun2,
            ShootGun2,
            Roll,
            End,
            Restart
        }
        private NextStepTodo m_NextStepTodo;

        public Dice dice;
        public Gun gun;
        public ClearButton clearButton;

        public bool isEnd;

        private void OnEnable()
        {
            InputController.instance.IsPaused = false;
            tutorialCanvas.transform.Find("BeforeButton").GetComponent<Button>().interactable = false;
            tutorialCanvas.transform.Find("NextButton").GetComponent<Button>().interactable = false;
            SetStep(NextStepTodo.PlaceDice);
        }
        
        private void Update()
        {
            if (isEnd)
            {
                if (m_NextStepTodo is NextStepTodo.Restart && dice.fittedDiceHole is not null)
                {
                    SetStep(NextStepTodo.PlaceDice);
                    isEnd = false;
                }
                else
                {
                    return;
                }
            }
            if (dice.fittedDiceHole == null)
            {
                SetStep(NextStepTodo.PlaceDice);
            }
            else if (!gun.isReloaded && gun.leftBullet == 2)
            {
                SetStep(NextStepTodo.PlaceGun1);
            }
            else if (gun.leftBullet == 2)
            {
                SetStep(NextStepTodo.ShootGun1);
            }
            else if (!gun.isReloaded && gun.leftBullet == 1)
            {
                SetStep(NextStepTodo.PlaceGun2);
            }
            else if (gun.leftBullet == 1)
            {
                SetStep(NextStepTodo.ShootGun2);
            }
            else if (!clearButton.isClicked)
            {
                SetStep(NextStepTodo.Roll);
            }
        }

        public void Success()
        {
            SetStep(NextStepTodo.End);
            isEnd = true;
            tutorialCanvas.transform.Find("BeforeButton").GetComponent<Button>().interactable = true;
            tutorialCanvas.transform.Find("NextButton").GetComponent<Button>().interactable = true;
            InputController.instance.IsPaused = true;
            gun.ResetGun(true);
        }

        public void Fail()
        {
            SetStep(NextStepTodo.Restart);
            gun.ResetGun(true);
            gun.leftBullet = 2;
            dice.ResetDice();
            dice.transform.rotation = Quaternion.identity;
            dice.gameObject.layer = LayerMask.NameToLayer("ClickAble");
            dice.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            isEnd = true;
            InputController.instance.IsPaused = false;
        }
        
        private void SetStep(NextStepTodo nextStep)
        {
            m_NextStepTodo = nextStep;
            var numOfChild = transform.childCount;
            for (int i = 0; i < numOfChild; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i == (int)nextStep);
            }
        }
    }
}