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
            weaponController.SwitchWeapon(WeaponHand.left, defaultWeapon);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            weaponController.SwitchWeapon(WeaponHand.right, defaultWeapon);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            weaponController.SwitchWeapon(WeaponHand.left, testWeapon);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            weaponController.SwitchWeapon(WeaponHand.right, testWeapon);
        }
    }

}
