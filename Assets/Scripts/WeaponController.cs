using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private GameObject leftWeapon;
    [SerializeField]
    private GameObject rightWeapon;

    private void Update()
    {
        LookAtMouse(leftWeapon);
        LookAtMouse(rightWeapon);
    }


    private void LookAtMouse(GameObject weapon)
    {
        Vector3 targetPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 weaponPosition = weapon.transform.position;

        float dy = targetPostion.y - weaponPosition.y;
        float dx = targetPostion.x - weaponPosition.x;
        float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
    }

    public void SwitchWeapon1(GameObject weapon)
    {
        //leftWeapon.GetComponet<Weapon>() ,,,
    }

    public void SwitchWeapon2(GameObject weapon)
    {
        //rightWeapon.GetComponet<Weapon>() ,,,
    }

    private void Fire()
    {
        //If left click -> leftWeapon.weapon.fire();
        //If right click -> rightWeapon.weapon.fire();
    }
}
