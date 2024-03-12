using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ShootingObject
{
    [SerializeField]
    private WeaponName title;
    public WeaponName Title { get => title; private set => title = value; }
}
