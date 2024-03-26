using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ShootingObject
{
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
        Recharge();
    }

    public override GameObject Fire(Vector3 dir)
    {
        if (curAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return null;
        }

        var obj = base.Fire(dir);
        obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);
        CurAmmo--;
        Recharge();
        return obj;
    }

    protected void Recharge()
    {
        chargedTime = 0f;
        if (curChargeCoroutine != null) StopCoroutine(curChargeCoroutine);
        curChargeCoroutine = IncreaseChargedTime();
        StartCoroutine(curChargeCoroutine);
    }

    protected float GetDamageMultiplier(float percentage)
        => 0.5f * Mathf.Pow(percentage, 2) + percentage + 1;

    protected virtual IEnumerator IncreaseChargedTime()
    {
        while (chargedTime < maxChargeTime)
        {
            chargedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
