using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeaponChanger : MonoBehaviour
{
    public WeaponController weaponController;
    public GameObject defaultWeapon;
    public GameObject testWeapon;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            weaponController.SwitchWeapon(WeaponType.left, defaultWeapon);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            weaponController.SwitchWeapon(WeaponType.right, defaultWeapon);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            weaponController.SwitchWeapon(WeaponType.left, testWeapon);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            weaponController.SwitchWeapon(WeaponType.right, testWeapon);
        }
    }

}
