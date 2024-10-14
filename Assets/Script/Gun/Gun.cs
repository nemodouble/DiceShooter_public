using System;
using System.Collections;
using Cinemachine;
using FMODUnity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Script
{
    public class Gun : MonoBehaviour
    {
        public Material outlineMaterial;
        public Material reloadOutlineMaterial;
        [SerializeField] private SpriteRenderer m_SpriteRenderer;
            
        public int bulletDamage = 1;
        public int bulletCount = 1;
        public int leftBullet;
        public GameObject bullet;
        private float bulletSpeed = 30f;
        public bool isSniper;
        
        public LaserShooter laserShooter;
        public CinemachineImpulseSource impulseSource;
        public float recoilAngle = 45f;
        public float recoilTime = 0.02f;
        private Coroutine m_ShootRecoilCoroutine;

        public bool isReloaded;
        public float reloadTime;
        public Vector2 mousePos;

        protected Vector2 ResetPosition;

        public EventReference reloadSound;
        public EventReference shootSound;

        
        protected virtual void Start()
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
            ResetPosition = transform.position;
            gameObject.layer = LayerMask.NameToLayer("ClickAble");
        }

        private void Update()
        {
            var toMouseDir = mousePos - (Vector2)transform.position;

            if(isReloaded)
            {
                if(m_ShootRecoilCoroutine != null)
                    StopCoroutine(m_ShootRecoilCoroutine);
                // 총 방향 회전
                var angle = toMouseDir == Vector2.zero ? 90 : Vector2.Angle(toMouseDir, Vector2.down);
                var lookDir = toMouseDir.x >= 0 ? 1 : -1;
                var tf = transform;
                tf.rotation = Quaternion.Euler(0, 0,  lookDir * angle - 90);
                m_SpriteRenderer.flipY = lookDir != 1;
                
                // 총알 궤적 예측
                ShootLaser(toMouseDir);
            }
        }

        public void Selected()
        {
            RuntimeManager.PlayOneShot(reloadSound);
            gameObject.layer = LayerMask.NameToLayer("ClickDisabled");
            transform.Find("Gun Shadow").gameObject.SetActive(false);
            
            m_SpriteRenderer.material = outlineMaterial;
            m_SpriteRenderer.sortingOrder += 10;
            transform.rotation = Quaternion.Euler(0,0,0);
            m_SpriteRenderer.transform.rotation = Quaternion.Euler(0,0,0);
        }
        
        public bool ShootGun(Vector2 position)
        {
            if (!isReloaded) return false; 
            // 바로 클릭을 뗀 경우 발사 x
            if (Time.time - reloadTime < 0.3f) return false;
            // 조준 거리가 너무 가까운 경우 발사 x
            if (Vector2.Distance(position, transform.position) < 0.01f) return false;
            
            var toMouseDir = position - (Vector2)transform.position;
            StartCoroutine(ShootBullet(transform.position, toMouseDir));
            m_ShootRecoilCoroutine = StartCoroutine(ShootRecoil());
            RuntimeManager.PlayOneShot(shootSound);
            
            isReloaded = false;
            StopLaser();
            m_SpriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
            return true;
        }
        
        public virtual void ReloadGun()
        {
            reloadTime = Time.time;
            isReloaded = true;
            m_SpriteRenderer.material = reloadOutlineMaterial;
            RuntimeManager.PlayOneShot(reloadSound);
        }
        
        public void ResetGun(bool moveToOriginPos = false)
        {
            if (moveToOriginPos)
            {
                transform.position = ResetPosition;
                transform.rotation = Quaternion.identity;
            }
            isReloaded = false;
            var tf = transform;
            tf.localScale = new Vector3(1, 1, 0);
            tf.rotation = Quaternion.identity;
            m_SpriteRenderer.flipY = false;
            m_SpriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
            m_SpriteRenderer.sortingOrder -= 10;
            StopLaser();
        }
        
        protected virtual IEnumerator ShootBullet(Vector2 start, Vector2 dir)
        {
            impulseSource.GenerateImpulse();
            AddBullet(-1);
            for (int i = 0; i < bulletCount; i++)
            {
                GameController.instance.isShooted = true;
                var bl = Instantiate(bullet, start, Quaternion.identity);
                bl.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;
                bl.GetComponent<Bullet>().shooter = this;
                yield return new WaitForSeconds(0.1f);
            }
            if (leftBullet <= 0)
            {
                m_SpriteRenderer.sortingOrder -= -1000;
                gameObject.layer = LayerMask.NameToLayer("ClickDisabled");
            }
        }

        protected virtual IEnumerator ShootRecoil()
        {
            return ShootRecoil(recoilAngle, recoilTime);
        }
        
        protected virtual IEnumerator ShootRecoil(float recoilAngle, float recoilTime)
        {
            // 회전시킬 각도 계산
            var tf = m_SpriteRenderer.transform;
            var startRot = tf.rotation;
            var endRotAngle = startRot.eulerAngles.z + (m_SpriteRenderer.flipY ? -recoilAngle : recoilAngle);
            var endRot = Quaternion.Euler(0, 0, endRotAngle);
            
            var time = 0f;
            while (time < recoilTime)
            {
                tf.rotation = Quaternion.Lerp(startRot, endRot, time / recoilTime);
                time += Time.deltaTime;
                yield return null;
            }
            
            tf.rotation = endRot;
        }

        public void AddBullet(int value)
        {
            if (leftBullet == 0 && leftBullet+value > 0)
            {
                m_SpriteRenderer.sortingOrder += 1000;
                gameObject.layer = LayerMask.NameToLayer("ClickAble");
            }
            leftBullet += value;
            transform.Find("LeftBullet").GetComponent<BulletLeft>().BulletUse(leftBullet);
        }
        
        protected virtual void ShootLaser(Vector2 toMouseDir)
        {
            laserShooter.ShootLaser(transform.position, toMouseDir);
        }

        protected virtual void StopLaser()
        {
            laserShooter.StopLaser();
        }
    }
}
