using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField]
    private List<GameObject> weapons = new List<GameObject>();
    private List<(GameObject obj, Weapon weapon)> availableWeaponList = new List<(GameObject, Weapon)>();

    [SerializeField]
    private Transform weaponsParent;

    [SerializeField]
    private bool isRightWeaponEnabled;
    public bool IsRightWeaponEnabled { get => isRightWeaponEnabled; set => isRightWeaponEnabled = value; }

    public int availableWeaponNum { get { return availableWeaponList.Count; } 
                                    private set { availableWeaponNum = availableWeaponList.Count; } }

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    private void Init()
    {
        AddAvailableWeapon(WeaponName.Default);
        AddAvailableWeapon(WeaponName.Default);
        AddAvailableWeapon(WeaponName.Rifle);
        AddAvailableWeapon(WeaponName.Shotgun);
        AddAvailableWeapon(WeaponName.Missilegun);

        isRightWeaponEnabled = false;
    }

    public (GameObject, Weapon) InitWeapon(GameObject weaponObj)      //Act at First
    {
        var obj = Instantiate(weaponObj, weaponsParent);
        var weapon = obj.GetComponent<Weapon>();
        weapon.Init();
        return (obj, weapon);
    }

    public void AddAvailableWeapon(WeaponName weaponName)
    {
        if (weaponName != WeaponName.Default)
        {
            foreach (var tuple in availableWeaponList)
            {
                if (tuple.weapon.Title == weaponName)
                {
                    return;
                }
            }
        }

        foreach (var obj in weapons)
        {
            var weapon = obj.GetComponent<Weapon>();
            if (weapon.Title == weaponName)
            {
                var result = InitWeapon(obj);
                availableWeaponList.Add(result);
                return;
            }
        }
    }

    public (GameObject, Weapon) GetWeapon(bool isFront)
    {
        if (isFront)
        {
            var tuple = availableWeaponList[0];
            availableWeaponList.RemoveAt(0);
            return tuple;
        }
        else
        {
            var tuple = availableWeaponList[availableWeaponList.Count - 1];
            availableWeaponList.RemoveAt(availableWeaponList.Count - 1);
            return tuple;
        }
    }

    public void ReturnWeapon(GameObject obj, Weapon weapon, bool isFront)
    {
        if (!weapon)
        {
            availableWeaponList.Add((obj, obj.GetComponent<Weapon>()));
        }
        else
        {
            if (isFront) availableWeaponList.Add((obj, weapon));
            else availableWeaponList.Insert(0, (obj, weapon));
        }
        obj.transform.SetParent(weaponsParent);
    }
}
