using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserEnemy : Enemy
{
    [Header("Warning Laser")]
    [SerializeField] private WarningLaser warningLaser;

    private EnemyController enemyController;
    private Coroutine currentAttackRoutine;
    private Vector3 firePosition;

    protected override void Awake()
    {
        base.Awake();

        enemyController = GetComponent<EnemyController>();

        enemyController.onPlayerLost += EnemyEvent_OnPlayerLost;
    }

    private void EnemyEvent_OnPlayerLost()
    {
        if (currentAttackRoutine != null)
        {
            StopCoroutine(currentAttackRoutine);
        }
        warningLaser.StopRoutine();
        isCool = false;
    }

    public override bool Attack(Transform playerTransform)
    {
        if (isCool)
        {
            return false;
        }

        currentAttackRoutine = StartCoroutine(DelayAttack(playerTransform));
        isCool = true;

        return true;
    }

    private IEnumerator DelayAttack(Transform playerTransform)
    {
        float delay = warningLaser.StartLuminesce();

        StartCoroutine(SetWarningLaser(playerTransform, delay));

        yield return new WaitForSeconds(delay);

        FireLaser(firePosition);
    }

    private IEnumerator SetWarningLaser(Transform playerTransform, float delay)
    {
        float elapsedTime = 0f;

        while (elapsedTime < delay)
        {
            if (delay - elapsedTime > 1f)
            {
                firePosition = warningLaser.SetPosition(transform.position, GetTargetDirection(playerTransform));
            }
            else
            {
                firePosition = warningLaser.SetPosition(transform.position, (firePosition - transform.position).normalized);
            }
            elapsedTime += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
    }

    private void FireLaser(Vector3 firePosition)
    {
        Vector3 direction = (firePosition - transform.position).normalized;

        GameObject obj = objectPool.GetBullet();

        obj.transform.position = transform.position + direction * weaponLength;

        Laser laser = obj.GetComponent<Laser>();

        laser.FinalDamage = damageValue;
        laser.FinalDamageOnCoreHit = damageValue;
        laser.AddTargetTag(Settings.playerTag);
        laser.SetLaserWidth(1);

        laser.Activate(direction);

        obj.GetComponent<LineRenderer>().material.color = new Color(128, 0, 0);

        StartCoroutine(CoolDownRoutine(coolTime));
    }
}
