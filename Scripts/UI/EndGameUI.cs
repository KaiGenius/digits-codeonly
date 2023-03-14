using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private EndGameRoute loseRoute, winRoute;
    [SerializeField] private LeaderboardPosition lbPrefab;
    [SerializeField] private Transform lbContainer;
    private Vector3 showPos;
    private void Start()
    {
        GameManager.Self.OnEndGame += Self_OnEndGame;
        gameObject.SetActive(false);
        showPos = transform.localPosition;
        transform.localPosition += new Vector3(0, -Screen.height, 0);
    }

    private void Self_OnEndGame(GameManager.ActorData[] obj)
    {
        gameObject.SetActive(true);
        ShowAnim();

        if(IsPlayerWin(obj))
        {
            winRoute.Activate();
        }
        else
        {
            loseRoute.Activate();
        }

        SetLeaderboard(obj);
    }

    private void ShowAnim()
    {
        transform.DOLocalMove(showPos, 1f);
    }

    private bool IsPlayerWin(GameManager.ActorData[] data)
    {
        return data[0].name == Player.ActiveInstance.name;
    }

    public void ReloadGame() => GameManager.Self.ReloadGame();

    private void SetLeaderboard(GameManager.ActorData[] data)
    {
        int pos = 1;
        foreach(var dt in data)
        {
            var lbInstance = Instantiate(lbPrefab, lbContainer);
            lbInstance.actorName = dt.name;
            lbInstance.actorPosition = pos++;
            lbInstance.actorScore = dt.GetScore();
            lbInstance.SetColor(dt.color);

            if (dt.isEated || dt.score < 1)
                lbInstance.SetStrikethrough();
            lbInstance.UpdateText();

            if(dt.name == Player.ActiveInstance.name)
            {
                lbInstance.transform.localScale *= 1.2f;
            }
        }
    }
}
