using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class ActorColorizer : UnitySingleton<ActorColorizer>
{
    [SerializeField] private List<Color> avalibleColors;
    private List<Color> cash;

    protected override void SingletonAwake()
    {
        cash = new List<Color>(avalibleColors);
    }

    public Color GetColor()
    {
        int index = Random.Range(0, cash.Count);
        var color = cash[index];
        cash.RemoveAt(index);

        return color;
    }
}
