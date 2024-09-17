using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int coin;
    public int Coin { get => coin; }

    public Action<int> onGetCoin;       // New Coin Value
    public Action<int> onUseCoin;       // New Coin Value

    public void GetCoin(int value)
    {
        coin += value;
        onGetCoin?.Invoke(coin);
    }

    public void UseCoin(int value)
    {
        coin -= value;
        onUseCoin?.Invoke(coin);
    }
}
