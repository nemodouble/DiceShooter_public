using System;
using UnityEngine;

namespace Script
{
    public class RollingDiceGenerator : MonoBehaviour
    {
        public GameObject rollingDice;
        public bool spawnDice = true;
        
        private float m_Timer;
        
        private void Update()
        {
            if (!spawnDice) return;
            
            m_Timer += Time.deltaTime;
            if (m_Timer >= 5f)
            {
                Instantiate(rollingDice, transform);
                m_Timer = 0;
            }
        }
    }
}