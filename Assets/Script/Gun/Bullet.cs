using System.Collections;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace Script
{
    public class Bullet : MonoBehaviour
    {
        public float lifeTime = 1000f;
        public Gun shooter;

        public EventReference wallPass;

        public ArrayList HitColList;

        private void Start()
        {
            Invoke(nameof(DestroySelf), lifeTime);
            HitColList = new ArrayList();
        }

        public void Broken(float lifetime)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Invoke(nameof(DestroySelf), lifetime);
        }
        
        private void DestroySelf()
        {
            Destroy(gameObject);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (HitColList.Contains(col)) return;
            HitColList.Add(col);
            var gh = col.GetComponent<IGunHitAble>();
            if (gh != null)
            {
                if(gh.GunHit(shooter))
                   RuntimeManager.PlayOneShot(wallPass);
            }
        }
    }
}