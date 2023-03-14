using Characters;
using UnityEngine;

[CreateAssetMenu]
public class BaseBuffSO : ScriptableObject
{
    [SerializeField] private string buffName;
    [SerializeField] private string description;
    [SerializeField] private StatName statName;

    [SerializeField] private float baseValue_Level;
    [SerializeField] private float multValue_Level;
    [SerializeField] private int maxLevel = 5;

    public string Name => buffName;
    public string Description => description;
    public StatName StatName => statName;
    public float BaseValue_Level => baseValue_Level;
    public float MultValue_Level => multValue_Level;
    public int MaxLevel => maxLevel;
}
