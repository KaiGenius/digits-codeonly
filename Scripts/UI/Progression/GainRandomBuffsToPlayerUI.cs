using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GainRandomBuffsToPlayerUI : MonoBehaviour, IClosable
{
    public int maxGainedCards = 3;
    public Action<int> installLevelUpDelegate;
    public bool disableButtons = true;
    [SerializeField] private GameObject toOpen;
    [SerializeField] private LevelProgressionSO levelSO;
    [SerializeField] private Transform cardHolder;
    [SerializeField] private BuffCard cardPrefab;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button rerollButton;
    [SerializeField] private TextMeshProUGUI rerollTxt, playerLvlTxt, nextWindowTxt;
    [SerializeField] private int baseRerollSum = 200;

    private List<BaseBuff> buffs = new List<BaseBuff>(8);
    private List<BuffCard> cardInstances = new List<BuffCard>();
    private Player player;
    private int levelIncrease = 0;
    private int currentRerollSum;


    public int LevelIncrease => levelIncrease;

    public void Call(Player player)
    {
        currentRerollSum = baseRerollSum;
        levelIncrease = 0;

        toOpen.SetActive(true);
        this.player = player;
        CheckRerollShowCondition();
        var levelProgression = player.GetComponent<LevelProgressionProvider>();
        levelProgression.SelectValidRandomBuffs(buffs, maxGainedCards);
        playerLvlTxt.text = $"{levelIncrease + player.CurrentLevel}";
        var expProvider = player.GetComponent<EXPProvider>();
        nextWindowTxt.text = $"NEXT AT {expProvider.requireScoreToNextLevel}";

        float delay = 0.5f;
        foreach(var buff in buffs)
        {
            var cardInstance = Instantiate(cardPrefab, cardHolder);
            cardInstance.Initialize(buff, delay += 0.2f, player.CurrentScore);
            cardInstance.OnSelectCard += CardInstance_OnSelectCard;
            cardInstances.Add(cardInstance);
        }

        if(!CanByAnyOption(player.CurrentScore))
        {
            Close();
        }
    }

    private void CheckRerollShowCondition()
    {
        var curScore = player.CurrentScore;
        if(currentRerollSum <= curScore)
        {
            rerollTxt.text = $"REFRESH FOR {currentRerollSum}";
            rerollButton.gameObject.SetActive(!disableButtons);
        }
        else
        {
            rerollButton.gameObject.SetActive(false);
        }
    }

    public void RerollCards()
    {
        if(player.CurrentScore >= currentRerollSum)
        {
            player.UpdateScore(-currentRerollSum, Operation.Flat, true, false);
            currentRerollSum += baseRerollSum;
            CheckRerollShowCondition();
        }
        else
        {
            CheckRerollShowCondition();
            return;
        }

        foreach (var c in cardInstances)
        {
            c.OnSelectCard -= CardInstance_OnSelectCard;
            Destroy(c.gameObject);
        }
        cardInstances.Clear();

        var levelProgression = player.GetComponent<LevelProgressionProvider>();
        levelProgression.SelectValidRandomBuffs(buffs, maxGainedCards);

        float delay = 0;
        foreach (var buff in buffs)
        {
            var cardInstance = Instantiate(cardPrefab, cardHolder);
            cardInstance.Initialize(buff, delay += 0.2f, player.CurrentScore);
            cardInstance.OnSelectCard += CardInstance_OnSelectCard;
            cardInstances.Add(cardInstance);
        }

        if (!CanByAnyOption(player.CurrentScore))
            Close();
    }

    public void Close()
    {
        closeBtn.gameObject.SetActive(false);
        rerollButton.gameObject.SetActive(false);
        foreach (var c in cardInstances)
        {
            c.OnSelectCard -= CardInstance_OnSelectCard;
            Destroy(c.gameObject);
        }
        cardInstances.Clear();

        toOpen.SetActive(false);
        gameObject.SetActive(false);
    }

    private void CardInstance_OnSelectCard(BuffCard selectedCard, BaseBuff selectedBuff)
    {
        if (selectedBuff.IsMax)
        {
            selectedCard.transform.DOShakePosition(0.5f, 10, 10, 90, false, true, ShakeRandomnessMode.Harmonic);
            return;
        }

        var upCost = levelSO.GetCost(selectedBuff.CurrentLevel + 1);
        if (player.CurrentScore >= upCost)
        {
            player.UpdateScore(-upCost, Operation.Flat, true, false);
        }
        else
        {
            selectedCard.transform.DOShakePosition(0.5f, 10, 10, 90, false, true, ShakeRandomnessMode.Harmonic);
            return;
        }

        closeBtn.gameObject.SetActive(!disableButtons);
        selectedBuff.IncreaseLevel(player.Stats);
        //levelIncrease += 1;
        installLevelUpDelegate?.Invoke(1);
        playerLvlTxt.text = $"{levelIncrease + player.CurrentLevel}";
        var expProvider = player.GetComponent<EXPProvider>();
        nextWindowTxt.text = $"NEXT AT {expProvider.requireScoreToNextLevel}";
        Camera.main.GetComponent<TransformFollow>().SetTopDownCameraView(true);
        foreach (var cr in cardInstances)
            cr.UpdateUI(player.CurrentScore);
        //selectedCard.UpdateUI(player.CurrentScore);
        CheckRerollShowCondition();
        var s = DOTween.Sequence();
        s.Append(selectedCard.transform.DOScale(1.2f, 0.5f));
        s.Append(selectedCard.transform.DOScale(1f, 0.5f));
        s.OnComplete(() => 
        {
            if (!CanByAnyOption(player.CurrentScore))
                Close();
        });
    }

    private bool CanByAnyOption(int currentScore)
    {
        foreach(var b in buffs)
        {
            if (b.IsMax)
                continue;

            var cost = levelSO.GetCost(b.CurrentLevel + 1);
            if (currentScore >= cost)
                return true;
        }

        return false;
    }
}
