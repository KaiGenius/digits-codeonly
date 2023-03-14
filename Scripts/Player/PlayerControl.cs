using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float speed = 10, minSpeed = 5;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ScaleFormulaSO scaleFormula;
    private IActor actor;
    private float currentSpeed;
    private float speedMult = 1;

    private Vector3 input;

    public Vector3 Input => input;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = speed;
    }

    private void Start()
    {
        actor = GetComponent<IActor>();
        actor.OnUpdateScore += Actor_OnUpdateScore;
        currentSpeed = speed;

        var speedStat = actor.Stats[Characters.StatName.Speed];
        speedStat.OnChangeValue += SpeedStat_OnChangeValue;
        SpeedStat_OnChangeValue(speedStat);
    }

    private void SpeedStat_OnChangeValue(Characters.Stat obj)
    {
        speedMult = obj.Value;
    }

    private void Actor_OnUpdateScore(IActor obj, bool _)
    {
        var speedEval = scaleFormula.SpeedEvaulate(scaleFormula.CalculateScale(obj.CurrentLevel));
        currentSpeed = Mathf.Lerp(speed, minSpeed, speedEval);
    }

    private void FixedUpdate()
    {
        if(GameManager.IsGamePaused)
        {
            rb.velocity = default;
            return;
        }

        var vel = rb.velocity;
        var velY = vel.y;
        vel =  currentSpeed * speedMult * input;
        vel.y = velY;

        rb.velocity = vel;
    }

    public void SetInput(Vector3 input)
    {
        if (input.magnitude > 1)
            input.Normalize();
        this.input = input;
    }
}
