using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ObjectPooling))]
public abstract class Weapon : MonoBehaviour
{
    protected ObjectPooling objectPool;

    [Header("Weapon Settings")]
    [SerializeField]
    private WeaponName title;
    public WeaponName Title { get => title; }

    [SerializeField] private Vector2 handlePosition;

    public Vector2 HandlePosition { get => handlePosition; }

    [SerializeField] protected float damageValue;

    [SerializeField] protected float chargeDamageMultiplier;

    [SerializeField] protected float coreHitDamageMultiplier;

    [SerializeField] protected float bulletSpeed;

    [SerializeField] protected float weaponLength;

    public float WeaponLength { get => weaponLength; }

    [SerializeField] private int maxAmmo;
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

    [SerializeField] private float aimAccuracy;
    public float AimAccuracy { get => aimAccuracy;}

    [SerializeField] private float minChargeTime;
    [SerializeField] private float maxChargeTime;

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

    public event Action onAmmoChanged;

    public Action<float> onHit;             //Parameter : DamageAmount
    public Action<float> onCoreHit;
    public Action onKill;

    private void Awake()
    {
        objectPool = GetComponent<ObjectPooling>();
        curAmmo = maxAmmo;
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

        float resultDamageMultiplier = (chargeDamageMultiplier + chargeDamageMultiplierBonus) * Mathf.Pow(x, 2) + 1;
        
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

    protected void SetBullet(GameObject obj, Bullet bullet, Vector3 direction, float damageAmount)
    {
        obj.transform.position = transform.position + direction * weaponLength;
        obj.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;

        bullet.firedWeapon = this;
        bullet.FinalDamage = damageAmount;
        bullet.FinalDamageOnCoreHit = damageAmount * coreHitDamageMultiplier;
        bullet.LookAtDirection(obj, direction);
        bullet.AddTargetTag(Settings.enemyTag);
    }

    public void IncreaseDamageValue(float amount) => damageValue += amount;

    public void IncreaseCoreHitDamageMultiplier(float amount) => coreHitDamageMultiplier += amount;

    public void IncreaseMaxAmmo(int amount)
    {
        maxAmmo += amount;
        onAmmoChanged?.Invoke();
    }

    public void AddAmmo(int amount)
    {
        CurAmmo += amount;
        onAmmoChanged?.Invoke();
    }

    public void IncreaseAimAccuracy(float amount)
    {
        aimAccuracy += amount;

        if (aimAccuracy > 100f)
        {
            aimAccuracy = 100f;
        }
    }

    public void IncreaseBulletSpeed(float amount) => bulletSpeed += amount;
}
