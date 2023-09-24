using System;
using UnityEngine;

namespace CadeBase.Weapon
{
    public class WeaponPickUp : MonoBehaviour
    {
        public RaycastWeapon WeaponFab;

        private void OnTriggerEnter(Collider other)
        {
            ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();
            if (activeWeapon)
            {
                RaycastWeapon newWeapon = Instantiate(WeaponFab);
                activeWeapon.Equip(newWeapon);
            }
        }
    }
}