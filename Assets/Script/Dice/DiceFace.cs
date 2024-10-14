using System;
using TMPro;
using UnityEngine;

namespace Script
{
    [ExecuteInEditMode]
    public class DiceFace : MonoBehaviour, IGunHitAble
    {
        public Dice dice;
        public Dice.DiceFaceType faceType;
        public IGunHitAble.HitReact hitReact;
        public PhysicsMaterial2D fullBounce;
        public DiceFaceCount diceFaceCount;

        public Material outlineMaterial;
        
        public int bulletCountScale = 1;
        
        public bool used;
        private SpriteRenderer m_SpriteRenderer;

        private void Start()
        {
            dice = transform.parent.GetComponent<Dice>();
            if(dice == null)
                Debug.LogError("부모 없는 주사위 면");
            diceFaceCount = transform.Find("Count").GetComponent<DiceFaceCount>();
            m_SpriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
            SetFaceType(faceType);
            SetHitReact(hitReact);
        }

        public void ActiveOutline(bool active = true)
        {
            if(active)
            {
                m_SpriteRenderer.color = ColorCode.TextPerfectColor;
                diceFaceCount.GetComponent<TextMeshPro>().color = ColorCode.Orange;
            }
            else
            {
                m_SpriteRenderer.color = GetMyColor();
                diceFaceCount.GetComponent<TextMeshPro>().color = Color.white;
            }
        }

        public void MakeBreakTrigger()
        {
            if (hitReact == IGunHitAble.HitReact.Break || hitReact == IGunHitAble.HitReact.Strong)
                GetComponent<Collider2D>().isTrigger = true;
        }

        public void MakeBreakNotTrigger()
        {
            if (hitReact == IGunHitAble.HitReact.Break || hitReact == IGunHitAble.HitReact.Strong)
                GetComponent<Collider2D>().isTrigger = false;
        }
        
        public void SetHitReact(IGunHitAble.HitReact hitReact)
        {
            var co = GetComponent<Collider2D>();
            switch (hitReact)
            {
                case IGunHitAble.HitReact.Break:
                    co.isTrigger = false;
                    gameObject.layer = LayerMask.NameToLayer("GunHit");
                    break;
                case IGunHitAble.HitReact.Pierce:
                    co.isTrigger = true;
                    gameObject.layer = LayerMask.NameToLayer("GunHitPass");
                    break;
                case IGunHitAble.HitReact.Reflect:
                    co.isTrigger = false;
                    gameObject.layer = LayerMask.NameToLayer("GunHitReflect");
                    co.sharedMaterial = fullBounce;
                    break;
                case IGunHitAble.HitReact.Strong:
                    co.isTrigger = false;
                    gameObject.layer = LayerMask.NameToLayer("GunHit");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hitReact), hitReact, null);
            }
            m_SpriteRenderer.color = GetMyColor();
        }
        
        private Color GetMyColor()
        {
            switch (hitReact)
            {
                case IGunHitAble.HitReact.Break:
                    return ColorCode.BreakColor;
                case IGunHitAble.HitReact.Pierce:
                    return ColorCode.NormalColor;
                case IGunHitAble.HitReact.Reflect:
                    return ColorCode.ReflectColor;
                case IGunHitAble.HitReact.Strong:
                    return ColorCode.StrongColor;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetFaceType(Dice.DiceFaceType ft)
        {
            switch (ft)
            {
                case Dice.DiceFaceType.Left:
                    transform.rotation = Quaternion.Euler(0,0, 180);
                    break;
                case Dice.DiceFaceType.Right:
                    transform.rotation = Quaternion.Euler(0,0, 0);
                    break;
                case Dice.DiceFaceType.Up:
                    transform.rotation = Quaternion.Euler(0,0, 90);
                    break;
                case Dice.DiceFaceType.Down:
                    transform.rotation = Quaternion.Euler(0,0, 270);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IGunHitAble.HitReact GetBulletReactType()
        {
            return hitReact;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var bl = col.collider.GetComponent<Bullet>();
            if(bl != null)
            {
                switch (hitReact)
                {
                    case IGunHitAble.HitReact.Break:
                        GunHit(bl.shooter);
                        bl.Broken(1f);
                        break;
                    case IGunHitAble.HitReact.Pierce:
                        GunHit(bl.shooter);
                        break;
                    case IGunHitAble.HitReact.Reflect:
                        bl.HitColList.Clear();
                        GunHit(bl.shooter);
                        break;
                    case IGunHitAble.HitReact.Strong:
                        bl.HitColList.Clear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var bl = other.GetComponent<Bullet>();
            if(bl != null)
            {
                transform.parent.GetComponent<Dice>().AddHoleCount(faceType, bl.shooter.bulletDamage * bulletCountScale);
                switch (hitReact)
                {
                    case IGunHitAble.HitReact.Break:
                        break;
                    case IGunHitAble.HitReact.Pierce:
                        break;
                    case IGunHitAble.HitReact.Reflect:
                        break;
                    case IGunHitAble.HitReact.Strong:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool GunHit(Gun gun)
        {
            if (dice.fittedDiceHole == null)
                return false;
            diceFaceCount.CountUp(gun.bulletDamage);
            transform.parent.GetComponent<Dice>().AddHoleCount(faceType, gun.bulletDamage * bulletCountScale);
            return true;
        }

        public void LayerUp()
        {
            m_SpriteRenderer.sortingOrder += 5;
        }

        public void LayerDown()
        {
            m_SpriteRenderer.sortingOrder -= 5;
        }
    }
}
