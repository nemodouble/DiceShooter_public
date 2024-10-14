
using UnityEngine;

namespace Script
{
    public interface IGunHitAble
    {
        public enum HitReact
        {
            Break,
            Pierce,
            Reflect,
            Strong
        }
        public HitReact GetBulletReactType();
        public bool GunHit(Gun gun);
    }
}