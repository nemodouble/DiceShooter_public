using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    [RequireComponent(typeof(Collider2D))]
    public class SceneChangeButton : MonoBehaviour, IButton
    {
        public string sceneName;
        public bool changeBGM;
        
        public bool Clicked()
        {
            if (changeBGM)
                Destroy(BgmController.Instance.gameObject);
            SceneManager.LoadScene(sceneName);
            return true;
        }
    }
}