using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    /// <summary>
    /// 한 번에 발사되는 탄환 수
    /// </summary>
    public int pelletCount;
    /// <summary>
    /// 샷건의 정확도 (범위: 0 ~ 100)
    /// </summary>
    public float aimAccuracy;

    public override GameObject Fire(Vector3 dir)
    {
        float accuracyRadian = (90 - (aimAccuracy * 9 / 10)) * Mathf.Deg2Rad;

        for (int i = 0; i < pelletCount; i++)
        {
            float randomAngle = Random.Range(-accuracyRadian, accuracyRadian);
            Vector3 randomDir = new Vector3(
                dir.x * Mathf.Cos(randomAngle) - dir.y * Mathf.Sin(randomAngle),
                dir.x * Mathf.Sin(randomAngle) + dir.y * Mathf.Cos(randomAngle),
                0 );
            base.Fire(randomDir);
        }

        return null;
    }
}
