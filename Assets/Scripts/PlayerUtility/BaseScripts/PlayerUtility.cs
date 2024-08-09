using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerUtility : MonoBehaviour
{
    public string utilityName;

    public Transform playerTransform;

    [SerializeField] protected UtilityRank utilityRank;

    public UtilityRank UtilityRank { get => utilityRank; }

    [SerializeField] protected int maxUtilityCount;

    public int MaxUtilityCount { get => maxUtilityCount; }
}
