using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponHand { Left = 0, Right = 1 };

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] weaponParents = new GameObject[2];

    private Weapon[] weapons = new Weapon[2];

    public Weapon[] Weapons { get => weapons; }

    private void Start()
    {
        Init();
    }

    private void Init()
    { 
        SwitchWeapon(WeaponHand.Left, isFront: true);
        SwitchWeapon(WeaponHand.Right, isFront: true);
    }

    private void Update()
    {
        LookAtMouse(weapons);

        ClickFire(WeaponHand.Left);
        ClickFire(WeaponHand.Right);

        CheckSwitch(WeaponHand.Left);
        CheckSwitch(WeaponHand.Right);
    }

    private void LookAtMouse(params Weapon[] weapons)
    {
        foreach (var weapon in weapons)
        {
            if (!weapon) return;

            Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 weaponPos = weapon.transform.position;

            float dy = targetPos.y - weaponPos.y;
            float dx = targetPos.x - weaponPos.x;
            float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

            weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
        }
    }

    private void CheckSwitch(WeaponHand weaponHand)
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if ((weaponHand == WeaponHand.Left && !Input.GetKey(KeyCode.LeftShift)) ||
            (weaponHand == WeaponHand.Right && Input.GetKey(KeyCode.LeftShift)))
        {
            if (wheelInput > 0f)
            {
                SwitchWeapon(weaponHand, isFront: false);
            }
            else if (wheelInput < 0f)
            {
                SwitchWeapon(weaponHand, isFront: true);
            }
        }
    }

    public void SwitchWeapon(WeaponHand weaponHand, bool isFront)
    {
        if (WeaponManager.Instance.availableWeaponNum == 0)
        {
            Debug.Log("Error: No Weapon in WeaponManager");
            return;
        }

        int weaponHandIdx = (int)weaponHand;
        var (obj, weapon) = WeaponManager.Instance.GetWeapon(isFront);
        obj.transform.SetParent(weaponParents[weaponHandIdx].transform);
        obj.transform.localPosition = Vector3.zero;

        if (weapons[weaponHandIdx])
        {
            WeaponManager.Instance.ReturnWeapon
                (weapons[weaponHandIdx].gameObject, weapons[weaponHandIdx], isFront);
        }
        weapons[weaponHandIdx] = weapon;

    }

    private void ClickFire(WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        Weapon weapon = weapons[weaponHandIdx];

        if (Input.GetMouseButton(weaponHandIdx))
        {
            if (!weapon) return;
            if (!weapon.isCharged) return;

            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 myPos = transform.position;
            var dir = (targetPos - myPos).normalized;
            weapon.Fire(dir);
        }
    }
}
