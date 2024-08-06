using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform[] weaponHandlePositions;
    [SerializeField] private LayerMask blockingLayerBetweenWeapon;

    private Weapon[] weapons = new Weapon[2];
    public Weapon[] Weapons { get => weapons; }

    private SpriteRenderer[] weaponSpriteRenderers = new SpriteRenderer[2];

    public delegate void OnWeaponChanged(WeaponHand weaponHand, Weapon weapon);
    public event OnWeaponChanged onWeaponChanged;

    public delegate void OnWeaponCharged(WeaponHand weaponHand, Weapon weapon);
    public event OnWeaponCharged onWeaponCharged;

    public delegate void OnWeaponShoot(WeaponHand weaponHand, Weapon weapon);
    public event OnWeaponShoot onWeaponShoot;

    public Action<WeaponHand, Weapon> onWeaponAmmoChanged;

    private float chargeDamageMultiplierBonus;
    private float maxChargedDamageBonus;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        SwitchWeapon(WeaponHand.Left, isFront: true);
        SwitchWeapon(WeaponHand.Right, isFront: true);

        chargeDamageMultiplierBonus = 0f;
        maxChargedDamageBonus = 0f;
    }

    private void OnEnable()
    {
        int weaponHand = (int)WeaponHand.Right;
        if (!WeaponManager.Instance.IsRightWeaponEnabled)
        {
            weaponHandlePositions[weaponHand].gameObject.SetActive(false);
        }
        else
        {
            weaponHandlePositions[weaponHand].gameObject.SetActive(true);
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

        onWeaponCharged?.Invoke(WeaponHand.Left, weapons[0]);
        onWeaponCharged?.Invoke(WeaponHand.Right, weapons[1]);
    }

    private void LookAtMouse(WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        Weapon weapon = weapons[weaponHandIdx];

        if (!weapon)
        {
            return;
        }

        Vector3 targetPos = Input.mousePosition;
            
        targetPos.x = Mathf.Clamp(targetPos.x, 0f, Screen.width);
        targetPos.y = Mathf.Clamp(targetPos.y, 0f, Screen.height);

        targetPos = Camera.main.ScreenToWorldPoint(targetPos);


        Vector3 weaponPos = weaponHandlePositions[weaponHandIdx].position;

        float dy = targetPos.y - weaponPos.y;
        float dx = targetPos.x - weaponPos.x;
        float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        weaponHandlePositions[weaponHandIdx].eulerAngles = new Vector3(0f, 0f, rotateDegree);

        if (Mathf.Abs(rotateDegree) > 90f)
        {
            Vector3 pointLocalScale = weaponHandlePositions[weaponHandIdx].localScale;
            weaponHandlePositions[weaponHandIdx].localScale = new Vector3(pointLocalScale.x, -1f, 1f);
        }
        else
        {
            //weaponSpriteRenderers[weaponHandIdx].flipY = false;
            Vector3 pointLocalScale = weaponHandlePositions[weaponHandIdx].localScale;
            weaponHandlePositions[weaponHandIdx].localScale = new Vector3(pointLocalScale.x, 1f, 1f);
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
        Weapon weapon = WeaponManager.Instance.GetWeapon(isFront);
        weapon.transform.SetParent(weaponHandlePositions[weaponHandIdx]);
        weapon.transform.localPosition = weapon.HandlePosition;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = new Vector3(weapon.transform.localScale.x, Mathf.Abs(weapon.transform.localScale.y), 1f);
        weaponSpriteRenderers[weaponHandIdx] = weapon.GetComponent<SpriteRenderer>();

        if (weapons[weaponHandIdx])
        {
            WeaponManager.Instance.ReturnWeapon
                (weapons[weaponHandIdx], isFront);
        }
        weapons[weaponHandIdx] = weapon;

        onWeaponChanged?.Invoke(weaponHand, weapon); ;
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
            
            Vector2 direction = (targetPos - myPos).normalized;

            if (isBlockingBetweenWeapon(direction, weapon.WeaponLength))
            {
                //weapon.CurAmmo--;
                //weapon.Recharge();
                //onWeaponShoot?.Invoke(weaponHand, weapon);
                return;
            }

            weapon.Fire(direction, chargeDamageMultiplierBonus, maxChargedDamageBonus);

            onWeaponShoot?.Invoke(weaponHand, weapon);
        }
    }

    private bool isBlockingBetweenWeapon(Vector2 direction, float distance)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, direction, distance, blockingLayerBetweenWeapon);

        if (rayHit)
            return true;
        else 
            return false;
    }

    public void AddChargeDamageMultiplierBonus(float amount) => chargeDamageMultiplierBonus += amount;
    public void MinusChargeDamageMultiplierBonus(float amount) => chargeDamageMultiplierBonus -= amount;

    public void AddMaxChargedDamageBonus(float amount) => maxChargedDamageBonus += amount;
    public void MinusMaxChargedDamageBonus(float amount) => maxChargedDamageBonus -= amount;

}
