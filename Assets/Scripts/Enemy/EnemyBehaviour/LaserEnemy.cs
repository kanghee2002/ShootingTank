using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LaserEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField]
    private float weaponLength;

    [Header("Warning Laser")]
    [SerializeField]
    private WarningLaser warningLaser;

    private Vector2 targetVec;

    private void Update()
    {
        /*if (IsAttackPossible())
        {
            StartCoroutine(DelayAttack());
            isCool = true;
        }
        if (IsPlayerDetected)
        {
            //LookAtPlayer(headObj, headSpriteRenderer);
            SetWarningLaser();
        }*/
    }

    public override bool Attack(Transform playerTransform)
    {
        if (isCool)
        {
            return false;
        }

        Vector3 direction = GetTargetDirection(playerTransform);

        var obj = objectPool.GetBullet();

        obj.transform.position = transform.position + direction * weaponLength;

        Laser laser = obj.GetComponent<Laser>();
        laser.FinalDamage = damageValue;
        laser.AddTargetTag(Settings.playerTag);
        laser.Activate(direction);

        obj.GetComponent<LineRenderer>().material.color = new Color(128, 0, 0);

        StartCoroutine(CoolDownRoutine(coolTime));
        isCool = true;

        return true;
    }

    private IEnumerator DelayAttack(Transform playerTransform)
    {
        float delay = warningLaser.StartLuminesce();

        yield return new WaitForSeconds(delay);

        if ((transform.position - playerTransform.position).magnitude <= warningLaser.MaxDistance)
        {
            //Attack(GetTargetDirection(playerTransform));
        }
        else
        {
            isCool = false;
        }
    }


    private void SetWarningLaser()
    {
        //warningLaser.SetPosition(transform.position, playerTransform.position);
    }
}
