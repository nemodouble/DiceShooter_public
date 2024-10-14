using System;
using UnityEngine;
using UnityEngine.Events;

namespace Script
{
    public class ActionButton : MonoBehaviour, IButton
    {
        public UnityEvent onClick;
        public bool Clicked()
        {
            onClick.Invoke();
            return true;
        }
    }
}