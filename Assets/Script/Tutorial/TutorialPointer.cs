using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class TutorialPointer : MonoBehaviour
    {
        private Vector2 m_StartPosition;
        private Vector2 m_EndPosition;
        private Vector2 m_OriginPosition;
        public float movingSpeed;

        public float pointerWaitTime;
        private WaitForSeconds m_PointerWaitForSeconds;

        private SpriteRenderer m_SpriteRenderer;
        public Sprite notClickSprite;
        public Sprite clickSprite;

        public Coroutine nowCoruoutine;
        
        private void OnEnable()
        {
            m_OriginPosition = transform.position;
            transform.position = m_StartPosition;

            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_StartPosition = transform.Find("StartPos").transform.position;
            m_EndPosition = transform.Find("EndPos").transform.position;
            
            m_PointerWaitForSeconds = new WaitForSeconds(pointerWaitTime / 2);

            nowCoruoutine = StartCoroutine(MoveStartToEnd());
        }

        private void OnDisable()
        {
            transform.position = m_OriginPosition;
            StopCoroutine(nowCoruoutine);
        }

        private IEnumerator MoveStartToEnd()
        {
            var movingLength = (m_EndPosition - m_StartPosition).normalized * movingSpeed;
            while (true)
            {
                transform.position = m_StartPosition;
                yield return m_PointerWaitForSeconds;
                m_SpriteRenderer.sprite = clickSprite;
                yield return m_PointerWaitForSeconds;
                
                while(((Vector2)transform.position - m_StartPosition).magnitude <= (m_EndPosition - m_StartPosition).magnitude)
                {
                    transform.position += (Vector3)(movingLength * Time.deltaTime);
                    yield return null;
                }
                
                transform.position = m_EndPosition;
                yield return m_PointerWaitForSeconds;
                m_SpriteRenderer.sprite = notClickSprite;
                yield return m_PointerWaitForSeconds;
            }   
        }
    }
}