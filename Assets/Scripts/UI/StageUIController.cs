using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUIController : MonoBehaviour
{
    [Header("Charge Slider")]
    [SerializeField]
    private Slider leftSlider;
    [SerializeField]
    private Slider rightSlider;

    [SerializeField]
    private Weapon[] weapons = null;

    private void Start()
    {
        InitSlider();
    }

    private void Update()
    {
        if (weapons[0] != null)
        {
            SetChargeSlider(leftSlider, WeaponHand.Left);
            if (WeaponManager.Instance.IsRightWeaponEnabled)
            {
                SetChargeSlider(rightSlider, WeaponHand.Right);
            }
        }
    }

    private void InitSlider()
    {
        if (!WeaponManager.Instance.IsRightWeaponEnabled)
        {
            rightSlider.gameObject.SetActive(false);
        }
        else
        {
            rightSlider.gameObject.SetActive(true);
        }
        weapons = GameManager.Instance.playerObj.GetComponent<WeaponController>().Weapons;
    }

    private void SetChargeSlider(Slider slider, WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        slider.value = weapons[weaponHandIdx].ChargePercentage;
    }
}
