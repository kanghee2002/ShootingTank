using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreTakeDamageMultiplerAdder : PlayerUtility
{
    [SerializeField]  private float coreTakeDamageMultiplerBonus;

    private CoreHealth[] coreHealthArray;

    private void Awake()
    {
        coreHealthArray = playerTransform.GetComponentsInChildren<CoreHealth>();
    }

    private void OnEnable()
    {
        foreach (CoreHealth coreHealth in coreHealthArray)
        {
            coreHealth.AddCoreDamageMultiplier(coreTakeDamageMultiplerBonus);
        }
    }

    private void OnDisable()
    {
        foreach (CoreHealth coreHealth in coreHealthArray)
        {
            coreHealth.MinusCoreDamageMultiplier(coreTakeDamageMultiplerBonus);
        }
    }
}
