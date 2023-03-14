using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EXPProvider : MonoBehaviour
{
    public int requireScoreToNextLevel = 1000;
    public int addRequireScoreToNextLevel = 100;
    public int currentLevel = 0;
    public int maxLevel = 25;

    public event Action<EXPProvider, int> OnUpdateLevel;
    protected IActor actor;

    protected virtual void Start()
    {
        actor = GetComponent<IActor>();
        actor.OnUpdateScore += Actor_OnUpdateScore;
    }

    protected void Self_OnCloseCallback(int increaseLevelAmount)
    {
        var oldLevel = currentLevel;
        requireScoreToNextLevel += addRequireScoreToNextLevel*increaseLevelAmount;
        currentLevel += increaseLevelAmount;
        if (currentLevel > maxLevel)
            currentLevel = maxLevel;
        else if (currentLevel < 0)
            currentLevel = 0;

        if(oldLevel != currentLevel)
            OnUpdateLevel?.Invoke(this, currentLevel);
    }

    private void Actor_OnUpdateScore(IActor arg1, bool arg2)
    {
        if(currentLevel < maxLevel)
            OnUpdateScore(arg1);
    }

    protected abstract void OnUpdateScore(IActor actor);

}
