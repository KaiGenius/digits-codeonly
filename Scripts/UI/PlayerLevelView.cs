using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLevelView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI printer;

    private void Start()
    {
        var expProvider = Player.ActiveInstance.GetComponent<EXPProvider>();
        expProvider.OnUpdateLevel += UpdateUI;
        UpdateUI(null, expProvider.currentLevel);
    }

    private void UpdateUI(EXPProvider _, int level)
    {
        printer.text = $"Level {level}";
    }
}
