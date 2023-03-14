using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : UnitySingleton<GameTimer>
{
    [SerializeField] private float startTimeInSeconds;
    private float time;

    public float TimeLeft => time;
    public event Action OnTimeEnd;
    public event Action<float> OnTimeChange;

    public static float Time => Self.startTimeInSeconds - _self.time;

    private void Start()
    {
        time = startTimeInSeconds;
        enabled = true;
        GameManager.Self.OnEndGame += Self_OnEndGame;
    }

    private void Self_OnEndGame(GameManager.ActorData[] obj)
    {
        //enabled = false;
    }

    private void Update()
    {
        if (GameManager.IsGamePaused)
            return;

        time -= UnityEngine.Time.smoothDeltaTime;
        if(time <=0)
        {
            time = 0;
            enabled = false;
            OnTimeEnd?.Invoke();
        }
        OnTimeChange?.Invoke(time);
    }
}
