using UnityEngine;

public class BotNumberView : ActorNumberViewBase
{
    [SerializeField] private Bot source;

    protected override void Start()
    {
        base.Start();
        source.OnUpdateScore += Source_OnUpdateScore;
        Source_OnUpdateScore(source, false);
    }
}
