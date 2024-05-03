using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName { Default, Rifle, Shotgun, Missilegun };

public abstract class Weapon : ShootingObject
{
    [Header("Weapon Settings")]
    [SerializeField]
    private WeaponName title;
    public WeaponName Title { get => title; }

    [SerializeField]
    private int maxAmmo;
    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }

    [SerializeField]
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

    private void OnEnable()
    {
        chargedTime = minChargeTime;
        if (curChargeCoroutine != null) StopCoroutine(curChargeCoroutine);
        curChargeCoroutine = IncreaseChargedTime();
        StartCoroutine(curChargeCoroutine);
    }

    protected void Recharge()
    {
        chargedTime = 0f;
        if (curChargeCoroutine != null) StopCoroutine(curChargeCoroutine);
        curChargeCoroutine = IncreaseChargedTime();
        StartCoroutine(curChargeCoroutine);
    }

    //maxChargeTime < minChargeTime * damageMultiplier 
    protected virtual float GetDamageMultiplier(float percentage)
        => 0.5f * Mathf.Pow(percentage, 2) + percentage + 1;

    protected virtual IEnumerator IncreaseChargedTime()
    {
        while (chargedTime < maxChargeTime)
        {
            chargedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    protected virtual Vector3 GetRandomDir(Vector3 dir, float aimAccuracy)
    {
        float accuracyRadian = (90 - (aimAccuracy * 9 / 10)) * Mathf.Deg2Rad;
        float randomAngle = Random.Range(-accuracyRadian, accuracyRadian);
        Vector3 randomDir = new Vector3(
                dir.x * Mathf.Cos(randomAngle) - dir.y * Mathf.Sin(randomAngle),
                dir.x * Mathf.Sin(randomAngle) + dir.y * Mathf.Cos(randomAngle),
                0);
        return randomDir;
    }
}
