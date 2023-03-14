using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BotExpProvider : EXPProvider
{
    [SerializeField] private float intervalToRaiseLevel = 10f;
    [SerializeField, Range(0f,1f)] private float chanseToRiseLevel = 0.5f;
    [SerializeField] private LevelProgressionSO levelSO;
    private List<BaseBuff> selectedBuffs = new List<BaseBuff>();
    private Bot selfBody;

    protected override void OnUpdateScore(IActor actor)
    {
        if (actor.CurrentScore >= requireScoreToNextLevel)
        {
            var lvlProgression = actor.transform.GetComponent<LevelProgressionProvider>();
            lvlProgression.SelectValidRandomBuffs(selectedBuffs, 3);
            var select = SelectRandomValidBuff(actor.CurrentScore);

            Observable.NextFrame().Subscribe(_ => {
                if (select != null)
                {
                    actor.UpdateScore(-levelSO.GetCost(select.CurrentLevel + 1), Operation.Flat, true, false);
                    select.IncreaseLevel(actor.Stats);
                    Self_OnCloseCallback(1);
                }
                else
                    Self_OnCloseCallback(0);
            });
        }
    }

    public void DecreaseLevel(int amount)
    {
        var levelProg = GetComponent<LevelProgressionProvider>();
        amount = levelProg.DecreaseLevel(amount);
        Self_OnCloseCallback(-amount);
    }

    private BaseBuff SelectRandomValidBuff(int currentScore)
    {
        BaseBuff selected = null;

        foreach(var b in selectedBuffs)
        {
            var cost = levelSO.GetCost(b.CurrentLevel + 1);
            if(!b.IsMax && cost <= currentScore)
            {
                selected = b;
                if (UnityEngine.Random.value >= 0.5f)
                    return selected;
            }
        }

        return selected;
    }

    protected override void Start()
    {
        base.Start();
        selfBody = GetComponent<Bot>();
        Observable.Interval(intervalToRaiseLevel.sec()).TakeUntilDestroy(gameObject).Subscribe(_ => 
        {
            if (!selfBody.InCameraView() && Random.value < chanseToRiseLevel)
            {
                var requireScore = requireScoreToNextLevel - actor.CurrentScore;
                actor.UpdateScore(requireScore, Operation.Flat, true, false);
            }
        });
    }
}

