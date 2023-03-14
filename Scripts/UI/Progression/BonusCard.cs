using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BonusCard : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField] private BuffCardBackgroundSO backgroundSO;
    [SerializeField] private TextMeshProUGUI namePrinter, desciprionPrinter;
    [SerializeField] private Image bgImage;
    //[SerializeField] private LevelView levelView;

    private BaseBonus bonus;

    public event Action<BonusCard, BaseBonus> OnSelectCard;

    public void Initialize(BaseBonus buff)
    {
        this.bonus = buff;
        UpdateUI();
    }

    public void UpdateUI()
    {
        namePrinter.text = bonus.Name;
        desciprionPrinter.text = bonus.Description;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        OnSelectCard?.Invoke(this, bonus);
    }
}
