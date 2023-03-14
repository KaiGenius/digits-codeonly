using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityOverride : MonoBehaviour
{
    public float limitLocalDistance = 10f;
    [SerializeField] private Vector3 overrideVel;
    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        rb.velocity = overrideVel;
    }

    private void LateUpdate()
    {
        if (limitLocalDistance <= 0)
            return;

        var distance = transform.localPosition.magnitude;
        if (distance > limitLocalDistance)
        {
            transform.localPosition = Vector3.Lerp(default, transform.localPosition, limitLocalDistance / distance);
        }
    }
}
