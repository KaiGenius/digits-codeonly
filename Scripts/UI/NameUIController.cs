using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class NameUIController : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private ActorName viewPref;
    private ActorName view;
    private IActor actor;

    private void Start()
    {
        enabled = false;
        Observable.NextFrame().Subscribe(_ => { 
        if (TryGetComponent<IActor>(out actor))
        {
            view = Instantiate(viewPref, CanvasNameContainer.ActiveInstance.transform);
            view.SetText(actor.name);
            view.SetColor(actor.color);
            view.SetPosition(anchor.position);
            enabled = true;
        }
        else
            enabled = false;
        });
    }

    private void OnDisable()
    {
        if(view != null)
            view.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(view != null)
            view.gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if(view!=null)
        view.SetPosition(anchor.position);
    }

}
