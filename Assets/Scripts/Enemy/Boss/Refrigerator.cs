using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class Refrigerator : Boss
{
    [SerializeField] private GameObject[] bulletPrefabArray;

    private List<string> patternArray = new()
    {
        ThrowFood,
        ThrowIcicle,
    };

    private const string ThrowFood = "Attack_ThrowFood";
    private const string ThrowIcicle = "Attack_ThrowIcicle";

    private State state = State.Idle;

    private void Update()
    {
        // DEBUG
        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeState(State.Idle);
        }
    }

    public void Initialize(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        ChangeState(State.Idle);
    }

    private void ChangeState(State state)
    {
        this.state = state;
        OnStateChanged(state);
    }

    private void OnStateChanged(State state)
    {
        switch (state)
        {
            case State.Idle:
                StartCoroutine(IdleRoutine());
                break;
            case State.ReadyToAttack:
                ExcutePattern();
                break;
            case State.Attacking:
                break;
        }
    }

    private IEnumerator IdleRoutine()
    {
        Vector2 originPosition = transform.position;

        float moveDistance = 5f;
        int moveCount = 6;
        
        float elapsedTime = 0f;

        while (elapsedTime < idleTime)
        {
            transform.position = new Vector2(transform.position.x, originPosition.y + moveDistance * Mathf.Sin(elapsedTime * (moveCount / 2) * Mathf.PI / idleTime));

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        ChangeState(State.ReadyToAttack);
    }

    private void ExcutePattern()
    {
        string selectedPattern = patternArray[Random.Range(0, patternArray.Count)];

        StartCoroutine(selectedPattern);
    }

    private IEnumerator Attack_ThrowFood()
    {
        float attackTime = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < attackTime)
        {
            Debug.Log("Throw Random Food");

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        ChangeState(State.Idle);
    }

    private IEnumerator Attack_ThrowIcicle()
    {
        float attackTime = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < attackTime)
        {
            Debug.Log("Throw Icicle");

            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        ChangeState(State.Idle);
    }
}
