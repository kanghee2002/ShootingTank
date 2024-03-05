using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponName { DefaultWeapon, TestWeapon };

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField]
    private List<GameObject> weapons = new List<GameObject>();
    private Dictionary<GameObject, Weapon> availableWeaponDic = new Dictionary<GameObject, Weapon>();

    private void Awake()
    {

    }

    public void Init()      //Act at First
    {
        foreach (var weapon in availableWeaponDic) 
        {
            weapon.Value.Init();
        }
    }

    public void AddAvailableWeapon(WeaponName weaponName)
    {
        foreach(var weapon in availableWeaponDic)
        {
            if (weapon.Value.title == weaponName)
            {
                return;
            }
        }

        foreach (var obj in weapons)
        {
            var weapon = obj.GetComponent<Weapon>();
            if (weapon.title == weaponName)
            {
                availableWeaponDic.Add(obj, weapon);
                return;
            }
        }
    }

    public void ReturnWeapon(GameObject obj)
    {

    }
}
