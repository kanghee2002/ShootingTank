using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class JumpPowerAdder : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private int addingJumpPowerValue = 5;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
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
