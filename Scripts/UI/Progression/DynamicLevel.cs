using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicLevel : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI levelPrint, scorePrint;

    private int requireScore, level, currentScore;
    private bool isMaxLevel;


    private Player player;
    private PlayerExpProvider expProvider;

    private void Start()
    {
        player = Player.ActiveInstance;
        expProvider = player.GetComponent<PlayerExpProvider>();
        expProvider.OnUpdateLevel += ExpProvider_OnUpdateLevel;

        player.OnUpdateScore += Player_OnUpdateScore;
        isMaxLevel = false;
        ExpProvider_OnUpdateLevel(expProvider, 1);
        Player_OnUpdateScore(player, false);
    }

    private void ExpProvider_OnUpdateLevel(EXPProvider arg1, int arg2)
    {
        level = arg1.currentLevel;
        requireScore = arg1.requireScoreToNextLevel;
        isMaxLevel = arg1.maxLevel <= arg2;
        UpdateUI();
    }

    private void Player_OnUpdateScore(IActor arg1, bool arg2)
    {
        currentScore = arg1.CurrentScore;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (isMaxLevel)
        {
            slider.DOKill();
            slider.DOValue(1f, 0.25f);
            levelPrint.text = level.ToString();
            scorePrint.text = "maximum";
        }
        else
        {
            var percent = currentScore / (float)requireScore;
            percent = Mathf.Clamp01(percent);
            slider.DOKill();
            slider.DOValue(percent, 0.25f);
            levelPrint.text = level.ToString();
            scorePrint.text = $"{currentScore}/{requireScore}";
        }
    }
}
