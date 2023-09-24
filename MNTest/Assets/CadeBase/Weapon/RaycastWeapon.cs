using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CadeBase.Weapon
{
    public class RaycastWeapon : MonoBehaviour
    {

        public class Bullet
        {
            public float Time;
            public Vector3 InitialPosition;
            public Vector3 InitialVelocity;
            public TrailRenderer BulletTracerEffect;
        }
        public ParticleSystem MuzzleFlash;
        public ParticleSystem HitEffect;
        public GameObject RaycastOrigin;
        public Transform RaycastDestination;
        public TrailRenderer TracerEffect;
        public AnimationClip WeaponAnimation;
        public float BulletSpeed = 1000f;
        public float BUlletDrop;
        public int FireRate = 25;
        public bool IsFiring;

        private Ray _ray;
        private RaycastHit _hitInfo;
        private float _accumulatedTime;
        private List<Bullet> bullets = new();
        private float _maxLifeTime = 3f;

        private Vector3 GetPosition(Bullet bullet)
        {
            Vector3 gravity = Vector3.down * BUlletDrop;
            return (bullet.InitialPosition) + (bullet.InitialVelocity * bullet.Time) +
                   (0.5f * gravity * bullet.Time);
        }

        private Bullet CreateBullet(Vector3 position, Vector3 velocity)
        {
            Bullet bullet = new Bullet();
            bullet.InitialPosition = position;
            bullet.InitialVelocity = velocity;
            bullet.Time = 0;
            bullet.BulletTracerEffect = Instantiate(TracerEffect, position, quaternion.identity);
            bullet.BulletTracerEffect.AddPosition(position);
            return bullet;
        }
        
        public void StartFiring()
        {
            IsFiring = true;
            _accumulatedTime = 0;
            FireBullet();
        }

        public void UpdateFiring(float deltaTime)
        {
            _accumulatedTime += deltaTime;
            float fireInterval = 1f / FireRate;
            while (_accumulatedTime >= 0)
            {
                FireBullet();
                _accumulatedTime -= fireInterval;
            }
        }

        public void UpdateBullet(float deltaTime)
        {
            SimulateBullets(deltaTime);
            DestroyBullets();
        }

        private void SimulateBullets(float deltaTime)
        {
            bullets.ForEach(bullet =>
            {
                Vector3 p0 = GetPosition(bullet);
                bullet.Time += deltaTime;
                Vector3 p1 = GetPosition(bullet);
                RaycastSegment(p0, p1, bullet);
            });
        }

        private void DestroyBullets()
        {
            bullets.RemoveAll(bullet => bullet.Time >= _maxLifeTime);
        }

        private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
        {
            Vector3 direction = end - start;
            float distance = (direction).magnitude;
            _ray.origin = start;
            _ray.direction = direction;
            if (Physics.Raycast(_ray, out _hitInfo, distance))
            {
                HitEffect.transform.position = _hitInfo.point;
                HitEffect.transform.forward = _hitInfo.normal;
                HitEffect.Play();
                bullet.BulletTracerEffect.transform.position = _hitInfo.point;
                bullet.Time = _maxLifeTime;
            }
            else
                bullet.BulletTracerEffect.transform.position = end;
        }

        private void FireBullet()
        {
            MuzzleFlash.Play();
            Vector3 velocity = (RaycastDestination.position - RaycastOrigin.transform.position).normalized *
                               BulletSpeed;
            var bullet = CreateBullet(RaycastOrigin.transform.position, velocity);
            bullets.Add(bullet);
            // var position = RaycastOrigin.transform.position;
            // _ray.origin = position;
            // _ray.direction = RaycastDestination.position - position;
            //
            // TrailRenderer tracer = Instantiate(TracerEffect, _ray.origin, quaternion.identity);
            // tracer.AddPosition(_ray.origin);
            //
            // if (Physics.Raycast(_ray, out _hitInfo))
            // {
            //     HitEffect.transform.position = _hitInfo.point;
            //     HitEffect.transform.forward = _hitInfo.normal;
            //     HitEffect.Play();
            //     tracer.transform.position = _hitInfo.point;
            // }
        }

        public void StopFiring() =>
            IsFiring = false;
    }
}