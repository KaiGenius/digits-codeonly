using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : UnitySingleton<GameManager>
{
    [SerializeField] private float timeFromEndGameToReload = 5f;

    private List<IActor> registredActors = new List<IActor>();
    private List<ActorData> eatedActorsData = new List<ActorData>();

    public event Action<ActorData[]> OnEndGame;
    private static bool isGameEnded = false, isGameReload = false;
    public static bool IsGamePaused { get; set; }
    public static bool IsGameEnded => isGameEnded;
    public static bool IsGameReload => isGameReload;
    private void Start()
    {
        IsGamePaused = false;
        isGameReload = false;
        isGameEnded = false;
        GameTimer.Self.OnTimeEnd += Self_OnTimeEnd;
    }

    private void Self_OnTimeEnd()
    {
        ForceEndGameEvent();
    }

    public IEnumerable<IActor> EnumerateAllAlive()
    {
        for(int i =0;i<registredActors.Count;i++)
        {
            if ((UnityEngine.Object)registredActors[i] != null)
                yield return registredActors[i];
        }
    }

    public void RegisterNew(IActor actor)
    {
        registredActors.Add(actor);
        DynamicLeaderboard.Self.RegisterNew(actor);

        actor.transform.GetComponent<EXPProvider>().OnUpdateLevel += GameManager_OnUpdateLevel;
        if(actor is Player)
        {
            actor.OnKilled += Player_OnEated;
        }
    }

    private void GameManager_OnUpdateLevel(EXPProvider arg1, int currentLevel)
    {
        //if(currentLevel >= arg1.maxLevel)
        //{
        //    ForceEndGameEvent();
        //}
    }

    private void Player_OnEated(IActor obj)
    {
        obj.transform.gameObject.SetActive(false);
        ForceEndGameEvent();
    }

    private void Actor_OnEated(IActor obj)
    {
        eatedActorsData.Add(new ActorData(obj, true));
        registredActors.Remove(obj);

        if (registredActors.Count is 1)
            ForceEndGameEvent();
    }

    public readonly struct ActorData
    {
        public readonly bool isEated;
        public readonly string name;
        public readonly int score;
        public readonly int level;
        public readonly Color color;
        private readonly Score _score;

        public ActorData(IActor actor, bool isEated)
        {
            name = actor.name;
            score = actor.CurrentLevel;
            color = actor.color;
            level = actor.CurrentLevel;
            this.isEated = isEated;
            _score = new Score(actor);
        }

        public Score GetScore() => _score;
    }

    private void ForceEndGameEvent()
    {
        if (isGameEnded)
            return;

        isGameEnded = true;
        IsGamePaused = true;

        eatedActorsData.Sort((x, y) => //while only eated
        {
            if (x.GetScore() > y.GetScore())
                return -1;
            else if (x.GetScore() < y.GetScore())
                return 1;
            else return 0;
        });

        registredActors.Sort((x, y) => //reverse list
        {
            if (new Score(x) > new Score(y))
                return 1;
            else if (new Score(x) < new Score(y))
                return -1;
            else return 0;
        });

        foreach (var act in registredActors)
        {
            eatedActorsData.Insert(0, new ActorData(act, false));
        }

        OnEndGame?.Invoke(eatedActorsData.ToArray());
        OnEndGame = null;

        if (timeFromEndGameToReload > 0)
            Observable.Timer(timeFromEndGameToReload.sec()).Subscribe(_ =>
            {
                ReloadGame();
            });
        //else if (timeFromEndGameToReload == 0)
        //    ReloadGame();
    }

    public void ReloadGame()
    {
        if (IsGameReload)
            return;
        IsGamePaused = false;
        isGameReload = true;
        Observable.NextFrame().Subscribe(_ => 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }
}
