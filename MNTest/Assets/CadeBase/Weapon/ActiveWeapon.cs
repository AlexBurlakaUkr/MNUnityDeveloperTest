using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;

namespace CadeBase.Weapon
{
    public class ActiveWeapon : MonoBehaviour
    {
        public Transform CrossHairTarget;
        public Rig HandIK;
        public Transform WeaponParent;
        public Transform WeaponLeftGrip;
        public Transform WeaponRightGrip;
        public Animator RigControllerAnimator;

        private RaycastWeapon _weapon;

        private void Start()
        {
            // RigControllerAnimator.Play("WeaponUnArm");
            HandIK.weight = 0;
            RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
            if (existingWeapon)
            {
                Equip(existingWeapon);
            }

        }

        private void Update()
        {
            if (_weapon)
            {
                if (Input.GetMouseButton(0))
                {
                    if (!_weapon.IsFiring)  _weapon.StartFiring();
                    _weapon.UpdateFiring(Time.deltaTime);
                }
                _weapon.UpdateBullet(Time.deltaTime);
                if (Input.GetMouseButtonUp(0))
                    _weapon.StopFiring();
            }
        }
        
        public void Equip(RaycastWeapon newWeapon)
        {
            if (_weapon) Destroy(_weapon.gameObject);
            _weapon = newWeapon;
            _weapon.RaycastDestination = CrossHairTarget;
            _weapon.transform.parent = WeaponParent;
            _weapon.transform.localPosition = Vector3.zero;
            _weapon.transform.localRotation = quaternion.identity;
            HandIK.weight = 1;
            RigControllerAnimator.Play("equip_" + _weapon.WeaponName);
        }
    }
}