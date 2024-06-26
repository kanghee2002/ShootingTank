﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName 
{ 
    Default,
    Rifle,
    Shotgun,
    Missilegun,
    GrenadeLauncher,
    Lasergun,
    Burstgun,
    LaserShotgun
};

[RequireComponent(typeof(ObjectPooling))]
public abstract class Weapon : MonoBehaviour
{
    protected ObjectPooling objectPool;

    [Header("Weapon Settings")]
    [SerializeField]
    private WeaponName title;
    public WeaponName Title { get => title; }

    [SerializeField] protected float damageValue;

    [SerializeField] protected float ChargeDamageMultiplier;

    [SerializeField] protected float bulletSpeed;

    [SerializeField] protected float weaponLength;

    [SerializeField]
    private int maxAmmo;
    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }

    private int curAmmo;
    public int CurAmmo { get => curAmmo; 
        set 
        {
            if (value < 0) curAmmo = 0;
            else if (value > maxAmmo) curAmmo = maxAmmo;
            else curAmmo = value;
        } 
    }

    [SerializeField]
    private float aimAccuracy;
    public float AimAccuracy { get => aimAccuracy; set => aimAccuracy = value; }

    [SerializeField]
    private float minChargeTime;
    [SerializeField]
    private float maxChargeTime;

    private float chargedTime;

    private IEnumerator curChargeCoroutine;

    public bool isCharged { get => chargedTime >= minChargeTime; }

    /// <summary>
    /// 0 &lt;= percentage &lt;= 1<br />
    /// </summary>
    public float ChargePercentage
    {
        get
        {
            float percentage = (chargedTime - minChargeTime) / (maxChargeTime - minChargeTime);
            if (percentage < 0f) return 0f;
            else if (percentage > 1f) return 1f;
            else return percentage;
        }
    }

    private void Awake()
    {
        objectPool = GetComponent<ObjectPooling>();
        curAmmo = maxAmmo;
        ChargeDamageMultiplier = 1.5f;
    }

    private void OnEnable()
    {
        Recharge();
    }

    protected void Recharge()
    {
        chargedTime = 0f;
        if (curChargeCoroutine != null) StopCoroutine(curChargeCoroutine);
        curChargeCoroutine = IncreaseChargedTime();
        StartCoroutine(curChargeCoroutine);
    }

    //maxChargeTime < minChargeTime * damageMultiplier 
    protected virtual float GetDamageMultiplier(float percentage, 
        float chargeDamageMultiplierBonus, float maxChargedDamageBonus)
    {
        float x = percentage;

        float resultDamageMultiplier = (ChargeDamageMultiplier + chargeDamageMultiplierBonus) * Mathf.Pow(x, 2) + 1;
        
        if (percentage >= 1f)
        {
            resultDamageMultiplier += maxChargedDamageBonus;
        }

        return resultDamageMultiplier;
    }

    protected virtual IEnumerator IncreaseChargedTime()
    {
        while (chargedTime < maxChargeTime)
        {
            chargedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    protected Vector3 GetDirectionByAngle(Vector3 dir, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        Vector3 newDir = new Vector3(
                dir.x * Mathf.Cos(radian) - dir.y * Mathf.Sin(radian),
                dir.x * Mathf.Sin(radian) + dir.y * Mathf.Cos(radian),
                0);
        return newDir;
    }

    protected Vector3 GetRandomDirection(Vector3 dir, float aimAccuracy)
    {
        float accuracyRadian = (90 - (aimAccuracy * 9 / 10)) * Mathf.Deg2Rad;
        float randomRadian = Random.Range(-accuracyRadian, accuracyRadian);
        Vector3 randomDir = new Vector3(
                dir.x * Mathf.Cos(randomRadian) - dir.y * Mathf.Sin(randomRadian),
                dir.x * Mathf.Sin(randomRadian) + dir.y * Mathf.Cos(randomRadian),
                0);
        return randomDir;
    }

    public abstract void Fire(Vector3 direction, float chargeDamageMultiplierBonus,
        float maxChargedDamageBonus);

    public void AddDamageValue(float amount) => damageValue += amount;

    public void AddMaxAmmo(int amount) => maxAmmo += amount;
}
