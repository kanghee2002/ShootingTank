using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeaponChanger : MonoBehaviour
{
    public WeaponController weaponController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //weaponController.SwitchWeapon(WeaponHand.Left, WeaponName.DefaultWeapon);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            //weaponController.SwitchWeapon(WeaponHand.Right, WeaponName.DefaultWeapon);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            //weaponController.SwitchWeapon(WeaponHand.Left, WeaponName.TestWeapon);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            //weaponController.SwitchWeapon(WeaponHand.Right, WeaponName.TestWeapon);
        }
    }

}
