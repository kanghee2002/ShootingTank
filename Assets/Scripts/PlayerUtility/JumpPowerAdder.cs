using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPowerAdder : PlayerUtility
{
    private PlayerController playerController;

    [SerializeField] private int addingJumpPowerValue = 5;

    private void Awake()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        playerController.AddJumpPowerValue(addingJumpPowerValue);
    }

    private void OnDisable()
    {
        playerController.MinusJumpPowerValue(addingJumpPowerValue);
    }
}
