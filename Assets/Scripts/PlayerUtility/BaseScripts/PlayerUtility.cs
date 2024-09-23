using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerUtility : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField] private string utilityName;
    public string UtilityName { get => utilityName; }

    [SerializeField] protected ItemRank itemRank;
    public ItemRank ItemRank { get => itemRank; }

    [SerializeField] protected string description;
    public string Description { get => description; }

    [Header("Details")]
    [SerializeField] protected int maxUtilityCount;
    public int MaxUtilityCount { get => maxUtilityCount; }

    [HideInInspector] public int price;     // used in Shop

    public abstract void GetAbility();

    public void DisableDetector(float time) => StartCoroutine(DisableDetectorRoutine(time));

    private IEnumerator DisableDetectorRoutine(float time)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(time);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
