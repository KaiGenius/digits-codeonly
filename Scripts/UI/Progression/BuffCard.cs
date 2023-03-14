using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private LevelProgressionSO levelSO;
    [SerializeField] private BuffCardBackgroundSO backgroundSO;
    [SerializeField] private TextMeshProUGUI namePrinter, desciprionPrinter, costPrinter;
    [SerializeField] private Image bgImage, iconImage;
    [SerializeField] private LevelView levelView;
    private bool blockInput = true;
    private Material sharedMaterial;

    private BaseBuff buff;

    public event Action<BuffCard, BaseBuff> OnSelectCard;

    public void Initialize(BaseBuff buff, float showDelay, int currency)
    {
        blockInput = true;
        this.buff = buff;

        sharedMaterial = new Material(bgImage.material);
        bgImage.material = sharedMaterial;
        iconImage.material = sharedMaterial;

        UpdateUI(currency);
        DoShowAnim(showDelay);
    }

    public void UpdateUI(int currency)
    {
        int cost = int.MaxValue;
        if(!buff.IsMax)
            cost = levelSO.GetCost(buff.CurrentLevel + 1);
        if (currency >= cost)
            UpdateGrayScale(0);
        else
            UpdateGrayScale(1);

        namePrinter.text = buff.name;
        desciprionPrinter.text = buff.description;
        var data = backgroundSO.Select(buff.statName);
        bgImage.sprite = data.background;
        iconImage.sprite = data.icon;
        levelView.SetLevel(buff.CurrentLevel);
        if (!buff.IsMax)
            costPrinter.text = cost.ToString();
        else
            costPrinter.text = "MAX";
    }

    public void DoShowAnim(float delay)
    {
        transform.localScale = new Vector3(0, 0, 0);
        Observable.Timer(delay.sec()).TakeUntilDestroy(gameObject).Subscribe(_ => 
        {
            transform.DOScale(1, 0.2f).OnComplete(() => blockInput = false);
        });
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(!blockInput)
            OnSelectCard?.Invoke(this, buff);
    }

    private void UpdateGrayScale(float amount)
    {
        amount = Mathf.Clamp01(amount);
        sharedMaterial.SetFloat("_EffectAmount", amount);
    }
}
