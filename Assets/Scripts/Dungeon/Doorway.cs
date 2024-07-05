using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Doorway
{
    public Orientation orientation;

    public Vector2Int position;

    public Vector2Int doorwayCopyPosition;
    public int doorwayCopyWidth;
    public int doorwayCopyHeight;
}
