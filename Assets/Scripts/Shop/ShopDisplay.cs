using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopDisplay : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;

    [SerializeField] private List<ShopComponent> shopComponentList;

    [SerializeField] private Button exitButton;

    private Shop currentShop;

    private int weaponIndex = 3;

    private bool isOpening;
    public bool IsOpening { get => isOpening; }

    private void Start()
    {
        exitButton.onClick.AddListener(ExitShop);

        isOpening = false;
    }

    public void SetShopDisplay(Shop shop,Weapon weapon, List<PlayerUtility> utilityList)
    {
        currentShop = shop;
        currentShop.isDisplaying = true;

        shopUI.SetActive(true);
        isOpening = true;

        foreach (ShopComponent shopComponent in shopComponentList)
        {
            shopComponent.buyButton.onClick.RemoveAllListeners();
        }

        // Set Weapon
        Sprite weaponSprite = weapon.GetComponent<SpriteRenderer>().sprite;

        shopComponentList[weaponIndex].rank.text = weapon.ItemRank.ToString();
        shopComponentList[weaponIndex].image.sprite = weaponSprite;
        shopComponentList[weaponIndex].name.text = weapon.WeaponName;
        shopComponentList[weaponIndex].description.text = weapon.Description;
        if (shop.isSold[weaponIndex]) shopComponentList[weaponIndex].price.text = "구매 완료";
        else shopComponentList[weaponIndex].price.text = "$" + weapon.price.ToString();

        int ratio = weaponSprite.texture.width / weaponSprite.texture.height;

        shopComponentList[weaponIndex].image.rectTransform.sizeDelta = new Vector2(50f * ratio, 50f);

        shopComponentList[weaponIndex].buyButton.onClick.AddListener(() => TryBuyWeapon(shop, weapon));

        // Set Utility
        for (int i = 0; i < 3; i++)
        {
            shopComponentList[i].rank.text = utilityList[i].ItemRank.ToString();
            shopComponentList[i].image.sprite = utilityList[i].GetComponent<SpriteRenderer>().sprite;
            shopComponentList[i].name.text = utilityList[i].UtilityName;
            shopComponentList[i].description.text = utilityList[i].Description;
            if (shop.isSold[i]) shopComponentList[i].price.text = "구매 완료";
            else shopComponentList[i].price.text = "$" + utilityList[i].price.ToString();

            int index = i;
            shopComponentList[i].buyButton.onClick.AddListener(() => TryBuyUtility(shop, utilityList[index], index));
        }
    }

    public void ExitShop()
    {
        currentShop.isDisplaying = false;
        shopUI.SetActive(false);
        isOpening = false;
    }

    private void TryBuyWeapon(Shop shop, Weapon weapon)
    {
        PlayerData playerData = GameManager.Instance.playerObject.GetComponent<PlayerData>();

        if (playerData.Coin >= weapon.price)
        {
            if (shop.isSold[weaponIndex])
            {
                // Can't buy sound
                return;
            }

            playerData.UseCoin(weapon.price);

            WeaponManager.Instance.AddAvailableWeapon(weapon.Title);

            shop.isSold[weaponIndex] = true;

            shopComponentList[weaponIndex].price.text = "구매 완료";
        }
        else
        {
            // Can't buy sound
        }
    }

    private void TryBuyUtility(Shop shop, PlayerUtility utility, int index)
    {
        PlayerData playerData = GameManager.Instance.playerObject.GetComponent<PlayerData>();

        if (playerData.Coin >= utility.price)
        {
            if (shop.isSold[index])
            {
                // Can't buy sound
                return;
            }

            playerData.UseCoin(utility.price);

            utility.GetAbility();

            shop.isSold[index] = true;

            shopComponentList[index].price.text = "구매 완료";
        }
        else
        {
            // Can't buy sound
        }
    }
}

[System.Serializable]
public class ShopComponent
{
    public TMP_Text rank;
    public Image image;
    public TMP_Text name;
    public TMP_Text description;
    public TMP_Text price;
    public Button buyButton;
}
