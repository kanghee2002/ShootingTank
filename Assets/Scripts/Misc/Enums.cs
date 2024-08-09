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
    //C
    Default,
    M16,
    AK47,
    abandonedShotgun,
    RustyShotgun,
    Revolver,
    NoChargingRifle,
    BrokenRifle,
    OldSniperRifle,
    HugeRifle,
    FaintGun,
    SlowGun,
    PeaGun,
    
    //B
    GrenadeLauncher,
    Lasergun,
    Missilegun,
    Coregun,
    SniperRifle,
    M4A16,
    Shotgun,
    Scattergun,         //산탄총
    HugeRevolver,
    ScatterShotgun,      //산탄 산탄총
    Splitgun,
    Tailgun,
    
    //A
    LaserShotgun,
    MissileShotgun,
    BurstGrenadeLauncher,
    BurstLasergun,
    BurstShotgun,
    HugeLasergun,
    HugeExplosionGrenadeLauncher,
    Sniper,

    //S
    Taser
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

public enum UtilityRank
{
    C,
    B,
    A,
    S
}

public enum GameState
{
    gameStarted,
    loadLevel,
    playingLevel,
    enterRoom,
    enterBossRoom,
    bossStage,
    levelCompleted,
    gameWon,
    gameLost,
    gamePaused,
    restartGame
}