using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUtility : PlayerUtility
{
    [Header("Move")]
    [SerializeField] private int jumpCountBonus;
    [SerializeField] private float jumpPowerBonus;
    [SerializeField] private float moveSpeedBonus;
    [SerializeField] private float downFallCoolTimeBonus;

    [Header("Core")]
    [SerializeField] private float coreScaleBonus;
    [SerializeField] private float coreTakeDamageMultiplierBonus;

    [Header("Etc")]
    [SerializeField] private float playerScale;
    [SerializeField] private bool canJumpInfinitely;
    [SerializeField] private bool canSetCoreInactive;
}
