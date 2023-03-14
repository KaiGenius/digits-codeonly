using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotData : MonoBehaviour
{
    [SerializeField] private Bot bot;
    [SerializeField] private ScaleFormulaSO scaleFormula;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Bot body;
    [SerializeField] private float speed = 10f, minSpeed = 5f;
    [SerializeField] private float seekRange = 15f;
    private float speedMult = 1;

    public ItemTracker currentTarget;
    public bool inHunterMode = false;

    public Bot Body => body;
    public new Rigidbody rigidbody => rb;
    public float SeekRange => seekRange;

    private void Start()
    {
        var speedStat = bot.Stats[Characters.StatName.Speed];
        speedStat.OnChangeValue += SpeedStat_OnChangeValue;
        SpeedStat_OnChangeValue(speedStat);
    }

    private void SpeedStat_OnChangeValue(Characters.Stat obj)
    {
        speedMult = obj.Value;
    }

    public float Speed()
    {
        var speedEval = scaleFormula.SpeedEvaulate(scaleFormula.CalculateScale(bot.currentScore));
        return Mathf.Lerp(speed, minSpeed, speedEval) * speedMult;
    }
}
