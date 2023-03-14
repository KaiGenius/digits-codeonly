using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerKillsCounter : MonoBehaviour
{
    public string outputFornula = "Kills: {0}";
    [SerializeField] private TextMeshProUGUI printer;
    private int counter = 0;

    private void Start()
    {
        var cc = Player.ActiveInstance.GetComponent<CollisionController>();
        cc.OnKill += Cc_OnKill;
        counter = -1;
        Cc_OnKill();
    }

    private void Cc_OnKill()
    {
        counter++;
        printer.text = string.Format(outputFornula, counter);
    }
}
