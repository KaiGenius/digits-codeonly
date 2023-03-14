using Characters;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public interface IActor
{
    void UpdateScore(int value, Operation operation, bool isImportant = false, bool isTriggerAnim = true);
    event Action<IActor, bool> OnUpdateScore;
    event Action<IActor> OnEated;
    event Action<IActor> OnKilled;
    int CurrentScore { get; }
    int CurrentLevel { get; }
    string name { get; set; }
    Transform transform { get; }
    Color color { get; }
    StatsContainer Stats { get; }
    void ResetScore(bool forceEatEvent);
    bool Invincble { get; set; }
    void Kill();
}

public static class UpdateScoreUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void UpdateScore(ref int targetValue, in int amount, in Operation operation, float multipilerPositive = 1)
    {
        switch (operation)
        {
            case Operation.Summary:
                targetValue += Mathf.RoundToInt(amount * multipilerPositive);
                return;
            case Operation.Substract:
                targetValue -= amount;
                return;
            case Operation.Multiplicate:
                targetValue *= Mathf.RoundToInt(amount * multipilerPositive);
                return;
            case Operation.Divided:
                targetValue /= amount;
                return;
            case Operation.Flat:
                targetValue += amount;
                return;
        }
    }
}
