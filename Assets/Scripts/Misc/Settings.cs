using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public const string playerTag = "Player";
    public const string enemyTag = "Enemy";

    public const float doorHorizontalOffset = 3.5f;
    public const float doorVerticalOffset = 2f;

    public static readonly List<RoomType> regularRoomTypeList = new()
    {
        RoomType.Small,
        RoomType.Medium,
        RoomType.Large
    };
}
