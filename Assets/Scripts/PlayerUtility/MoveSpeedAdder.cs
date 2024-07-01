using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class MoveSpeedAdder : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private int addingMoveSpeedValue = 3;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
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
