using Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Bot : MonoBehaviour, IActor
{
    public bool negateNumbersWorkOnlyInCameraView = true;
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

    private Plane[] frustrumPlanes = new Plane[6];
    private bool unableChangeScore = true;
    public bool Invincble { get; set; }
    void Awake()
    {
        stats = new StatsContainer(statsData);
    }
    private void Start()
    {
        Observable.Timer(0.5f.sec()).Subscribe(_ => unableChangeScore = false);
    }

    public void Kill() => OnKilled?.Invoke(this);
    public void UpdateScore(int value, Operation operation, bool isImportant = false, bool isTriggerAnim = true)
    {
        if (unableChangeScore)
            return;

        int oldScore = currentScore;

        if (operation == Operation.Substract)
        {
            var negateValue = stats[StatName.Resistance_NegateNumbers].ValueAsInt();
            value -= negateValue;
            if (value < 1)
                value = 1;
        }

        UpdateScoreUtils.UpdateScore(ref currentScore, value, operation, stats[StatName.Additional_PositiveNumbers].Value);

        if (IsPositiveOperation(operation))
        {
            currentScore += stats[StatName.Additional_PositiveNumbersBonus].ValueAsInt();
        }

        if (!isImportant && oldScore > currentScore && !InCameraView())
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

    private void OnDisable()
    {
        if (!GameManager.IsGameReload)
        {
            OnEated?.Invoke(this);
        }
    }

    public bool InCameraView()
    {
        if (negateNumbersWorkOnlyInCameraView)
        {
            GeometryUtility.CalculateFrustumPlanes(Camera.main, frustrumPlanes);
            var bounds = view.Bounds;
            bounds.center = transform.position;
            return GeometryUtility.TestPlanesAABB(frustrumPlanes, bounds);
        }
        else
            return true;
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
