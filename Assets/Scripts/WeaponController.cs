using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { left, right };

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private GameObject leftWeaponParent;
    [SerializeField]
    private GameObject rightWeaponParent;

    private Weapon leftWeapon;
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


    public void SwitchWeapon(WeaponType type, GameObject weaponObj)
    {
        if (type == WeaponType.left)
        {
            var obj = Instantiate(weaponObj, leftWeaponParent.transform);
            leftWeaponParent = obj;
            leftWeapon = leftWeaponParent.GetComponent<Weapon>();
        }
        else if (type == WeaponType.right)
        {
            var obj = Instantiate(weaponObj, rightWeaponParent.transform);
            rightWeaponParent = obj;
            rightWeapon = rightWeaponParent.GetComponent<Weapon>();
        }
    }

    private void ClickFire()
    {
        if (Input.GetMouseButtonDown(0))    //Left Click
        {
            if (leftWeapon == null) return;

            leftWeapon.Fire(Camera.main.ScreenToWorldPoint(Input.mousePosition).normalized);
        }
        //If right click -> rightWeapon.weapon.fire();
    }
}
