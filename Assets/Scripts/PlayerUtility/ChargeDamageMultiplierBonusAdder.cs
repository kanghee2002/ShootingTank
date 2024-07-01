using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponController))]
public class ChargeDamageMultiplierBonusAdder : MonoBehaviour
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
