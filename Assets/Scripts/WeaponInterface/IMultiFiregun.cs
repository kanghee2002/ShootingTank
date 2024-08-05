using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMultiFiregun : IDefaultgun
{
    public void IncreasePelletCount(int count);
}
