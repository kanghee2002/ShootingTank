using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class JumpCountAdder : MonoBehaviour
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
