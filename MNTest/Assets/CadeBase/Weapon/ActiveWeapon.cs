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
        
        private RaycastWeapon _weapon;
        private Animator _animator;
        private AnimatorOverrideController _overrideController;
        

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _overrideController = _animator.runtimeAnimatorController as AnimatorOverrideController;
            
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
            else
            {
                HandIK.weight = 0.0f;
                _animator.SetLayerWeight(1,0);
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
            HandIK.weight = 1f;
            _animator.SetLayerWeight(1,1);
            Invoke(nameof(SetAnimationDelayed),0.001f) ;
        }

        private void SetAnimationDelayed()
        {
            _overrideController["WeaponEmpty"] = _weapon.WeaponAnimation;

        }

        [ContextMenu("SaveWeaponPose")]
        private void SaveWeaponPose()
        {
            GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
            recorder.BindComponentsOfType<Transform>(WeaponParent.gameObject, false);
            recorder.BindComponentsOfType<Transform>(WeaponLeftGrip.gameObject, false);
            recorder.BindComponentsOfType<Transform>(WeaponRightGrip.gameObject, false);
            recorder.TakeSnapshot(0);
            recorder.SaveToClip(_weapon.WeaponAnimation);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
}