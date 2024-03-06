using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeaponChanger : MonoBehaviour
{
    public WeaponController weaponController;

    private void Start()
    {
        WeaponManager.Instance.AddAvailableWeapon(WeaponName.DefaultWeapon);
        WeaponManager.Instance.AddAvailableWeapon(WeaponName.TestWeapon);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            weaponController.SwitchWeapon(WeaponHand.left, WeaponName.DefaultWeapon);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            weaponController.SwitchWeapon(WeaponHand.right, WeaponName.DefaultWeapon);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            weaponController.SwitchWeapon(WeaponHand.left, WeaponName.TestWeapon);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            weaponController.SwitchWeapon(WeaponHand.right, WeaponName.TestWeapon);
        }
    }

}
