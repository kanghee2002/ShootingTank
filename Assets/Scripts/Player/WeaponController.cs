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

    private SpriteRenderer[] weaponSpriteRenderers = new SpriteRenderer[2];

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        SwitchWeapon(WeaponHand.Left, isFront: true);
        SwitchWeapon(WeaponHand.Right, isFront: true);
    }

    private void OnEnable()
    {
        int weaponHand = (int)WeaponHand.Right;
        if (!WeaponManager.Instance.IsRightWeaponEnabled)
        {
            weaponParents[weaponHand].SetActive(false);
        }
        else
        {
            weaponParents[weaponHand].SetActive(true);
        }
    }

    private void Update()
    {
        LookAtMouse(WeaponHand.Left);
        ClickFire(WeaponHand.Left);
        CheckSwitch(WeaponHand.Left);

        if (WeaponManager.Instance.IsRightWeaponEnabled)
        {
            LookAtMouse(WeaponHand.Right);
            ClickFire(WeaponHand.Right);
            CheckSwitch(WeaponHand.Right);
        }
    }

    private void LookAtMouse(WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        Weapon weapon = weapons[weaponHandIdx];

        if (!weapon)
        {
            return;
        }

        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 weaponPos = weapon.transform.position;

        float dy = targetPos.y - weaponPos.y;
        float dx = targetPos.x - weaponPos.x;
        float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        weapon.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);

        if (Mathf.Abs(rotateDegree) > 90f)
        {
            weaponSpriteRenderers[weaponHandIdx].flipY = true;
        }
        else
        {
            weaponSpriteRenderers[weaponHandIdx].flipY = false;
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
        weaponSpriteRenderers[weaponHandIdx] = obj.GetComponent<SpriteRenderer>();

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
