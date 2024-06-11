using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageUIController : MonoBehaviour
{
    [Header("Weapon Display")]
    [SerializeField]
    private GameObject[] weaponDisplay;

    [Header("Charge Slider")]
    [SerializeField]
    private Slider[] weaponChargeSlider;

    [Header("Ammo Text")]
    [SerializeField]
    private TMP_Text[] weaponAmmoText;

    [Header("Hp Display")]
    [SerializeField]
    private Image hpImage;
    [SerializeField]
    private TMP_Text hpText;

    private void Start()
    {
        InitDisplay();

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.onHpChanged += SetHpDisplay;
        }

        WeaponController weaponController = FindObjectOfType<WeaponController>();
        if (weaponController != null)
        {
            weaponController.onWeaponChanged += SetAmmoText;
            weaponController.onWeaponCharged += SetChargeSlider;
            weaponController.onWeaponShoot += SetAmmoText;
        }
    }

    private void InitDisplay()
    {
        if (!WeaponManager.Instance.IsRightWeaponEnabled)
        {
            weaponDisplay[1].gameObject.SetActive(false);
        }
        else
        {
            weaponDisplay[1].gameObject.SetActive(true);
        }
    }

    private void SetChargeSlider(WeaponHand weaponHand, Weapon weapon)
    {
        int weaponHandIdx = (int)weaponHand;
        weaponChargeSlider[weaponHandIdx].value = weapon.ChargePercentage;
    }

    private void SetAmmoText(WeaponHand weaponHand, Weapon weapon)
    {
        int weaponHandIdx = (int)weaponHand;
        if (weapon.Title == WeaponName.Default)
        {
            weaponAmmoText[weaponHandIdx].text = " - / - ";
        }
        else
        {
            weaponAmmoText[weaponHandIdx].text = weapon.CurAmmo.ToString() + " / " + weapon.MaxAmmo.ToString();
        }
    }

    private void SetHpDisplay(float curHp, float maxHp)
    {
        hpImage.fillAmount = curHp / maxHp;
        hpText.text = curHp.ToString();
    }
}
