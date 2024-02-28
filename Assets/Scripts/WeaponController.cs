using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private Weapon leftWeapon;
    [SerializeField]
    private Weapon rightWeapon;

    private void Update()
    {
        LookAtMouse(leftWeapon);
        LookAtMouse(rightWeapon);

        ClickFire();
    }


    private void LookAtMouse(Weapon weapon)
    {
        if (weapon == null) return;

        Vector3 targetPostion = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 weaponPosition = weapon.transform.position;

        float dy = targetPostion.y - weaponPosition.y;
        float dx = targetPostion.x - weaponPosition.x;
        float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
    }

    public void SwitchLeftWeapon(Weapon weapon) => leftWeapon = weapon;

    public void SwitchRightWeapon(Weapon weapon) => rightWeapon = weapon;

    private void ClickFire()
    {
        if (Input.GetMouseButtonDown(0))    //Left Click
        {
            leftWeapon.Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        //If left click -> leftWeapon.weapon.fire();
        //If right click -> rightWeapon.weapon.fire();
    }
}
