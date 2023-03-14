using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpController : MonoBehaviour
{
    [SerializeField] private AnimationCurve pumpPercentPerSecondCurve;
    [SerializeField] private ScaleFormulaSO scaleFormula;

    private IActor selfActor;
    private UpdateScorePopupAnim popupAnim;

    private List<IActor> collidedActors = new List<IActor>();
    private List<IActor> clearList = new List<IActor>();
    private Dictionary<IActor, Timer> mapTime = new Dictionary<IActor, Timer>();

    private float curveLength = 0;

    private void Start()
    {
        selfActor = GetComponentInParent<IActor>();
        popupAnim = selfActor.transform.GetComponent<UpdateScorePopupAnim>();

        var keys = pumpPercentPerSecondCurve.keys;
        curveLength = 0;
        foreach (var k in keys)
        {
            if (k.time > curveLength)
                curveLength = k.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 9 && other.attachedRigidbody.TryGetComponent<IActor>(out var actor))
        {
            if (selfActor != actor)
                HandleCollision(actor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9 && other.attachedRigidbody.TryGetComponent<IActor>(out var actor))
        {
            RemoveRegistry(actor);
        }
    }

    private void HandleCollision(IActor other)
    {
        bool isValidCollision = false;
        if (selfActor.CurrentLevel > other.CurrentLevel)
            isValidCollision = true;
        else if (selfActor.CurrentLevel == other.CurrentLevel && selfActor.CurrentScore > other.CurrentScore)
            isValidCollision = true;

        if (isValidCollision)
        {
            AddRegistry(other);
        }
    }

    private void AddRegistry(IActor actor)
    {
        if (!collidedActors.Contains(actor))
        {
            collidedActors.Add(actor);
            if(mapTime.TryGetValue(actor, out var timer))
            {
                if(timer.ElapsedFromLastTick > 1f)
                {
                    mapTime[actor] = new Timer();
                }
            }
            else
                mapTime.Add(actor, new Timer());
            popupAnim.SetActiveAnim(false);
        }
    }

    private void RemoveRegistry(IActor actor)
    {
        if(collidedActors.Remove(actor))
        {
            //mapTime.Remove(actor);
        }

        if(collidedActors.Count is 0)
            popupAnim.SetActiveAnim(true);
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGamePaused)
            return;

        if (collidedActors.Count < 1)
            return;

        int curScore = selfActor.CurrentScore;
        int addScore = 0;
        foreach (var actor in collidedActors)
        {
            var deltaTime = mapTime[actor].Tick(Time.fixedDeltaTime);
            deltaTime = Mathf.Clamp(deltaTime, 0, curveLength);
            var removeScore = Mathf.RoundToInt(actor.CurrentScore * Time.fixedDeltaTime * pumpPercentPerSecondCurve.Evaluate(deltaTime));
            if (removeScore < 1)
                removeScore = 1;
            if (removeScore > actor.CurrentScore)
                removeScore = actor.CurrentScore;

            actor.UpdateScore(-removeScore, Operation.Flat, true, false);
            addScore += removeScore;
            if (actor.CurrentScore <= 1)
            {
                //OnKill?.Invoke();
                actor.Kill();
                clearList.Add(actor);
            }
        }

        if (addScore > 0)
            selfActor.UpdateScore(Mathf.RoundToInt(addScore * 1), Operation.Flat, true, false);

        if (clearList.Count > 0)
        {
            foreach (var toDelete in clearList)
            {
                RemoveRegistry(toDelete);
            }
            clearList.Clear();
        }
    }

    private class Timer
    {
        public readonly float initialTime;
        private float currentTime;
        private float timeOfLastTick;

        public float Elapsed => currentTime - initialTime;
        public float ElapsedFromLastTick => GameTimer.Time - timeOfLastTick;

        public float Tick(float deltaTime)
        {
            currentTime += deltaTime;
            timeOfLastTick = GameTimer.Time;
            return Elapsed;
        }

        public Timer()
        {
            initialTime = GameTimer.Time;
            currentTime = initialTime;
            timeOfLastTick = initialTime;
        }
    }
}
