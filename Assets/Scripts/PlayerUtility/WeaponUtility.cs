using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUtility : PlayerUtility
{
    [Header("Damage Value")]
    [SerializeField] private float damageValueBonusPercentage;
    [SerializeField] private float chargeDamageMultiplierBonus;
    [SerializeField] private float maxChargedDamageBonus;
    [SerializeField] private float coreHitDamageMultiplierBonus;

    [Header("Ammo")]
    [SerializeField] [Range(0f, 1f)] private float maxAmmoBonusPercentage; 
    [SerializeField] [Range(0f, 1f)] private float ammoBonusPercentage;

    [Header("Etc")]
    [SerializeField] private float aimAccuracyBonus;
    [SerializeField] [Range(0f, 1f)] private float bulletSpeedBonusPercentage;

    [Header("Defaultgun")]
    [SerializeField] private float bulletSizeBonus;

    [Header("MultiFiregun")]
    [SerializeField] private int pelletCountBonus;

    [Header("Explosivegun")]
    [SerializeField] private float explosionRadiusBonus;

    [Header("Lasergun")]
    [SerializeField] private float laserWidthBonus;
    [SerializeField] private float laserDurationBonus;

    [Header("On Hit")]
    [SerializeField] private bool canGetCoinOnKill;
    [SerializeField] private int coinBonusOnKill;

    [SerializeField] private bool canGetAmmoOnKill;
    [SerializeField] [Range(0f, 1f)] private float ammoBonusPercentageOnKill;

    [SerializeField] private bool canGetHealthOnCoreHit;
    [SerializeField] [Range(0f, 3f)] private float healthBonusPercentageOnCoreHit;

    [SerializeField] private bool canGetHealthOnKill;
    [SerializeField] private float healthBonusOnKill;

    private WeaponController weaponController;
    private Health health;
    private PlayerData playerData;

    private void Awake()
    {
        weaponController = playerTransform.GetComponent<WeaponController>();
        health = playerTransform.GetComponent<Health>();
        playerData = playerTransform.GetComponent<PlayerData>();
    }

    private void OnEnable()
    {
        weaponController.AddChargeDamageMultiplierBonus(chargeDamageMultiplierBonus);

        weaponController.AddMaxChargedDamageBonus(maxChargedDamageBonus);

        foreach (Weapon weapon in WeaponManager.Instance.PlayerWeaponList)
        {
            weapon.IncreaseDamageValue(damageValueBonusPercentage);

            weapon.IncreaseCoreHitDamageMultiplier(coreHitDamageMultiplierBonus);

            weapon.IncreaseMaxAmmo(Mathf.RoundToInt(maxAmmoBonusPercentage * weapon.MaxAmmo));

            weapon.AddAmmo(Mathf.RoundToInt(maxAmmoBonusPercentage * weapon.MaxAmmo));

            weapon.AddAmmo(Mathf.RoundToInt(ammoBonusPercentage * weapon.MaxAmmo));

            weapon.IncreaseAimAccuracy(aimAccuracyBonus);

            weapon.IncreaseBulletSpeed(bulletSpeedBonusPercentage);

            if (weapon is IDefaultgun)
            {
                (weapon as IDefaultgun).IncreaseBulletsize(bulletSizeBonus);
            }
            if (weapon is IMultiFiregun)
            {
                (weapon as IMultiFiregun).IncreasePelletCount(pelletCountBonus);
            }
            if (weapon is IExplosivegun)
            {
                (weapon as IExplosivegun).IncreaseExplosionRadius(explosionRadiusBonus);
            }
            if (weapon is ILasergun)
            {
                (weapon as ILasergun).IncreaseLaserWidth(laserWidthBonus);
                (weapon as ILasergun).IncreaseLaserDuration(laserDurationBonus);
            }

            if (canGetCoinOnKill)
            {
                weapon.onKill += GetCoin;
            }

            if (canGetAmmoOnKill)
            {
                weapon.onKill += AddAmmoOnKill;
                weapon.onKill += WeaponAmmoChanged;
            }

            if (canGetHealthOnCoreHit)
            {
                weapon.onCoreHit += IncreaseHealthOnCoreHit;
            }

            if (canGetHealthOnKill)
            {
                weapon.onKill += IncreaseHealthOnKill;
            }
        }

        WeaponAmmoChanged();
    }

    private void OnDisable()
    {
        weaponController.MinusChargeDamageMultiplierBonus(chargeDamageMultiplierBonus);

        weaponController.MinusMaxChargedDamageBonus(maxChargedDamageBonus);

        foreach (Weapon weapon in WeaponManager.Instance.PlayerWeaponList)
        {
            weapon.IncreaseDamageValue(-damageValueBonusPercentage);

            weapon.IncreaseCoreHitDamageMultiplier(-coreHitDamageMultiplierBonus);

            weapon.IncreaseMaxAmmo(Mathf.RoundToInt(-maxAmmoBonusPercentage * weapon.MaxAmmo));

            weapon.IncreaseAimAccuracy(-aimAccuracyBonus);

            weapon.IncreaseBulletSpeed(-bulletSpeedBonusPercentage);

            if (weapon is IDefaultgun)
            {
                (weapon as IDefaultgun).IncreaseBulletsize(-bulletSizeBonus);
            }
            if (weapon is IMultiFiregun)
            {
                (weapon as IMultiFiregun).IncreasePelletCount(-pelletCountBonus);
            }
            if (weapon is IExplosivegun)
            {
                (weapon as IExplosivegun).IncreaseExplosionRadius(-explosionRadiusBonus);
            }
            if (weapon is ILasergun)
            {
                (weapon as ILasergun).IncreaseLaserWidth(-laserWidthBonus);
                (weapon as ILasergun).IncreaseLaserDuration(-laserDurationBonus);
            }

            if (canGetCoinOnKill)
            {
                weapon.onKill -= GetCoin;
            }

            if (canGetAmmoOnKill)
            {
                weapon.onKill -= AddAmmoOnKill;
                weapon.onKill -= WeaponAmmoChanged;
            }

            if (canGetHealthOnCoreHit)
            {
                weapon.onCoreHit -= IncreaseHealthOnCoreHit;
            }

            if (canGetHealthOnKill)
            {
                weapon.onKill -= IncreaseHealthOnKill;
            }
        }

        WeaponAmmoChanged();
    }

    private void WeaponAmmoChanged()
    {
        weaponController.onWeaponAmmoChanged?.Invoke(WeaponHand.Left, weaponController.Weapons[0]);
        weaponController.onWeaponAmmoChanged?.Invoke(WeaponHand.Right, weaponController.Weapons[1]);
    }

    private void GetCoin()
    {
        playerData.GetCoin(coinBonusOnKill);
    }

    private void AddAmmoOnKill()
    {
        foreach (Weapon weapon in WeaponManager.Instance.PlayerWeaponList)
        {
            weapon.AddAmmo(Mathf.RoundToInt(ammoBonusPercentageOnKill * weapon.MaxAmmo));
        }
    }

    private void IncreaseHealthOnCoreHit(float damageAmount)
    {
        health.HealByAmount(damageAmount * healthBonusPercentageOnCoreHit);
    }

    private void IncreaseHealthOnKill()
    {
        health.HealByAmount(healthBonusOnKill);
    }
}
