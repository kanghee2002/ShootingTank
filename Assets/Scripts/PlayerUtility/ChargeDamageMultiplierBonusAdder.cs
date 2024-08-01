using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeDamageMultiplierBonusAdder : PlayerUtility
{
    private WeaponController weaponController;

    [SerializeField] private float chargeDamageMultiplierBonus;

    private void Awake()
    {
        weaponController = GetComponent<WeaponController>();
    }

    private void OnEnable()
    {
        weaponController.AddChargeDamageMultiplierBonus(chargeDamageMultiplierBonus);
    }

    private void OnDisable()
    {
        weaponController.MinusChargeDamageMultiplierBonus(chargeDamageMultiplierBonus);
    }

}
