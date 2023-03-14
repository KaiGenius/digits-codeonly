using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI printer;

    private void Start()
    {
        GameTimer.Self.OnTimeChange += Self_OnTimeChange;
    }

    private void Self_OnTimeChange(float time)
    {
        TimeSpan formated = time.sec();

        printer.text = formated.ToString(@"m\:ss");
    }
}
