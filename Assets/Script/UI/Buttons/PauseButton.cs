using UnityEngine;

namespace Script
{
    public class PauseButton : MonoBehaviour, IButton
    {
        public bool Clicked()
        {
            GameController.instance.PauseOrResumeGame(true);
            return true;
        }
    }
}