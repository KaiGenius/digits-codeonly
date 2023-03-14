using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelProgressionSO : ScriptableObject
{
    [SerializeField] private int[] cardsLevelCost = new int[0];

    public int GetCost(int level)
    {
        return cardsLevelCost[Mathf.Clamp(level - 1, 0, cardsLevelCost.Length-1)];
    }
}
