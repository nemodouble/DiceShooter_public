using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class ClearBoxText : MonoBehaviour
    {
        public static ClearBoxText Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<ClearBoxText>();

                if (instance != null)
                    return instance;

                throw new UnityException();
            }
        }
        public static ClearBoxText instance;
        
        private TextMeshPro m_TextMeshPro;
        private Coroutine m_NowTextCoroutine;

        private void Start()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            m_TextMeshPro = GetComponent<TextMeshPro>();
            m_NowTextCoroutine = StartCoroutine(Blink());
        }

        public void ShowBlinkText()
        {
            StopCoroutine(m_NowTextCoroutine);
            m_TextMeshPro.color = Color.white;
            m_NowTextCoroutine = StartCoroutine(Blink());
        }

        public void ShowClearText()
        {
            StopCoroutine(m_NowTextCoroutine);
            m_TextMeshPro.text = "Clear";
            m_TextMeshPro.color = ColorCode.TextClearColor;
        }

        public void ShowFailText()
        {
            StopCoroutine(m_NowTextCoroutine);
            m_TextMeshPro.text = "Fail";
            m_TextMeshPro.color = ColorCode.TextFailColor;
        }
        
        public void ShowPerfectText()
        {
            StopCoroutine(m_NowTextCoroutine);
            m_TextMeshPro.text = "100%!\nPerfect";
            m_TextMeshPro.color = ColorCode.TextPerfectColor;
        }
        
        public void ShowRollingText()
        {
            StopCoroutine(m_NowTextCoroutine);
            m_TextMeshPro.color = Color.white;
            m_NowTextCoroutine = StartCoroutine(Rolling());
        }
        
        private IEnumerator Rolling()
        {
            while (true)
            {
                m_TextMeshPro.text = "Rolling\n⠀⠀⠀⠀⠀⠀⠀";
                yield return new WaitForSeconds(0.5f);
                m_TextMeshPro.text = "Rolling\n.⠀⠀⠀⠀⠀⠀";
                yield return new WaitForSeconds(0.5f);
                m_TextMeshPro.text = "Rolling\n..⠀⠀⠀⠀⠀";
                yield return new WaitForSeconds(0.5f);
                m_TextMeshPro.text = "Rolling\n...⠀⠀⠀⠀";
                yield return new WaitForSeconds(0.5f);
                m_TextMeshPro.text = "Rolling\n....⠀⠀⠀";
                yield return new WaitForSeconds(0.5f);
                m_TextMeshPro.text = "Rolling\n.....⠀⠀";
                yield return new WaitForSeconds(0.5f);
                m_TextMeshPro.text = "Rolling\n......⠀";
                yield return new WaitForSeconds(0.5f);
                m_TextMeshPro.text = "Rolling\n.......";
                yield return new WaitForSeconds(0.5f);
            }
        }
        public void ShowGunShootReact()
        {
            StopCoroutine(m_NowTextCoroutine);
            m_NowTextCoroutine = StartCoroutine(GunShootReact());
        }

        private IEnumerator GunShootReact()
        {
            m_TextMeshPro.text = "(°ロ°)";
            m_TextMeshPro.color = Color.grey;
            yield return new WaitForSeconds(0.3f);
            m_TextMeshPro.color = Color.white;
            m_NowTextCoroutine = StartCoroutine(Blink());
        }

        public void ShowBulletAdd(string gunName)
        {
            StopCoroutine(m_NowTextCoroutine);
            m_NowTextCoroutine = StartCoroutine(BulletAdd(gunName));
        }
        
        private IEnumerator BulletAdd(string gunName)
        {
            m_TextMeshPro.text =gunName + "\nBullet +1";
            for (int i = 0; i < 25; i++)
            {
                m_TextMeshPro.color = Color.yellow;
                yield return new WaitForSeconds(0.1f);
                m_TextMeshPro.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }
            m_NowTextCoroutine = StartCoroutine(Blink());
        }
        
        private IEnumerator Blink()
        {
            while(true)
            {
                m_TextMeshPro.text = "(o w o)";
                yield return new WaitForSeconds(Random.Range(3f, 5f));
                m_TextMeshPro.text = "(- w -)";
                yield return new WaitForSeconds(0.2f);
                m_TextMeshPro.text = "(o w o)";
                yield return new WaitForSeconds(0.2f);
                m_TextMeshPro.text = "(- w -)";
                yield return new WaitForSeconds(0.2f);
            }
        }

        
        
        
        
    }
}