using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCountAdder : PlayerUtility
{
    private PlayerController playerController;

    [SerializeField] private int addingJumpCountValue = 1;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        playerController.AddMaxJumpCount(addingJumpCountValue);
    }

    private void OnDisable()
    {
        playerController.MinusMaxJumpCount(addingJumpCountValue);
    }
}
