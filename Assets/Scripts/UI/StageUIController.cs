using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageUIController : MonoBehaviour
{
    [Header("Weapon Display")]
    [SerializeField] private GameObject[] weaponDisplay;

    [Header("Charge Display")]
    [SerializeField] private Slider[] weaponChargeSlider;

    [Header("Ammo Display")]
    [SerializeField] private Slider[] weaponAmmoSlider;
    [SerializeField] private TMP_Text[] weaponAmmoText;

    [Header("Hp Display")]
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text playerHealthText;

    [Header("Coin Display")]
    [SerializeField] private TMP_Text coinText;
    
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
        weaponController.onWeaponChanged += SetAmmoDisplay;
        weaponController.onWeaponCharged += SetChargeSliderValue;
        weaponController.onWeaponShoot += SetAmmoDisplay;
        weaponController.onWeaponAmmoChanged += SetAmmoDisplay;

        weaponController.onWeaponAmmoChanged?.Invoke(WeaponHand.Left, weaponController.Weapons[0]);
        weaponController.onWeaponAmmoChanged?.Invoke(WeaponHand.Right, weaponController.Weapons[1]);
    
        PlayerData playerData = player.GetComponent<PlayerData>();
        playerData.onGetCoin += SetCoinText;
        playerData.onUseCoin += SetCoinText;
    }

    private void SetHealthText(float currentHealth, float maxHealth)
    {
        playerHealthText.text = Mathf.FloorToInt(currentHealth).ToString();
    }

    private void SetChargeSliderValue(WeaponHand weaponHand, Weapon weapon)
    {
        int weaponHandIdx = (int)weaponHand;
        weaponChargeSlider[weaponHandIdx].value = weapon.ChargePercentage;
    }

    private void SetAmmoDisplay(WeaponHand weaponHand, Weapon weapon)
    {
        int weaponHandIdx = (int)weaponHand;
        if (weapon.Title == WeaponName.Default)
        {
            weaponAmmoSlider[weaponHandIdx].value = 1f;
            weaponAmmoText[weaponHandIdx].text = " - / - ";
        }
        else
        {
            weaponAmmoSlider[weaponHandIdx].value = (float)weapon.CurAmmo / weapon.MaxAmmo;
            weaponAmmoText[weaponHandIdx].text = weapon.CurAmmo.ToString() + " / " + weapon.MaxAmmo.ToString();
        }
    }

    private void SetCoinText(int coin)
    {
        coinText.text = coin.ToString();
    }
}
