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
    private WeaponController weaponController;
    private Weapon[] weapons;

    private void Start()
    {
        InitDisplay();

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.onPlayerHpChanged += SetHpDisplay;
        }

        weaponController = GameManager.Instance.playerObj.GetComponent<WeaponController>();
        weaponController.onWeaponChanged += OnWeaponChanged;

        weapons = GameManager.Instance.playerObj.GetComponent<WeaponController>().Weapons;
    }

    public void OnWeaponCharged()
    {
        SetChargeSlider(leftWeaponChargeSlider, WeaponHand.Left);
        SetChargeSlider(rightWeaponChargeSlider, WeaponHand.Right);
    }

    public void OnWeaponChanged()
    {
        SetAmmoText(leftWeaponAmmoText, WeaponHand.Left);
        SetAmmoText(rightWeaponAmmoText, WeaponHand.Right);
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
    }

    private void SetChargeSlider(Slider slider, WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        slider.value = weapons[weaponHandIdx].ChargePercentage;
    }

    private void SetAmmoText(TMP_Text ammoText, WeaponHand weaponHand)
    {
        int weaponHandIdx = (int)weaponHand;
        if (weapons[weaponHandIdx] == null)
        {
            return;
        }
        if (weapons[weaponHandIdx].Title == WeaponName.Default)
        {
            ammoText.text = " - / - ";
        }
        else
        {
            ammoText.text = weapons[weaponHandIdx].CurAmmo.ToString() + " / " + weapons[weaponHandIdx].MaxAmmo.ToString();
        }
    }

    private void SetHpDisplay(float curHp, float maxHp)
    {
        hpImage.fillAmount = curHp / maxHp;
        hpText.text = curHp.ToString();
    }
}
