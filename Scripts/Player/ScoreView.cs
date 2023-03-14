using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI printer;

    private void Start()
    {
        player.OnUpdateScore += Player_OnUpdateScore;
        SetScore(player.currentScore);
    }

    private void Player_OnUpdateScore(IActor obj, bool _)
    {
        SetScore(obj.CurrentScore);
    }

    private void SetScore(int score)
    {
        printer.text = $"Score: {score}";
    }
}
