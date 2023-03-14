using AI;
using StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAI : MonoBehaviour
{
    [SerializeField] private BotData data;
    private StateMachine<BotData> stateMachine;

    private void Start()
    {
        stateMachine = new StateMachine<BotData>(data);

        stateMachine.AddNewState(new BotAIState_Enter());
        stateMachine.AddNewState(new BotAIState_Exit());
        stateMachine.AddNewState(new BotAIState_Idle());
        stateMachine.AddNewState(new BotAIState_SeekNumber());
        stateMachine.AddNewState(new BotAIState_GoToTarget());
        stateMachine.AddNewState(new BotAIState_PatternSelector());
        stateMachine.AddNewState(new BotAIState_EnemyHunting());
        stateMachine.AddNewState(new BotAIState_RunFromHunter());

        stateMachine.SwitchTo(nameof(BotAIState_Enter));

        GameManager.Self.OnEndGame += Self_OnEndGame;
        GameTimer.Self.OnTimeEnd += Self_OnTimeEnd;
    }

    private void Self_OnTimeEnd()
    {
        stateMachine.Stop();
        enabled = false;
    }

    private void Self_OnEndGame(GameManager.ActorData[] obj)
    {
        stateMachine.Stop();
        enabled = false;
        data.rigidbody.velocity = default;
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGamePaused)
        {
            data.rigidbody.velocity = default;
            return;
        }

        stateMachine.Update();
    }

    private void OnDestroy()
    {
        stateMachine.Stop();
        if (!GameManager.IsGameReload)
        {
            GameTimer.Self.OnTimeEnd -= Self_OnTimeEnd;
        }
        if (!GameManager.IsGameEnded)
        {
            GameManager.Self.OnEndGame -= Self_OnEndGame;
           
        }
    }
}
