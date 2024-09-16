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

    private void Start()
    {
        exitButton.onClick.AddListener(ExitShop);
    }

    public void SetShopDisplay(Shop shop,Weapon weapon, List<PlayerUtility> utilityList)
    {
        currentShop = shop;

        shopUI.SetActive(true);

        // Set Weapon
        Sprite weaponSprite = weapon.GetComponent<SpriteRenderer>().sprite;

        shopComponentList[3].rank.text = weapon.ItemRank.ToString();
        shopComponentList[3].image.sprite = weaponSprite;
        shopComponentList[3].name.text = weapon.WeaponName;
        shopComponentList[3].description.text = weapon.Description;
        shopComponentList[3].price.text = "$" + weapon.price.ToString();

        int ratio = weaponSprite.texture.width / weaponSprite.texture.height;

        shopComponentList[3].image.rectTransform.sizeDelta = new Vector2(50f * ratio, 50f);

        // Set Utility
        for (int i = 0; i < 3; i++)
        {
            shopComponentList[i].rank.text = utilityList[i].ItemRank.ToString();
            shopComponentList[i].image.sprite = utilityList[i].GetComponent<SpriteRenderer>().sprite;
            shopComponentList[i].name.text = utilityList[i].UtilityName;
            shopComponentList[i].description.text = utilityList[i].Description;
            shopComponentList[i].price.text = "$" + utilityList[i].price.ToString();
        }

        foreach (ShopComponent shopComponent in shopComponentList)
        {
            // Set shopComponent.buyButton
        }
    }

    public void ExitShop()
    {
        currentShop.isOpening = false;
        shopUI.SetActive(false);
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
