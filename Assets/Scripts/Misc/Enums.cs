using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponHand { Left = 0, Right = 1 }

public enum JumpState
{
    NotJumping,
    Jumping,
    Falling
}

public enum WeaponName
{
    Default,
    Rifle,
    Shotgun,
    Missilegun,
    GrenadeLauncher,
    Lasergun,
    Burstgun,
    LaserShotgun
}

public enum RoomType
{
    None,
    Entrance,
    Small,
    Medium,
    Large,
    Chest,
    Shop,
    Boss,
    Hidden,
}

public enum Orientation
{
    Up,
    Down,
    Left,
    Right
}

public enum EnemyRank
{
    C,
    B,
    A,
    S
}