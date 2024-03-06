using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponHand { left = 0, right = 1 };

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] weaponParents = new GameObject[2];

    private Weapon[] weapons = new Weapon[2];

    private bool[] isWeaponCool = new bool[2]; 

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < isWeaponCool.Length; i++)
        {
            isWeaponCool[i] = false;
        }
    }

    private void Update()
    {
        LookAtMouse(weapons);

        ClickFire(WeaponHand.left);
        ClickFire(WeaponHand.right);
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

    public void SwitchWeapon(WeaponHand weaponHand, WeaponName weaponName)
    {
        int weaponIdx = (int)weaponHand;

        if (weapons[weaponIdx] != null)
        {
            WeaponManager.Instance.ReturnWeapon(weapons[weaponIdx].gameObject, weapons[weaponIdx]);
        }
        var (obj, weapon) = WeaponManager.Instance.GetWeapon(weaponName);
        obj.transform.SetParent(weaponParents[weaponIdx].transform);
        obj.transform.localPosition = Vector3.zero;
        weapons[weaponIdx] = weapon;

    }

    private void ClickFire(WeaponHand weaponHand)
    {
        int weaponIdx = (int)weaponHand;

        if (Input.GetMouseButton(weaponIdx))
        {
            if (weapons[weaponIdx] == null || isWeaponCool[weaponIdx]) return;

            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 myPos = transform.position;
            var dir = (targetPos - myPos).normalized;
            weapons[weaponIdx].Fire(dir);

            StartCoroutine(CheckCoolTime(weaponHand, weapons[weaponIdx].coolTime));
            isWeaponCool[weaponIdx] = true;
        }
    }

    IEnumerator CheckCoolTime(WeaponHand weaponHand, float coolTime)
    {
        int weaponIdx = (int)weaponHand;

        while (coolTime > 0f)
        {
            coolTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        isWeaponCool[weaponIdx] = false;
    }
}
