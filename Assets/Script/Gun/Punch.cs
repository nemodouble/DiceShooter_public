using System.Collections;
using FMODUnity;
using UnityEngine;

namespace Script
{
    public class Punch : Gun
    {
        public override void ReloadGun()
        {
            isReloaded = false;
            StartCoroutine(ShootBullet(transform.position, Vector2.zero));
        }
        
        protected override IEnumerator ShootBullet(Vector2 start, Vector2 dir)
        {
            transform.Find("Sprite").gameObject.SetActive(false);
            var layerMask = LayerMask.GetMask("GunHit")
                            + LayerMask.GetMask("GunHitPass")
                            + LayerMask.GetMask("GunHitReflect");
            var hitList = new RaycastHit2D[10];
            var size = Physics2D.RaycastNonAlloc(start, Vector2.zero, hitList, 10f, layerMask);

            if (size == 0) yield break;
            for (int i = 0; i < size; i++)
            {
                hitList[i].collider.GetComponent<IGunHitAble>().GunHit(this);
            }
            if (--leftBullet == 0)
                Destroy(gameObject);

            transform.Find("Sprite").gameObject.SetActive(false);
            RuntimeManager.PlayOneShot(shootSound);
            transform.position = ResetPosition;
            yield return null;
        }

        protected override void StopLaser()
        {
            // Doesn't have laser. Do nothing
        }
    }
}