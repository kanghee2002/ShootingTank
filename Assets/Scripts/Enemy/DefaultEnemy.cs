using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemy : Enemy
{
    [SerializeField]
    private GameObject headObj;

    [SerializeField]
    private SpriteRenderer headSpriteRenderer;

    private void Start()
    {
        base.Init();
    }

    private void Update()
    {
        if (IsAttackPossible()) Attack(Player);
        if (IsPlayerDetected) LookAtPlayer(headObj, headSpriteRenderer);
    }
}
