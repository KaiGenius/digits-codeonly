using Core;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpWindow : UnitySingleton<LevelUpWindow>
{
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void SelfClose()
    {
        gameObject.SetActive(false);
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.2f);
    }

    public void ShowUpWindow()
    {
        gameObject.SetActive(true);
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.2f);
    }
}
