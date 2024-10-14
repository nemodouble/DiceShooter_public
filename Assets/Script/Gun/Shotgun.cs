using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    public class Shotgun : Gun
    {
        private const float Angle = 45f;
        public int pelletCount = 3;
        public LaserShooter[] laserShooters;

        protected override void ShootLaser(Vector2 dir)
        {
            for (int i = 0; i < laserShooters.Length; i++)
            {
                var pelletAngle = Angle / (pelletCount - 1) * i - Angle / 2;
                var upPelletDir = Rotate(dir, pelletAngle);
                laserShooters[i].ShootLaser(transform.position, upPelletDir);
            }
        }

        protected override void StopLaser()
        {
            foreach (var shooter in laserShooters)
            {
                shooter.StopLaser();
            }
        }

        protected override IEnumerator ShootBullet(Vector2 start, Vector2 dir)
        {
            leftBullet += 2;
            for (int i = 0; i < pelletCount; i++)
            {
                var pelletAngle = Angle / (pelletCount - 1) * i - Angle / 2;
                var upPelletDir = Rotate(dir, pelletAngle);
                StartCoroutine(base.ShootBullet(start, upPelletDir));
            }
            yield return null;
        }

        private static Vector2 Rotate(Vector2 v, float degrees) {
            var sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            var cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
            var tx = v.x;
            var ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}