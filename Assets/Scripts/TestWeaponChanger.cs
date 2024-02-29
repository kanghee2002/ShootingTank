using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeaponChanger : MonoBehaviour
{
    public WeaponController weaponController;
    public GameObject defaultWeapon;

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
    }

}
