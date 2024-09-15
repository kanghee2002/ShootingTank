using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO_", menuName = "ItemSO")]
public class ItemSO : ScriptableObject
{
    public enum ItemType
    {
        Weapon,
        Utility,
    }

    public ItemType itemType;
    public Sprite sprite;
    public string title;
    public ItemRank rank;
    public string description;

    [Header("Weapon")]
    public WeaponName weaponName;
}
