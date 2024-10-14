using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public bool isSniper;

    private void Awake()
    {
        SetLineRenderer();
    }

    private void SetLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();
    }
    
    public void StopLaser()
    {
        lineRenderer.positionCount = 0;
    }
    // Start is called before the first frame update
    public void ShootLaser(Vector2 start, Vector2 dir)
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, start);
        var hitCount = 0;
        while(hitCount < 100)
        {
            hitCount++;
            var raycastMask = LayerMask.GetMask("GunHitReflect");
            if (!isSniper)
                raycastMask += LayerMask.GetMask("GunHit");
            var hit = Physics2D.Raycast(start, dir, 1000f, raycastMask);
            if (hit.collider == null)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(hitCount, start + dir * 1000f);
                break;
            }

            lineRenderer.positionCount++;
            lineRenderer.SetPosition(hitCount, hit.point);
            var bulletReactType = hit.collider.GetComponent<IGunHitAble>().GetBulletReactType();
            switch (bulletReactType)
            {
                case IGunHitAble.HitReact.Break:
                case IGunHitAble.HitReact.Strong:
                    return;
                case IGunHitAble.HitReact.Reflect:
                    dir = Vector2.Reflect(dir, hit.normal);
                    start = hit.point + 0.0000005f * dir;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
