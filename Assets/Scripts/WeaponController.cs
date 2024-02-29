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

        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 weaponPos = weapon.transform.position;

        float dy = targetPos.y - weaponPos.y;
        float dx = targetPos.x - weaponPos.x;
        float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
    }

    public void SwitchWeapon(WeaponType type, GameObject weaponObj)
    {
        if (type == WeaponType.left)
        {
            if (leftWeapon != null)
            {
                //Optimization: make disable and destroy when it's loading
                Destroy(leftWeapon.gameObject);
            }
            var obj = Instantiate(weaponObj, leftWeaponParent.transform);
            leftWeapon = obj.GetComponent<Weapon>();
        }
        else if (type == WeaponType.right)
        {
            if (rightWeapon != null)
            {
                Destroy(rightWeapon.gameObject);
            }
            var obj = Instantiate(weaponObj, rightWeaponParent.transform);
            rightWeapon = obj.GetComponent<Weapon>();
        }
    }

    private void ClickFire()
    {
        if (Input.GetMouseButtonDown(0))    //Left Click
        {
            if (leftWeapon == null) return;

            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 myPos = transform.position;
            var dir = (targetPos - myPos).normalized;
            leftWeapon.Fire(dir);
        }
        //If right click -> rightWeapon.weapon.fire();
    }
}
