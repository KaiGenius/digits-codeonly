using Characters;
using System;
using UniRx;
using UnityEngine;

public class Player : MonoBehaviour, IActor
{
    public static Player ActiveInstance { get; private set; }
    //public SizeLevelList sizeLevelList;
    public float currentSize { get; private set; } = 0;
    public int currentScore = 1;

    [SerializeField] private ActorNumberViewBase view;
    [SerializeField] private StatsContainerSO statsData;
    [SerializeField] private EXPProvider expProvider;
    private StatsContainer stats;

    public StatsContainer Stats => stats;
    public int CurrentScore => currentScore;
    public int CurrentLevel => expProvider.currentLevel;
    public Color color => view.Color;
    public event Action<IActor, bool> OnUpdateScore;
    public event Action<IActor> OnEated;
    public event Action<IActor> OnKilled;
    private bool unableChangeScore = true;

    public bool Invincble { get; set; }

    public void Kill() => OnKilled?.Invoke(this);

    public void UpdateScore(int value, Operation operation, bool isImportant = false, bool isTriggerAnim = true)
    {
        if (unableChangeScore)
            return;

        if(operation == Operation.Substract)
        {
            var negateValue = stats[StatName.Resistance_NegateNumbers].ValueAsInt();
            value -= negateValue;
            if (value < 1)
                value = 1;
        }

        int oldScore = currentScore;
        UpdateScoreUtils.UpdateScore(ref currentScore, value, operation, stats[StatName.Additional_PositiveNumbers].Value);

        if(IsPositiveOperation(operation))
        {
            currentScore += stats[StatName.Additional_PositiveNumbersBonus].ValueAsInt();
        }

        if(Invincble && currentScore < oldScore)
        {
            currentScore = oldScore;
            return;
        }

        if (currentScore <= 1 && oldScore > 1)
            ResetScore(true);
        else if (currentScore < 1)
            currentScore = 1;

        OnUpdateScore?.Invoke(this, isTriggerAnim);
    }

    private void Awake()
    {
        ActiveInstance = this;
        stats = new StatsContainer(statsData);
    }

    private void OnDisable()
    {
        if(!GameManager.IsGameEnded)
            OnEated?.Invoke(this);
    }

    private void Start()
    {
        Observable.Timer(0.5f.sec()).Subscribe(_ => unableChangeScore = false);
    }

    public void ResetScore(bool forceEatEventEat)
    {
        currentScore = 1;
        OnUpdateScore?.Invoke(this, false);
        if (forceEatEventEat)
            OnEated?.Invoke(this);
    }

    private bool IsPositiveOperation(Operation op)
    {
        return op == Operation.Summary || op == Operation.Multiplicate;
    }
}
