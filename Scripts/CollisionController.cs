using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    [SerializeField] private bool isSingleImmortal = false;
    [SerializeField] private float setScorePercent = 0.5f;
    [SerializeField] private AnimationCurve pumpPercentPerSecondCurve;
    [SerializeField] private UpdateScorePopupAnim popupAnim;
    private List<IActor> collidedActors = new List<IActor>();
    private List<IActor> clearList = new List<IActor>();
    private Dictionary<IActor, float> mapTime = new Dictionary<IActor, float>();

    private IActor selfActor;
    protected IActor SelfActor => selfActor;
    public event Action OnKill;
    public event Action<IActor> OnLostSingleImmortal;
    private float sumMult = 1;
    private float curveLength = 0;

    public void SetSingleImmortal() => isSingleImmortal = true;

    protected void Start()
    {
        selfActor = GetComponent<IActor>();
        if (selfActor == null)
            Debug.LogError("Not found IActor instance!");
        var stat = selfActor.Stats[Characters.StatName.OnEat_ConsumeScorePercent];
        stat.OnChangeValue += Stat_OnChangeValue;
        Stat_OnChangeValue(stat);

        var keys = pumpPercentPerSecondCurve.keys;
        curveLength = 0;
        foreach(var k in keys)
        {
            if (k.time > curveLength)
                curveLength = k.time;
        }
    }

    private void Stat_OnChangeValue(Characters.Stat obj)
    {
        sumMult = obj.Value;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (this == null || !gameObject.activeInHierarchy) //is destroying
            return;

        if(collision.attachedRigidbody != null && collision.attachedRigidbody.TryGetComponent<IActor>(out var actor))
        {
            HanldeCollision(actor);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (this == null || !gameObject.activeInHierarchy) //is destroying
            return;

        if (collision.attachedRigidbody != null && collision.attachedRigidbody.TryGetComponent<IActor>(out var actor))
        {
            collidedActors.Remove(actor);
            mapTime.Remove(actor);
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGamePaused)
            return;

        if (collidedActors.Count < 1)
        {
            popupAnim.SetActiveAnim(true);
            return;
        }

        popupAnim.SetActiveAnim(false);

        int curScore = selfActor.CurrentScore;
        int addScore = 0;
        foreach(var actor in collidedActors)
        {
            var deltaTime = Time.time - mapTime[actor];
            deltaTime = Mathf.Clamp(deltaTime, 0, curveLength);
            var removeScore = Mathf.RoundToInt(actor.CurrentScore * Time.fixedDeltaTime * pumpPercentPerSecondCurve.Evaluate(deltaTime));
            if (removeScore < 1)
                removeScore = 1;
            if (removeScore > actor.CurrentScore)
                removeScore = actor.CurrentScore;

            actor.UpdateScore(-removeScore, Operation.Flat, true, false);
            addScore += removeScore;
            if(actor.CurrentScore <= 1)
            {
                OnKill?.Invoke();
                actor.Kill();
                clearList.Add(actor);
            }
        }

        if(addScore>0)
            selfActor.UpdateScore(Mathf.RoundToInt(addScore * sumMult), Operation.Flat, true, false);

        if(clearList.Count > 0)
        {
            foreach(var toDelete in clearList)
            {
                collidedActors.Remove(toDelete);
                mapTime.Remove(toDelete);
            }
            clearList.Clear();
        }
    }

    protected virtual void HanldeCollision(IActor other)
    {
        if (other.Invincble)
            return;

        bool isValidCollision = false;
        if (selfActor.CurrentLevel > other.CurrentLevel)
            isValidCollision = true;
        else if (selfActor.CurrentLevel == other.CurrentLevel && selfActor.CurrentScore > other.CurrentScore)
            isValidCollision = true;

        if (isValidCollision)
        {
            //selfActor.UpdateScore(Mathf.RoundToInt(other.CurrentScore * sumMult), Operation.Summary, true);

            if (other.transform.TryGetComponent<CollisionController>(out var otherController) && otherController.isSingleImmortal)
            {
                otherController.isSingleImmortal = false;
                other.UpdateScore(-Mathf.RoundToInt(other.CurrentScore * setScorePercent), Operation.Flat, true, true);
                otherController.OnLostSingleImmortal?.Invoke(other);
                return;
            }
            else
            {
                if (!collidedActors.Contains(other))
                {
                    collidedActors.Add(other);
                    mapTime.Add(other, Time.time);
                }
            }
        }
    }
}

public class BotCollisionController : CollisionController
{
    [SerializeField] private BotData botData;
    [SerializeField] private Bot bot;

    protected override void HanldeCollision(IActor other)
    {
        if (!bot.InCameraView() && !botData.inHunterMode)
            return;

        base.HanldeCollision(other);
    }
}
