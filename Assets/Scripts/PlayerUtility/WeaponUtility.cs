using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUtility : PlayerUtility
{
    [Header("Damage Value")]
    [SerializeField] private float damageValueBonus;
    [SerializeField] private float chargeDamageMultiplierBonus;
    [SerializeField] private float maxChargedDamageBonus;
    [SerializeField] private float coreHitDamageMultiplierBonus;

    [Header("Ammo")]
    [SerializeField] private int maxAmmoBonus;
    [SerializeField] private int ammoBonus;

    [Header("Etc")]
    [SerializeField] private float aimAccuracyBonus;
    [SerializeField] private float bulletSpeedBonus;

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
    [SerializeField] private int ammoBonusOnKill;

    [SerializeField] private bool canGetHealthOnCoreHit;
    [SerializeField] private float healthBonusOnCoreHit;        // Multiplier

    [SerializeField] private bool canGetHealthOnKill;
    [SerializeField] private float healthBonusOnKill;

    private WeaponController weaponController;
    private Health health;

    private void Awake()
    {
        weaponController = playerTransform.GetComponent<WeaponController>();
        health = playerTransform.GetComponent<Health>();
    }

    private void OnEnable()
    {
        weaponController.AddChargeDamageMultiplierBonus(chargeDamageMultiplierBonus);

        weaponController.AddMaxChargedDamageBonus(maxChargedDamageBonus);

        foreach (Weapon weapon in WeaponManager.Instance.PlayerWeaponList)
        {
            weapon.IncreaseDamageValue(damageValueBonus);

            weapon.IncreaseCoreHitDamageMultiplier(coreHitDamageMultiplierBonus);

            weapon.IncreaseMaxAmmo(maxAmmoBonus);

            weapon.AddAmmo(ammoBonus);

            weapon.IncreaseAimAccuracy(aimAccuracyBonus);

            weapon.IncreaseBulletSpeed(bulletSpeedBonus);

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
            weapon.IncreaseDamageValue(-damageValueBonus);

            weapon.IncreaseCoreHitDamageMultiplier(-coreHitDamageMultiplierBonus);

            weapon.IncreaseMaxAmmo(-maxAmmoBonus);

            weapon.IncreaseAimAccuracy(-aimAccuracyBonus);

            weapon.IncreaseBulletSpeed(bulletSpeedBonus);

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

    }

    private void AddAmmoOnKill()
    {
        foreach (Weapon weapon in WeaponManager.Instance.PlayerWeaponList)
        {
            weapon.AddAmmo(ammoBonusOnKill);
        }
    }

    private void IncreaseHealthOnCoreHit(float damageAmount)
    {
        health.IncreaseHealth(damageAmount * healthBonusOnCoreHit);
    }

    private void IncreaseHealthOnKill()
    {
        health.IncreaseHealth(healthBonusOnKill);
    }
}
