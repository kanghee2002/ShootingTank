using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShotgunEnemy : Enemy
{
    [Header("Shotgun Settings")]
    [SerializeField] Shotgun.ShootingType shootingType;
    [SerializeField] private int pelletCount;
    [SerializeField] private float aimAccuracy;         // used on Random
    [SerializeField] private float shootingAngle;       // used on Angle

    public override bool Attack(Transform playerTransform)
    {
        if (isCool)
        {
            return false;
        }

        Vector3 direction = GetTargetDirection(playerTransform);

        List<Vector3> directions = new();

        if (shootingType == Shotgun.ShootingType.Random)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                directions.Add(GetRandomDirection(direction, aimAccuracy));
            }
        }
        else if (shootingType == Shotgun.ShootingType.Angle)
        {
            if (pelletCount % 2 == 0)       // Even
            {
                float defaultAngle = shootingAngle / 2f;
                for (int i = 0; i < pelletCount / 2; i++)
                {
                    directions.Add(GetDirectionByAngle(direction, defaultAngle + shootingAngle * i));
                    directions.Add(GetDirectionByAngle(direction, -(defaultAngle + shootingAngle * i)));
                }
            }
            else                            // Odd
            {
                directions.Add(direction);

                for (int i = 1; i <= pelletCount / 2; i++)
                {
                    directions.Add(GetDirectionByAngle(direction, shootingAngle * i));
                    directions.Add(GetDirectionByAngle(direction, -shootingAngle * i));
                }
            }
        }

        foreach (Vector3 dir in directions)
        {
            GameObject obj = objectPool.GetBullet();
            Bullet bullet = obj.GetComponent<Bullet>();

            SetBullet(obj, bullet, dir, damageValue);
        }

        isCool = true;
        StartCoroutine(CoolDownRoutine(coolTime));

        return true;
    }


    private Vector3 GetDirectionByAngle(Vector3 dir, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        Vector3 newDir = new Vector3(
                dir.x * Mathf.Cos(radian) - dir.y * Mathf.Sin(radian),
                dir.x * Mathf.Sin(radian) + dir.y * Mathf.Cos(radian),
                0);
        return newDir;
    }

    private Vector3 GetRandomDirection(Vector3 dir, float aimAccuracy)
    {
        float accuracyRadian = (90 - (aimAccuracy * 9 / 10)) * Mathf.Deg2Rad;
        float randomRadian = Random.Range(-accuracyRadian, accuracyRadian);
        Vector3 randomDir = new Vector3(
                dir.x * Mathf.Cos(randomRadian) - dir.y * Mathf.Sin(randomRadian),
                dir.x * Mathf.Sin(randomRadian) + dir.y * Mathf.Cos(randomRadian),
                0);
        return randomDir;
    }
}
