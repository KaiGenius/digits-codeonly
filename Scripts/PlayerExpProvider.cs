public class PlayerExpProvider : EXPProvider
{
    protected override void Start()
    {
        base.Start();
        LevelUpUI.Self.OnCloseCallback += WrapperOnCloseCallback;
        //GetComponent<CollisionController>().OnKill += PlayerExpProvider_OnKill;
    }

    private void PlayerExpProvider_OnKill()
    {
        /*DISABLED*/

        //if (GameManager.IsGamePaused)
        //    return;

        //LevelUpUI.Self.Open(LevelUpUI.OpenState.GainTemporaryBuff);
    }

    private void WrapperOnCloseCallback(LevelUpUI.OpenState openState, int increaseLevelAmount)
    {
        actor.transform.GetComponent<UpdateScorePopupAnim>().SetActiveAnim(true);
        //if(openState == LevelUpUI.OpenState.GainPersistentBuff)
        //    Self_OnCloseCallback(increaseLevelAmount);
    }

    protected override void OnUpdateScore(IActor actor)
    {
        if (GameManager.IsGamePaused)
            return;

        if (actor.CurrentScore >= requireScoreToNextLevel)
        {
            actor.transform.GetComponent<UpdateScorePopupAnim>().SetActiveAnim(false);
            LevelUpUI.Self.InstallLevelUp(Self_OnCloseCallback);
            LevelUpUI.Self.Open(LevelUpUI.OpenState.GainPersistentBuff);
        }
    }
}

