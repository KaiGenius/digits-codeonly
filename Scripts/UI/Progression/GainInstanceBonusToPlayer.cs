using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using UniRx;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;

public class GainInstanceBonusToPlayer : MonoBehaviour, IClosable
{
    public int maxGainedBonuses = 3;
    [SerializeField] private Transform container;
    [SerializeField] private BonusCard cardPrefab;

    private List<BaseBonus> bonuses = new List<BaseBonus>(8);
    private List<BonusCard> cardInstances = new List<BonusCard>();
    private Player player;

    public int LevelIncrease => 0;

    public void Call(Player player)
    {
        this.player = player;
        FillRandomBonuses();

        foreach (var bonus in bonuses)
        {
            var cardInstance = Instantiate(cardPrefab, container);
            cardInstance.Initialize(bonus);
            cardInstance.OnSelectCard += CardInstance_OnSelectCard;
            cardInstances.Add(cardInstance);
        }
    }

    private void FillRandomBonuses()
    {
        bonuses.Clear();
        List<int> indexes = new List<int>() { 0, 1, 2, 3, 4 };
        for(int i =0; i < 5 - maxGainedBonuses; i++)
            indexes.RemoveAt(Random.Range(0, indexes.Count));

        for(int i =0;i<maxGainedBonuses;i++)
        {
            switch(indexes[i])
            {
                case 0:
                    bonuses.Add(new BonusScoreUp());
                    break;
                case 1:
                    bonuses.Add(new BonusMultScore());
                    break;
                case 2:
                    bonuses.Add(new BonusSpeedUp());
                    break;
                case 3:
                    bonuses.Add(new BonusInvincble());
                    break; 
                case 4:
                    bonuses.Add(new BonusSingleSafe());
                    break;
            }
        }
    }

    public void Close()
    {
        foreach (var c in cardInstances)
        {
            Destroy(c.gameObject);
        }
        cardInstances.Clear();

        gameObject.SetActive(false);
    }

    private void CardInstance_OnSelectCard(BonusCard selectedCard, BaseBonus selectedBonus)
    {
        foreach (var card in cardInstances)
        {
            card.OnSelectCard -= CardInstance_OnSelectCard;
        }

        selectedCard.UpdateUI();
        var s = DOTween.Sequence();
        s.Append(selectedCard.transform.DOScale(1.2f, 0.5f));
        s.Append(selectedCard.transform.DOScale(1f, 0.5f));
        s.OnComplete(() => 
        {
            Observable.NextFrame().Subscribe(_ => selectedBonus.ApplyTo(player));
            Close();
        });
    }

}

public abstract class BaseBonus
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract void ApplyTo(IActor actor);
}

public class BonusScoreUp : BaseBonus
{
    public override string Name => "Score Up";
    public override string Description => "Gain +100-500 score points";

    public override void ApplyTo(IActor actor)
    {
        var scoreUp = Random.Range(100, 501);
        actor.UpdateScore(scoreUp, Operation.Summary, true, false);
    }
}

public class BonusMultScore : BaseBonus
{
    public override string Name => "Mult Score";
    public override string Description => "Gain x2-5 mult to current score points";

    public override void ApplyTo(IActor actor)
    {
        var scoreUp = Random.Range(2, 6);
        actor.UpdateScore(scoreUp, Operation.Multiplicate, true, false);
    }
}

public class BonusSpeedUp : BaseBonus
{
    public override string Name => "Speed Up";
    public override string Description => "Gain +100% speed on next 15 seconds";
    private Stat target;

    public override void ApplyTo(IActor actor)
    {
        var stat = actor.Stats[StatName.Speed];
        stat.Multipiler += 1f;
        target = stat;

        Observable.Timer(15f.sec()).Subscribe(_ => target.Multipiler -= 1f);
    }
}

public class BonusInvincble : BaseBonus
{
    public override string Name => "Invincble";
    public override string Description => "Gain invincble on next 15 seconds";
    private IActor target;
    public override void ApplyTo(IActor actor)
    {
        target = actor;
        target.Invincble = true;
        Observable.Timer(15f.sec()).Subscribe(_ => target.Invincble = false);
    }
}

public class BonusSingleSafe : BaseBonus
{
    public override string Name => "Single Safe";

    public override string Description => "1 safe from eating, safe 50% of score";

    public override void ApplyTo(IActor actor)
    {
        var colController = actor.transform.GetComponent<CollisionController>();
        colController.SetSingleImmortal();
        colController.OnLostSingleImmortal += ColController_OnLostSingleImmortal;
    }

    private void ColController_OnLostSingleImmortal(IActor actor)
    {
        var colController = actor.transform.GetComponent<CollisionController>();
        colController.OnLostSingleImmortal -= ColController_OnLostSingleImmortal;
        actor.Invincble = true;
        actor.transform.GetComponent<ActorNumberViewBase>().SetFlashing(3f);
        Observable.Timer(15f.sec()).Subscribe(_ => actor.Invincble = false);
    }
}
