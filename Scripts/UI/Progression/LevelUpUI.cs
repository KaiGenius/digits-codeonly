using Core;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IClosable
{
    void Call(Player player);
    void Close();
    int LevelIncrease { get; }
}
public class LevelUpUI : UnitySingleton<LevelUpUI>
{
    public enum OpenState
    {
        GainPersistentBuff,
        GainTemporaryBuff,
    }

    [SerializeField] private GainRandomBuffsToPlayerUI gainPersistentBuff;
    [SerializeField] private GainInstanceBonusToPlayer gainTemporaryBuff;
    [SerializeField] private GameObject[] otherManagedUIs;
    public event Action<OpenState, int> OnCloseCallback;
    private OpenState openState;
    private bool isOpened = false;
    private IClosable ui;
    private Queue<OpenState> openCallbacksQueue = new Queue<OpenState>();
    private Vector3? localPos;
   

    public void InstallLevelUp(Action<int> installLevelUpDel)
    {
        gainPersistentBuff.installLevelUpDelegate = installLevelUpDel;
    }

    public void Open(OpenState state)
    {
        if (isOpened)
        {
            if (openState != OpenState.GainPersistentBuff && state == OpenState.GainPersistentBuff && !openCallbacksQueue.Contains(OpenState.GainPersistentBuff))
                openCallbacksQueue.Enqueue(state);
            return;
        }

        foreach (var otherM in otherManagedUIs)
            otherM.SetActive(false);

        Camera.main.GetComponent<TransformFollow>().SetTopDownCameraView(true);
        isOpened = true;
        openState = state;
        Observable.Timer(0.1f.sec()).TakeUntilDestroy(gameObject).Subscribe(_ => {
            
            ui = state switch
            {
                OpenState.GainPersistentBuff => gainPersistentBuff,
                OpenState.GainTemporaryBuff => gainTemporaryBuff,
                _ => null,
            };
            if (ui == null)
                return;

            if (!localPos.HasValue)
                localPos = transform.localPosition;

            transform.localPosition = localPos.Value + new Vector3(0, -Screen.height, 0);
            transform.DOLocalMove(localPos.Value, 1f);
            gameObject.SetActive(true);
            ui.Call(Player.ActiveInstance);
        });
    }

    public void Close()
    {
        ui?.Close();
    }

    private void OnEnable()
    {
        GameManager.IsGamePaused = true;
    }

    private void OnDisable()
    {
        Observable.NextFrame().Subscribe(_ => {
            GameManager.IsGamePaused = false;
            isOpened = false;
            OnCloseCallback?.Invoke(openState, ui.LevelIncrease);

            if (openCallbacksQueue.Count > 0)
                Open(openCallbacksQueue.Dequeue());
            else
            {
                foreach (var otherM in otherManagedUIs)
                    otherM.SetActive(true);
                Camera.main.GetComponent<TransformFollow>().SetTopDownCameraView(false);
            }
        });
    }
}
