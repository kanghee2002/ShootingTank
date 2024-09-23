using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Preferences")]
    [SerializeField] private GameObject eButton;
    [SerializeField] private Sprite openedChestSprite;
    [SerializeField] private List<PlayerUtility> utilityPrefabList;

    private List<float> percentagePerRank = new()
    {
        60f,        // Rank C
        30f,        // Rank B
        9f,         // Rank A
        1f          // Rank S
    };

    private SpriteRenderer spriteRenderer;

    private bool hasOpened;
    private bool isPlayerInRange;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hasOpened = false;
        isPlayerInRange = false;
        eButton.SetActive(false);
    }

    private void Update()
    {
        if (!hasOpened && isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            hasOpened = true;
            eButton.SetActive(false);
            spriteRenderer.sprite = openedChestSprite;
            SpawnItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasOpened && collision.CompareTag(Settings.playerTag))
        {
            isPlayerInRange = true;
            eButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!hasOpened && collision.CompareTag(Settings.playerTag))
        {
            isPlayerInRange = false;
            eButton.SetActive(false);
        }
    }

    private void SpawnItem()
    {
        ItemRank itemRank = GetRandomRank();

        List<PlayerUtility> utilityList = utilityPrefabList.Where(utility => utility.ItemRank == itemRank).ToList();

        PlayerUtility selectedUtility = utilityList[Random.Range(0, utilityList.Count)];

        PlayerUtility utilityObject = Instantiate(selectedUtility, transform);

        Rigidbody2D utilityRigidbody = utilityObject.GetComponent<Rigidbody2D>();
        utilityRigidbody.AddForce(Vector2.up * 13f, ForceMode2D.Impulse);

        utilityObject.DisableDetector(1.5f);
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
