using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName { DefaultWeapon, TestWeapon };

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField]
    private List<GameObject> weapons = new List<GameObject>();
    private List<(GameObject obj, Weapon weapon)> availableWeaponList = new List<(GameObject, Weapon)>();

    [SerializeField]
    private Transform weaponsParent;

    public (GameObject, Weapon) InitWeapon(GameObject weaponObj)      //Act at First
    {
        var obj = Instantiate(weaponObj, weaponsParent);
        var weapon = obj.GetComponent<Weapon>();
        weapon.Init();
        return (obj, weapon);
    }

    public void AddAvailableWeapon(WeaponName weaponName)
    {
        foreach(var tuple in availableWeaponList)
        {
            if (tuple.weapon.Title == weaponName)
            {
                return;
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

    public (GameObject, Weapon) GetWeapon(WeaponName weaponName)
    {
        foreach (var tuple in availableWeaponList)
        {
            if (tuple.weapon.Title == weaponName)
            {
                availableWeaponList.Remove(tuple);
                return tuple;
            }
        }

        Debug.Log("Unavailable Weapon Request");
        return (null, null);    //Need to be removed
    }

    public void ReturnWeapon(GameObject obj, Weapon weapon = null)
    {
        if (weapon == null)
        {
            availableWeaponList.Add((obj, obj.GetComponent<Weapon>()));
        }
        else
        {
            availableWeaponList.Add((obj, weapon));
        }
        obj.transform.SetParent(weaponsParent);
    }
}
