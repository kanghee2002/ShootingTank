using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxAmmoAdder
{
    public int maxAmmoBonus;

    public MaxAmmoAdder(int maxAmmoBonus)
    {
        this.maxAmmoBonus = maxAmmoBonus;
        foreach (var weapon in WeaponManager.Instance.GetAvailableWeaponList())
        {
            weapon.AddMaxAmmo(maxAmmoBonus);
        }
    }
}
