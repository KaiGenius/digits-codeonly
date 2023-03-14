using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNumberView : ActorNumberViewBase
{
    [SerializeField] private Player source;
    [SerializeField] private PlayerScoreUIView otherView;

    protected override void Start()
    {
        base.Start();
        source.OnUpdateScore += Source_OnUpdateScore;
        source.OnUpdateScore += Source_OnUpdateScore1;
        Source_OnUpdateScore(source, false);
        otherView?.UpdateMaterial(Color);
    }

    private void Source_OnUpdateScore1(IActor arg1, bool arg2)
    {
        otherView?.UpdateView(arg1.CurrentScore);
    }
}
