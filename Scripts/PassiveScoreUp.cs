using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PassiveScoreUp : MonoBehaviour
{
    private IActor actor;
    private int passiveScoreCountUp = 0;

    private void Start()
    {
        actor = GetComponent<IActor>();
        var passtat = actor.Stats[Characters.StatName.Passive_ScoreUp];
        passtat.OnChangeValue += Passtat_OnChangeValue;
        Passtat_OnChangeValue(passtat);
    }

    private void Passtat_OnChangeValue(Characters.Stat obj)
    {
        passiveScoreCountUp = obj.ValueAsInt();
    }

    private void OnEnable()
    {
        Observable.Interval(1f.sec()).TakeUntilDisable(this).Subscribe(_ => 
        {
            if (passiveScoreCountUp is 0 || GameManager.IsGamePaused)
                return;
            actor.UpdateScore(passiveScoreCountUp, Operation.Flat, isTriggerAnim: false);
        });
    }
}
