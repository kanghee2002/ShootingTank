using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargeSliderUI : MonoBehaviour
{
    [SerializeField]
    private Slider leftSlider;
    [SerializeField]
    private Slider rightSlider;

    private Weapon[] weapons = new Weapon[2];

    private void Start()
    {
        if (!WeaponManager.Instance.IsRightWeaponEnabled)
        {
            rightSlider.gameObject.SetActive(false);
        }
        else
        {
            rightSlider.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!weapons[0])
        {
            if (!GameManager.Instance.playerObj.
            GetComponent<WeaponController>().Weapons[0])
            {
                return;
            }
            weapons = GameManager.Instance.playerObj.
            GetComponent<WeaponController>().Weapons;
        }
        else
        {
            SetChargeSlider(leftSlider, WeaponHand.Left);
            if (WeaponManager.Instance.IsRightWeaponEnabled)
            {
                SetChargeSlider(rightSlider, WeaponHand.Right);
            }
        }
    }

    private void SetChargeSlider(Slider slider, WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        slider.value = weapons[weaponHandIdx].ChargePercentage;
    }
}
