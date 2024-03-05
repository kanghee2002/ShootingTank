using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeaponChanger : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            WeaponManager.Instance.AddAvailableWeapon(WeaponName.DefaultWeapon);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            WeaponManager.Instance.AddAvailableWeapon(WeaponName.TestWeapon);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            WeaponManager.Instance.AddAvailableWeapon(WeaponName.DefaultWeapon);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            WeaponManager.Instance.AddAvailableWeapon(WeaponName.TestWeapon);
        }
    }

}
