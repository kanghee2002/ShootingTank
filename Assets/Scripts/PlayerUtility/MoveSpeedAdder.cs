using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedAdder : PlayerUtility
{
    private PlayerController playerController;

    [SerializeField] private int addingMoveSpeedValue = 3;

    private void Awake()
    {
        playerController = playerTransform.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        playerController.AddMoveSpeedValue(addingMoveSpeedValue);
    }

    private void OnDisable()
    {
        playerController.MinusMoveSpeedValue(addingMoveSpeedValue);
    }
}
