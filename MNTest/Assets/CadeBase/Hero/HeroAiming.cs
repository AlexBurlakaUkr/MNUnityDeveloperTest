using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace CadeBase.Hero
{
    public class HeroAiming : MonoBehaviour
    {
        public float TurnSpeed = 15;
        public float AimDuration = 0.3f;
        public Rig AimLayer;

        private Camera _mainCamera;

        private void Start() => 
            _mainCamera = Camera.main;

        private void Update()
        {
            if (Input.GetMouseButton(1))
                AimLayer.weight += Time.deltaTime / AimDuration;
            else
                AimLayer.weight -= Time.deltaTime / AimDuration;
        }

        private void FixedUpdate()
        {
            float yawCamera = _mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, yawCamera,0)
                , TurnSpeed * Time.fixedDeltaTime);
        }
    }
}