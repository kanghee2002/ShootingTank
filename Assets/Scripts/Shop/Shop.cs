using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private List<float> percentagePerRank = new()
    {
        60f,        // Rank C
        30f,        // Rank B
        9f,         // Rank A
        1f          // Rank S
    };

    private Dictionary<ItemRank, (int, int)> weaponPriceRangeDictionary = new()
    {
        { ItemRank.C, (80, 120) },
        { ItemRank.B, (150, 200) },
        { ItemRank.A, (400, 500) },
        { ItemRank.S, (700, 80) },

    };

    private Dictionary<ItemRank, (int, int)> utilityPriceRangeDictionary = new()
    {
        { ItemRank.C, (50, 80) },
        { ItemRank.B, (100, 150) },
        { ItemRank.A, (250, 350) },
        { ItemRank.S, (500, 650) },

    };

    [SerializeField] private List<Weapon> weaponPrefabList;
    [SerializeField] private List<PlayerUtility> utilityPrefabList;

    private Weapon weapon;
    private List<PlayerUtility> utilityList;

    private bool isPlayerInRange;
    public bool isOpening;

    private void Start()
    {
        isPlayerInRange = false;
        weapon = GetRandomWeapon();
        utilityList = new();
        for (int i = 0; i < 3; i++) utilityList.Add(GetRandomUtility());
    }

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.M))
        {
            weapon = GetRandomWeapon();
            utilityList.Clear();
            for (int i = 0; i < 3; i++) utilityList.Add(GetRandomUtility());
            StageManager.Instance.shopDisplay.SetShopDisplay(this, weapon, utilityList);
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isOpening = true;
            StageManager.Instance.shopDisplay.SetShopDisplay(this, weapon, utilityList);
        }

        if (isOpening && Vector3.Distance(transform.position, GameManager.Instance.playerObject.transform.position) > 10f)
        {
            StageManager.Instance.shopDisplay.ExitShop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            isPlayerInRange = false;
        }
    }


    private List<Weapon> GetWeaponListByRank(ItemRank itemRank) => weaponPrefabList.Where(weapon => weapon.ItemRank == itemRank).ToList();
    private List<PlayerUtility> GetUtilityListByRank(ItemRank itemRank) => utilityPrefabList.Where(utility => utility.ItemRank == itemRank).ToList();

    public Weapon GetRandomWeapon()
    {
        ItemRank selectedRank = GetRandomRank();

        List<Weapon> weaponList = GetWeaponListByRank(selectedRank);

        Weapon selectedWeapon = weaponList[Random.Range(0, weaponList.Count)];

        (int, int) priceRange = weaponPriceRangeDictionary[selectedRank];

        int randomPrice = Random.Range(priceRange.Item1, priceRange.Item2 + 1);

        selectedWeapon.price = randomPrice;

        return selectedWeapon;
    }

    public PlayerUtility GetRandomUtility()
    {
        ItemRank selectedRank = GetRandomRank();

        List<PlayerUtility> utilityList = GetUtilityListByRank(selectedRank);

        PlayerUtility selectedUtility = utilityList[Random.Range(0, utilityList.Count)];

        (int, int) priceRange = utilityPriceRangeDictionary[selectedRank];

        int randomPrice = Random.Range(priceRange.Item1, priceRange.Item2 + 1);

        selectedUtility.price = randomPrice;

        return selectedUtility;
    }


    private ItemRank GetRandomRank()
    {
        int randomPercentage = Random.Range(0, 100);

        List<int> cumulativePercentage = GetCumulativePercentage();

        ItemRank itemRank = ItemRank.C;
        for (int i = 0; i < cumulativePercentage.Count; i++)
        {
            if (randomPercentage < cumulativePercentage[i])
            {
                if (i == 0) itemRank = ItemRank.C;
                else if (i == 1) itemRank = ItemRank.B;
                else if (i == 2) itemRank = ItemRank.A;
                else itemRank = ItemRank.S;
                break;
            }
        }

        return itemRank;
    }

    private List<int> GetCumulativePercentage()
    {
        List<int> cumulativePercentage = new();

        int tmp = 0;
        for (int i = 0; i < percentagePerRank.Count; i++)
        {
            tmp += Mathf.FloorToInt(percentagePerRank[i]);
            cumulativePercentage.Add(tmp);
        }

        return cumulativePercentage;
    }

}
