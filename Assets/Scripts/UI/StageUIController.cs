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
    private Slider playerHealthSlider;
    [SerializeField]
    private TMP_Text playerHealthText;

    private Health playerHealth;

    private void Start()
    {
        InitDisplay();

        foreach (GameObject playerObject in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (playerObject != null)
            {
                if (playerObject.TryGetComponent(out Health health))
                {
                    playerHealth = health;
                    health.SetHealthSlider(playerHealthSlider);
                    health.onHpChanged += SetHealthText;
                }
            }
        }

        WeaponController weaponController = FindObjectOfType<WeaponController>();
        if (weaponController != null)
        {
            weaponController.onWeaponChanged += SetAmmoText;
            weaponController.onWeaponCharged += SetChargeSliderValue;
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

    private void SetHealthText()
    {
        playerHealthText.text = playerHealth.GetHealth.ToString();
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
