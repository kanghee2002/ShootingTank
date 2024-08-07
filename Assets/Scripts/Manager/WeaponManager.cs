﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    [SerializeField] private List<GameObject> weaponPrefabList = new List<GameObject>();
    private List<Weapon> playerWeaponList = new List<Weapon>();
    private List<Weapon> availableWeaponList = new List<Weapon>();

    [SerializeField] private Transform weaponsParent;

    [SerializeField] private bool isRightWeaponEnabled;
    public bool IsRightWeaponEnabled { get => isRightWeaponEnabled; set => isRightWeaponEnabled = value; }

    public List<Weapon> PlayerWeaponList { get => playerWeaponList; }

    public int availableWeaponNum { get { return availableWeaponList.Count; } 
                                    private set { availableWeaponNum = availableWeaponList.Count; } }

    public List<WeaponName> InitialWeaponList;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }
    private void Initialize()
    {
        AddAvailableWeapon(WeaponName.Default);
        AddAvailableWeapon(WeaponName.Default);

        foreach(WeaponName weaponName in InitialWeaponList)
        {
            AddAvailableWeapon(weaponName);
        }

        //isRightWeaponEnabled = false;
    }

    public Weapon InitializeWeapon(GameObject weaponPrefab)      //Act at First
    {
        GameObject weaponObject = Instantiate(weaponPrefab, weaponsParent);
        Weapon weapon = weaponObject.GetComponent<Weapon>();
        return weapon;
    }

    public void AddAvailableWeapon(WeaponName weaponName)
    {
        if (weaponName != WeaponName.Default)
        {
            foreach (Weapon weapon in availableWeaponList)
            {
                if (weapon.Title == weaponName)
                {
                    // 중복되는 무기 추가 시 탄약 증가
                    return;
                }
            }
        }

        foreach (GameObject weaponPrefab in weaponPrefabList)
        {
            Weapon currentWeapon = weaponPrefab.GetComponent<Weapon>();
            if (currentWeapon.Title == weaponName)
            {
                Weapon instantiatedWeapon = InitializeWeapon(weaponPrefab);
                availableWeaponList.Add(instantiatedWeapon);
                playerWeaponList.Add(instantiatedWeapon);
                return;
            }
        }
    }

    public Weapon GetWeapon(bool isFront)
    {
        if (isFront)
        {
            Weapon nextWeapon = availableWeaponList[0];
            availableWeaponList.RemoveAt(0);
            return nextWeapon;
        }
        else
        {
            Weapon nextWeapon = availableWeaponList[availableWeaponList.Count - 1];
            availableWeaponList.RemoveAt(availableWeaponList.Count - 1);
            return nextWeapon;
        }
    }

    public void ReturnWeapon(Weapon weapon, bool isFront)
    {
        if (!weapon)
        {
            availableWeaponList.Add(weapon);
        }
        else
        {
            if (isFront) availableWeaponList.Add(weapon);
            else availableWeaponList.Insert(0, weapon);
        }
        weapon.transform.SetParent(weaponsParent);
    }

    public List<Weapon> GetAvailableWeaponList() => availableWeaponList;
}
