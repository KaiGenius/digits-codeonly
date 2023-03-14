using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTracker : MonoBehaviour
{
    [SerializeField] private Number3D number;
    public int Number => number.number;
    public Operation Operation => number.operation;

    public event Action<ItemTracker> OnDestroyEvent;

    private void OnDestroy()
    {
        OnDestroyEvent?.Invoke(this);
        OnDestroyEvent = null;
    }

    private void Start()
    {
        ItemsPresenter.Self.AddItem(this);
    }
}
