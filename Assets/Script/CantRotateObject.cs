using System;
using UnityEngine;

namespace Script
{
    public class CantRotateObject : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }
    }
}