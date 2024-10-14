using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

namespace Script
{
    [RequireComponent(typeof(Collider2D))]
    public class ClearButton : MonoBehaviour, IButton
    {
        public Sprite notClicked;
        public Sprite clicked;

        private SpriteRenderer m_SpriteRenderer;
        private Collider2D m_Collider2D;

        public bool isClicked;
        public static ClearButton Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<ClearButton>();

                if (instance != null)
                    return instance;

                Create ();

                return instance;
            }
        }
        public static ClearButton instance;

        public static ClearButton Create ()
        {
            GameObject sceneControllerGameObject = new GameObject("GameController");
            instance = sceneControllerGameObject.AddComponent<ClearButton>();

            return instance;
        }
        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Collider2D = GetComponent<Collider2D>();
        }

        public bool Clicked()
        {
            m_SpriteRenderer.sprite = clicked;
            RuntimeManager.PlayOneShot("event:/ui/ui_button");
            ButtonDown();
            StartCoroutine(GameController.instance.RollDice());
            return true;
        }

        public void ButtonUp()
        {
            isClicked = false;
            m_SpriteRenderer.sprite = notClicked;
            m_Collider2D.enabled = true;
        }

        private void ButtonDown()
        {
            isClicked = true;
            m_SpriteRenderer.sprite = clicked;
            m_Collider2D.enabled = false;
        }
    }
}
