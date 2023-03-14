using System;
using UniRx;
using UnityEngine;

public abstract class ActorNumberViewBase : MonoBehaviour, IBoundsProvider
{
    public Vector3 triggerExpandSize = new Vector3(0.15f, 0.15f, 4f);
    public float deltaToExpandCollider = 0.5f;
    public float xStrength = 0.25f;
    [SerializeField] private Transform container;
    [SerializeField] private DigitView view;
    [SerializeField] private BoxCollider collider, dublicatedCollider, triggerCollider;
    [SerializeField] private ScaleFormulaSO scaleFormula;
    private int curLevel;

    private IDisposable observer;
    private Color selectedColor;

    public Color Color => selectedColor;

    public Bounds Bounds => collider.bounds;

    protected virtual void Start()
    {
        selectedColor = ActorColorizer.Self.GetColor();
        view.UpdateMaterial(selectedColor);
       
        Observable.NextFrame().Subscribe(_ => 
        {
            if (TryGetComponent<IActor>(out var actor))
            {
                GameManager.Self.RegisterNew(actor);
                curLevel = actor.CurrentLevel;
                var size = scaleFormula.CalculateScale(actor.CurrentLevel);
                container.localScale = Vector3.one * size;
            }
        });
    }

    protected void Source_OnUpdateScore(IActor actor, bool _)
    {
        var size = scaleFormula.CalculateScale(actor.CurrentLevel);
        //container.localScale = Vector3.one * size;
        view.SetNumber(actor.CurrentScore.ToString());
        var bounds = view.Bounds;
        var newSize = bounds.size * size;
        var level = actor.CurrentLevel;
        collider.center = default;
        collider.size = newSize;
        view.transform.localPosition = -bounds.center;

        newSize = collider.size + new Vector3(0,0,4);
        newSize.x *= xStrength;
        if(RequireExpand(dublicatedCollider.size, newSize))
        {
            dublicatedCollider.size = newSize;
        }

        newSize = collider.size + triggerExpandSize * size;
        triggerCollider.size = newSize;
        triggerCollider.center = default;
        //newSize = collider.size + new Vector3(0.15f, 0.15f, 4);

        //var magDelta = newSize.magnitude - dublicatedCollider.size.magnitude;
        //if (level > curLevel || magDelta > 0f || magDelta <= -0.5f)
        //{
        //    dublicatedCollider.size = newSize;
        //    dublicatedCollider.center = default;
        //    curLevel = level;
        //}

        
    }
    protected virtual bool RequireExpand(Vector3 baseSize, Vector3 requireSize)
    {
        if (Mathf.Abs(baseSize.x - requireSize.x) >= deltaToExpandCollider)
            return true;
        else if (Mathf.Abs(baseSize.y - requireSize.y) >= deltaToExpandCollider)
            return true;
        else if (Mathf.Abs(baseSize.z - requireSize.z) >= deltaToExpandCollider)
            return true;
        else return false;
    }

    public void SetFlashing(float time)
    {
        view.IsRendering = true;
        observer?.Dispose();

        observer = Observable.Interval(0.1f.sec()).TakeUntil(Observable.Timer(time.sec())).TakeUntilDestroy(gameObject).Finally(() => 
        {
            if(this!=null)
            {
                view.IsRendering = true;
            }
        }).Subscribe(_ => 
        {
            view.IsRendering = !view.IsRendering;
        });
    }
}
