using System;
using FMODUnity;
using UnityEngine;

namespace Script
{
    [ExecuteInEditMode]
    public class Wall : MonoBehaviour , IGunHitAble
    {
        public bool isHorizon;
        public bool isDot;
        
        public int cellX;
        public int cellY;

        public IGunHitAble.HitReact m_HitReact;
        public PhysicsMaterial2D fullBounce;

        public EventReference wallReflectEvent;

        private void Start()
        {
            switch (m_HitReact)
            {
                case IGunHitAble.HitReact.Break:
                    GetComponent<BoxCollider2D>().isTrigger = false;
                    gameObject.layer = LayerMask.NameToLayer("GunHit");
                    transform.Find("Sprite").GetComponent<SpriteRenderer>().color = ColorCode.BreakColor;
                    break;
                case IGunHitAble.HitReact.Pierce:
                    GetComponent<BoxCollider2D>().isTrigger = true;
                    gameObject.layer = LayerMask.NameToLayer("GunHitPass");
                    transform.Find("Sprite").GetComponent<SpriteRenderer>().color = ColorCode.NormalColor;
                    break;
                case IGunHitAble.HitReact.Reflect:
                    GetComponent<BoxCollider2D>().isTrigger = false;
                    gameObject.layer = LayerMask.NameToLayer("GunHitReflect");
                    GetComponent<BoxCollider2D>().sharedMaterial = fullBounce;
                    transform.Find("Sprite").GetComponent<SpriteRenderer>().color = ColorCode.ReflectColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void Update()
        {
            var boxCollider2D = GetComponent<BoxCollider2D>();
            transform.position = new Vector3(cellX * 2f, cellY * 2f, 0);
            if (isDot)
            {
                boxCollider2D.offset = new Vector2(0, 0);
                boxCollider2D.size = new Vector2(0.5f, 0.5f);
            }
            else
            {
                if (isHorizon)
                {
                    boxCollider2D.offset = new Vector2(-1f, 0);
                    boxCollider2D.size = new Vector2(1.5f, 0.5f);
                }
                else
                {
                    boxCollider2D.offset = new Vector2(0, -1f);
                    boxCollider2D.size = new Vector2(0.5f, 1.5f);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.GetComponent<Bullet>() == null) return; 
            switch (m_HitReact)
            {
                case IGunHitAble.HitReact.Break:
                    other.collider.GetComponent<Bullet>().Broken(1f);
                    break;
                case IGunHitAble.HitReact.Pierce:
                    break;
                case IGunHitAble.HitReact.Reflect:
                    other.collider.GetComponent<Bullet>().HitColList.Clear();
                    RuntimeManager.PlayOneShot(wallReflectEvent);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void MakeBreakTrigger()
        {
            if (m_HitReact == IGunHitAble.HitReact.Break)
                GetComponent<Collider2D>().isTrigger = true;
        }

        public void MakeBreakNotTrigger()
        {
            if (m_HitReact == IGunHitAble.HitReact.Break)
                GetComponent<Collider2D>().isTrigger = false;
            
        }
        public IGunHitAble.HitReact GetBulletReactType()
        {
            return m_HitReact;
        }

        public bool GunHit(Gun gun)
        {
            // Do nothing
            return true;
        }
    }
}
