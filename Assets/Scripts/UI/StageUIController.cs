using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageUIController : MonoBehaviour
{
    [Header("Weapon Display")]
    [SerializeField] private GameObject[] weaponDisplay;

    [Header("Charge Slider")]
    [SerializeField] private Slider[] weaponChargeSlider;

    [Header("Ammo Text")]
    [SerializeField] private TMP_Text[] weaponAmmoText;

    [Header("Hp Display")]
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text playerHealthText;

    [Header("Player")]
    [SerializeField] private GameObject player;

    private Health playerHealth;

    private void Start()
    {
        InitializeDisplay();
        InitializePlayerComponents();
    }

    private void InitializeDisplay()
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

    private void InitializePlayerComponents()
    {
        playerHealth = player.GetComponent<Health>();
        playerHealth.SetHealthSlider(playerHealthSlider);
        playerHealth.onHealthChanged += SetHealthText;

        WeaponController weaponController = player.GetComponent<WeaponController>();
        weaponController.onWeaponChanged += SetAmmoText;
        weaponController.onWeaponCharged += SetChargeSliderValue;
        weaponController.onWeaponShoot += SetAmmoText;
    }

    private void SetHealthText(float currentHealth, float maxHealth)
    {
        playerHealthText.text = currentHealth.ToString();
    }

    private void SetChargeSliderValue(WeaponHand weaponHand, Weapon weapon)
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
}
