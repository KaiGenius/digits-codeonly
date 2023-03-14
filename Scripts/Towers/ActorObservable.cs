using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorObservable : MonoBehaviour
{
    public event Action<IActor> OnEnter, OnExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.TryGetComponent<IActor>(out var actor))
        {
            OnEnter?.Invoke(actor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.attachedRigidbody != null && other.attachedRigidbody.TryGetComponent<IActor>(out var actor))
        {
            OnExit?.Invoke(actor);
        }
    }
}
