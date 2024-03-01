using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { left, right };        //Naming has to be changed

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private GameObject leftWeaponParent;
    [SerializeField]
    private GameObject rightWeaponParent;

    private Weapon leftWeapon;
    private Weapon rightWeapon;

    private bool isLeftWeaponCool;
    private bool isRightWeaponCool;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        isLeftWeaponCool = false;
        isRightWeaponCool = false;
    }

    private void Update()
    {
        LookAtMouse(leftWeapon, rightWeapon);

        ClickFire();
    }

    private void LookAtMouse(params Weapon[] weapons)
    {
        foreach (var weapon in weapons)
        {
            if (weapon == null) return;

            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 weaponPos = weapon.transform.position;

            float dy = targetPos.y - weaponPos.y;
            float dx = targetPos.x - weaponPos.x;
            float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

            weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
        }
    }

    public void SwitchWeapon(WeaponType type, GameObject weaponObj)
    {
        if (type == WeaponType.left)
        {
            if (leftWeapon != null)
            {
                //Optimization: SetActive(false) and destroy when it's loading
                Destroy(leftWeapon.gameObject);
            }
            var obj = Instantiate(weaponObj, leftWeaponParent.transform);
            leftWeapon = obj.GetComponent<Weapon>();
            leftWeapon.Init();

            StopCoroutine(CheckCoolTime(WeaponType.left, leftWeapon.coolTime));
            isLeftWeaponCool = false;
        }
        else if (type == WeaponType.right)
        {
            if (rightWeapon != null)
            {
                Destroy(rightWeapon.gameObject);
            }
            var obj = Instantiate(weaponObj, rightWeaponParent.transform);
            rightWeapon = obj.GetComponent<Weapon>();
            rightWeapon.Init();

            StopCoroutine(CheckCoolTime(WeaponType.left, rightWeapon.coolTime));
            isRightWeaponCool = false;
        }
    }

    private void ClickFire()
    {
        if (Input.GetMouseButton(0))    //Left Click
        {
            if (leftWeapon == null || isLeftWeaponCool) return;

            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 myPos = transform.position;
            var dir = (targetPos - myPos).normalized;
            leftWeapon.Fire(dir);

            StartCoroutine(CheckCoolTime(WeaponType.left, leftWeapon.coolTime));
            isLeftWeaponCool = true;
        }
        //If right click -> rightWeapon.weapon.fire();
    }

    IEnumerator CheckCoolTime(WeaponType weaponType, float coolTime)
    {
        while (coolTime > 0f)
        {
            coolTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (weaponType == WeaponType.left) isLeftWeaponCool = false;
    }
}
