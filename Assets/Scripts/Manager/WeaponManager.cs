using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName { DefaultWeapon, TestWeapon };

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField]
    private List<GameObject> weapons = new List<GameObject>();
    private Dictionary<GameObject, Weapon> availableWeaponDic = new Dictionary<GameObject, Weapon>();

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
        foreach(var weapon in availableWeaponDic)
        {
            if (weapon.Value.Title == weaponName)
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
                availableWeaponDic.Add(result.Item1, result.Item2);
                return;
            }
        }
    }

    public void ReturnWeapon(GameObject obj)
    {
        availableWeaponDic.Add(obj, obj.GetComponent<Weapon>());
    }
}
