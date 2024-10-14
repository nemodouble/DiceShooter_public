using FMODUnity;
using UnityEngine;

namespace Script
{
    [ExecuteInEditMode]
    public class DiceHole : MonoBehaviour
    {
        public GameObject dice;

        public bool diceLocked;
        
        public int cellX;
        public int cellY;
        void Update()
        {
            transform.position = new Vector3(cellX * 2f + 1f, cellY * 2f + 1f, 0);
        }

        public bool HaveDice()
        {
            return dice != null;
        }
        public void PlaceDice(GameObject dice)
        {
            if (diceLocked || this.dice != null)
            {
                dice.GetComponent<Dice>().ResetDice();
                return;
            }
            
            gameObject.layer = LayerMask.NameToLayer("ClickAble");
            
            RuntimeManager.PlayOneShot("event:/ui/dice_place");
            var diceController = dice.GetComponent<Dice>();
            if (diceController.fittedDiceHole != null)
            {
                diceController.ResetDice();
            }
            this.dice = dice;
            dice.transform.position = transform.position;
            diceController.PlaceDice(this);
        }
    }
}
