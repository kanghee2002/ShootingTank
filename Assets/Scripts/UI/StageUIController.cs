using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageUIController : MonoBehaviour
{
    [Header("Weapon Display")]
    [SerializeField]
    private GameObject leftWeaponDisplay;
    [SerializeField]
    private GameObject rightWeaponDisplay;

    [Header("Charge Slider")]
    [SerializeField]
    private Slider leftWeaponChargeSlider;
    [SerializeField]
    private Slider rightWeaponChargeSlider;

    [Header("Ammo Text")]
    [SerializeField]
    private TMP_Text leftWeaponAmmoText;
    [SerializeField]
    private TMP_Text rightWeaponAmmoText;

    [Header("Hp Display")]
    [SerializeField]
    private Image hpImage;
    [SerializeField]
    private TMP_Text hpText;

    private PlayerController playerController;
    private Weapon[] weapons;

    private void Start()
    {
        InitDisplay();
        playerController = GameManager.Instance.playerObj.GetComponent<PlayerController>();
    }

    private void Update()
    {
        //Set Sliders
        if (weapons[0] != null)
        {
            SetChargeSlider(leftWeaponChargeSlider, WeaponHand.Left);
            SetChargeSlider(rightWeaponChargeSlider, WeaponHand.Right);
        }

        //Set Ammo Texts
        if (weapons[0] != null)
        {
            SetAmmoText(leftWeaponAmmoText, WeaponHand.Left);
            SetAmmoText(rightWeaponAmmoText, WeaponHand.Right);
        }

        //Set Hp Image
        SetHpDisplay();
    }

    private void InitDisplay()
    {
        if (!WeaponManager.Instance.IsRightWeaponEnabled)
        {
            rightWeaponDisplay.gameObject.SetActive(false);
        }
        else
        {
            rightWeaponDisplay.gameObject.SetActive(true);
        }
        weapons = GameManager.Instance.playerObj.GetComponent<WeaponController>().Weapons;
    }

    private void SetChargeSlider(Slider slider, WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        slider.value = weapons[weaponHandIdx].ChargePercentage;
    }

    private void SetAmmoText(TMP_Text ammoText, WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        if (weapons[weaponHandIdx].Title == WeaponName.Default)
        {
            ammoText.text = " - / - ";
        }
        else
        {
            ammoText.text = weapons[weaponHandIdx].CurAmmo.ToString() + " / " + weapons[weaponHandIdx].MaxAmmo.ToString();
        }
    }

    private void SetHpDisplay()
    {
        hpImage.fillAmount = playerController.CurHp / playerController.MaxHp;
        hpText.text = playerController.CurHp.ToString();
    }
}
