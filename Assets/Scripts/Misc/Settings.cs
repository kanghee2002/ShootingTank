using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public const string playerTag = "Player"; 

    public const float doorHorizontalOffset = 1.5f;
    public const float doorVerticalOffset = 2f;

    public static readonly List<RoomType> regularRoomTypeList = new()
    {
        RoomType.Small,
        RoomType.Medium,
        RoomType.Large
    };
}
