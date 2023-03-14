using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class NumberMagnet : MonoBehaviour
{
    [SerializeField] private SphereCollider trigger;
    [SerializeField] private ScaleFormulaSO scaleFormula;
    private IActor actor;
    private float rawRadius = 0;

    private void Start()
    {
        actor = GetComponentInParent<IActor>();

        actor.transform.GetComponent<EXPProvider>().OnUpdateLevel += NumberMagnet_OnUpdateLevel;

        var stat = actor.Stats[Characters.StatName.Magnet_Range];
        stat.OnChangeValue += Stat_OnChangeValue;
        Stat_OnChangeValue(stat);
    }

    private void NumberMagnet_OnUpdateLevel(EXPProvider arg1, int arg2)
    {
        RecalculateRadius(rawRadius);
    }

    private void Stat_OnChangeValue(Characters.Stat obj)
    {
        rawRadius = obj.Value;
        RecalculateRadius(rawRadius);
    }

    private void RecalculateRadius(float rawRadius)
    {
        var rad = rawRadius;
        if (rad < 0.1f)
            rad = 0.1f;
        else
            rad += scaleFormula.CalculateScale(actor.CurrentLevel);
        trigger.radius = rad;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent<Number3D>(out var number3D))
        {
            if (number3D.isMagneting)
                return;

            switch(number3D.operation)
            {
                case Operation.Multiplicate:
                case Operation.Summary:
                    {
                        number3D.isMagneting = true;
                        var mg = other.attachedRigidbody.gameObject.AddComponent<Magneting>();
                        mg.target = actor.transform;
                    }
                    return;
                default:
                    return;
            }
        }
    }
}
