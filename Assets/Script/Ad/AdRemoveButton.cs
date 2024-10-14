using UnityEngine;
using UnityEngine.UI;

namespace Script.Ad
{
    public class AdRemoveButton : MonoBehaviour, IButton
    {
        public void RemoveAd()
        {
            AdController.instance.RemoveAd();
        }

        public void RemoveFail()
        {
            Debug.Log("RemoveFail");
        }

        public bool Clicked()
        {
            GetComponent<Button>().onClick.Invoke();
            return true;
        }
    }
}