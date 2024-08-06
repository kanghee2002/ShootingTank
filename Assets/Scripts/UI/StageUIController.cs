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
    
    private GameObject player;

    private void Start()
    {
        player = GameManager.Instance.playerObject;

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
        Health playerHealth = player.GetComponent<Health>();
        playerHealth.SetHealthSlider(playerHealthSlider);
        playerHealth.onHealthChanged += SetHealthText;

        WeaponController weaponController = player.GetComponent<WeaponController>();
        weaponController.onWeaponChanged += SetAmmoText;
        weaponController.onWeaponCharged += SetChargeSliderValue;
        weaponController.onWeaponShoot += SetAmmoText;
        weaponController.onWeaponAmmoChanged += SetAmmoText;

        weaponController.onWeaponAmmoChanged?.Invoke(WeaponHand.Left, weaponController.Weapons[0]);
        weaponController.onWeaponAmmoChanged?.Invoke(WeaponHand.Right, weaponController.Weapons[1]);
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
