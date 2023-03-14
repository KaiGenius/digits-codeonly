using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UpdateScorePopupAnim : MonoBehaviour
{
    public bool recalculateAutoCurve = true;
    public AnimationCurve curve;
    public float scaleStepStrengthUp = 1.5f, scaleStepStrengthDown = 1.5f; 
    public ScaleFormulaSO scaleFormula;
    public Transform anim;
    private IActor actor;
    private int oldScore;
    private bool isActiveAnim = true;
    private Sequence s;

    private void Start()
    {
        Observable.NextFrame().Subscribe(_ => { 
        actor = GetComponent<IActor>();
        actor.OnUpdateScore += Actor_OnUpdateScore;
        oldScore = actor.CurrentScore;
        });
        GetComponent<EXPProvider>().OnUpdateLevel += UpdateScorePopupAnim_OnUpdateLevel;

        if (recalculateAutoCurve)
        {
            curve.MoveKey(0, new Keyframe(0, 1));
            curve.MoveKey(1, new Keyframe(1, scaleFormula.limitScale / scaleFormula.baseScale));
            curve.SmoothTangents(0, 100);
            curve.SmoothTangents(1, 100);
        }
    }

    private void UpdateScorePopupAnim_OnUpdateLevel(EXPProvider arg1, int arg2)
    {
        s?.Kill();
        float newScale = scaleFormula.CalculateScale(arg1.currentLevel);
        s = DOTween.Sequence();
        s.Append(anim.DOScale(newScale + scaleStepStrengthUp, 0.25f));
        s.Append(anim.DOScale(newScale, 0.25f));
        oldScore = actor.CurrentScore;
    }

    private void Actor_OnUpdateScore(IActor obj, bool isTriggerAnim)
    {
        if (!isActiveAnim)
            return;

        if (obj.CurrentScore < 1)
            return;

        var curNumber = obj.CurrentScore;
        var delta = curNumber - oldScore;
        if (isTriggerAnim)
        {
            oldScore = obj.CurrentLevel; //temp
            s?.Kill();
            if (delta > 0)
                Popup(obj.CurrentLevel);
            else if (delta < 0)
                Popdown(obj.CurrentLevel);
        }
        oldScore = curNumber;
    }

    private void Popup(int curNumber)
    {
        float oldScale = scaleFormula.CalculateScale(oldScore);
        float newScale = scaleFormula.CalculateScale(curNumber);
        //anim.localScale = new Vector3(oldScale, oldScale, oldScale);
        s = DOTween.Sequence();
        s.Append(anim.DOScale(newScale + scaleStepStrengthUp * curve.Evaluate(scaleFormula.ClampedScale(curNumber)), 0.25f));
        s.Append(anim.DOScale(newScale, 0.25f));
    }

    private void Popdown(int curNumber)
    {
        float oldScale = scaleFormula.CalculateScale(oldScore);
        float newScale = scaleFormula.CalculateScale(curNumber);
        //anim.localScale = new Vector3(oldScale, oldScale, oldScale);
        s = DOTween.Sequence();
        s.Append(anim.DOScale(newScale - scaleStepStrengthDown * curve.Evaluate(scaleFormula.ClampedScale(curNumber)), 0.25f));
        s.Append(anim.DOScale(newScale, 0.25f));
    }

    public void SetActiveAnim(bool val) => isActiveAnim = val;
}
