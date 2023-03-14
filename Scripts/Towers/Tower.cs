using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private ActorObservable observable;
    [SerializeField] private TowerPlateView plateView;
    [SerializeField] private DigitView numberView;

    private int _score = 0;
    private IActor currentCaptured = null;

    public bool IsCaptured => currentCaptured != null;

    public int score
    {
        get => _score;
        set
        {
            if(value != _score)
            {
                _score = value;
                numberView?.SetNumber(value.ToString());
                numberView.transform.localPosition = -(numberView.transform.localRotation * numberView.Bounds.center) + new Vector3(0,2f,0);
            }
        }
    }

    private void Start()
    {
        observable.OnEnter += Observable_OnEnter;
        score = Random.Range(10, 101);
        numberView.UpdateMaterial(Color.gray);

        TowerDispatcher.Self.Register(this);
    }

    private void Observable_OnEnter(IActor obj)
    {
        if (TryCapture(obj))
        {
            plateView.SetColor(obj.color);
        }
    }

    private bool TryCapture(IActor actor)
    {
        if(currentCaptured != actor)
        {
            if(actor.CurrentScore > score)
            {
                actor.UpdateScore(-score, Operation.Flat, true, true);
                currentCaptured = actor;
                score = 1;
                return true;
            }
        }
        else if(currentCaptured == actor && score > 1)
        {
            actor.UpdateScore(score, Operation.Flat, true, true);
            score = 1;
            return false;
        }

        return false;
    }
}
