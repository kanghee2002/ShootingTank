using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefaultBullet : Bullet
{
    protected override string TargetTag => "Player";
}
