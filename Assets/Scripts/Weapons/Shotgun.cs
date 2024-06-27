using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    /// <summary>
    /// 한 번에 발사되는 탄환 수
    /// </summary>
    [SerializeField]
    private int pelletCount;
    public int PelletCount { get => pelletCount; set => pelletCount = value; }

    public override void Fire(Vector3 direction)
    {
        if (CurAmmo <= 0)
        {
            Debug.Log(name + " : No Ammo");
            return;
        }

        for (int i = 0; i < pelletCount; i++)
        {
            var obj = objectPool.GetBullet();

            var randomDirection = GetRandomDirection(direction, AimAccuracy);

            obj.GetComponent<Bullet>().FinalDamage = damageValue * GetDamageMultiplier(ChargePercentage);

            obj.transform.position = transform.position + randomDirection * weaponLength;
            obj.GetComponent<Rigidbody2D>().velocity = randomDirection * bulletSpeed;

            objectPool.LookAtDirection(obj, randomDirection);
        }

        CurAmmo -= pelletCount;

        Recharge();
    }
}
