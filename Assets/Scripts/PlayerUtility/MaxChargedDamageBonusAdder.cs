using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxChargedDamageBonusAdder : PlayerUtility
{
    private WeaponController weaponController;

    [SerializeField] private float maxChargedDamageBonus;

    private void Awake()
    {
        weaponController = GetComponent<WeaponController>();
    }

    private void OnEnable()
    {
        weaponController.AddMaxChargedDamageBonus(maxChargedDamageBonus);
    }

    private void OnDisable()
    {
        weaponController.MinusMaxChargedDamageBonus(maxChargedDamageBonus);
    }
}
