using System;
using TMPro;
using UnityEngine;

namespace Script
{
    public class BulletLeft : MonoBehaviour
    {
        private TextMeshPro m_TextMeshPro;
        protected void Start()
        {
            m_TextMeshPro = GetComponent<TextMeshPro>();
            BulletUse(transform.parent.GetComponent<Gun>().leftBullet);
        }

        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }

        public void BulletUse(int leftBullet)
        {
            var answer = "x" + leftBullet;
            m_TextMeshPro.text = answer;
        }
    }
}