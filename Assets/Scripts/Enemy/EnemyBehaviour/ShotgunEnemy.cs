using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShotgunEnemy : Enemy
{
    [Header("Attack Settings")]
    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private float weaponLength;

    [Header("Move Ray Settings")]
    [SerializeField]
    private float platformCheckRayGap;
    [SerializeField]
    private float platformCheckRayDistance;
    [SerializeField]
    private float lampCheckRayDistance;

    [Header("Attack Settings")]
    [SerializeField]
    private float aimAccuracy;

    private void Update()
    {
        //if (IsAttackPossible()) Attack(GetTargetDirection(playerTransform));
        //if (IsPlayerDetected) LookAtPlayer(headObj, headSpriteRenderer);
    }

    public override void Attack(Transform playerTransform)
    {
        Vector3 direction = GetTargetDirection(playerTransform);

        List<Vector3> directions = new();

        float aimAngle = (90 - (aimAccuracy * 9 * 0.1f)) * Mathf.Deg2Rad;

        Vector3 direction2 = new Vector3(
                direction.x * Mathf.Cos(aimAngle) - direction.y * Mathf.Sin(aimAngle),
                direction.x * Mathf.Sin(aimAngle) + direction.y * Mathf.Cos(aimAngle));
        Vector3 direction3 = new Vector3(
                direction.x * Mathf.Cos(-aimAngle) - direction.y * Mathf.Sin(-aimAngle),
                direction.x * Mathf.Sin(-aimAngle) + direction.y * Mathf.Cos(-aimAngle));

        directions.Add(direction);
        directions.Add(direction2);
        directions.Add(direction3);

        foreach (var dir in directions)
        {
            var obj = objectPool.GetBullet();

            obj.transform.position = transform.position + dir * weaponLength;
            obj.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;

            objectPool.LookAtDirection(obj, dir);

            Bullet bullet = obj.GetComponent<Bullet>();
            bullet.FinalDamage = damageValue;
            bullet.AddTargetTag(Settings.playerTag);

            StartCoroutine(CoolDownRoutine(coolTime));
            isCool = true;
        }
    }
}
