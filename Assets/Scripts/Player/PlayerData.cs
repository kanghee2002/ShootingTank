using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private int coin;
    public int Coin { get => coin; }

    public void GetCoin(int value)
    {
        coin += value;
    }
}
